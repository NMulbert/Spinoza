using AutoMapper;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using Spinoza.Backend.Accessor.TestCatalog.Models;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;


namespace Spinoza.Backend.Accessor.TestCatalog.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TestAccessorController : ControllerBase
    {
        private readonly ILogger<TestAccessorController> _logger;

        private readonly DaprClient _daprClient;
        private readonly ICosmosDBWrapper _cosmosDBWrapper;
        private readonly IMapper _mapper;

        public TestAccessorController(ILogger<TestAccessorController> logger, DaprClient daprClient, ICosmosDBWrapper cosmosDBWrapper, IMapper mapper)
        {
            _logger = logger;
            _daprClient = daprClient;
            _cosmosDBWrapper = cosmosDBWrapper;
            _mapper = mapper;
        }

        [HttpGet("tests")]
        public async Task<IActionResult> GetTests(int? offset, int? limit)
        {
            try
            {
                
                var dbTests = await _cosmosDBWrapper.GetAllCosmosElementsAsync<Models.DB.Test>(offset ?? 0, limit ?? 100);
                var resultTests = _mapper.Map<List<Models.Results.Test>>(dbTests);

                return new OkObjectResult(resultTests);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting tests: {ex.Message}");

            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }
        
        [HttpGet("/test/{id:Guid}")]

        public async Task<IActionResult> GetTest(Guid id)
        {
            var query = new QueryDefinition("SELECT * FROM ITEMS item WHERE item.id = @id").WithParameter("@id", id.ToString().ToUpper());
            try
            {
                var dbTest = (await _cosmosDBWrapper.GetCosmosElementsAsync<Models.DB.Test>(query)).FirstOrDefault();
                if (dbTest == null)
                {
                    return new NotFoundObjectResult(id.ToString());
                }
                //else
                var resultTest = _mapper.Map<Models.Results.Test>(dbTest);

                return new OkObjectResult(resultTest);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting tests: {ex.Message}");

            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }
        

        [HttpPost("/testaccessorrequestqueue")]
        public async Task<IActionResult> GetTestDataInputFromQueueBinding()
        {
           

            Models.Requests.Test? testRequest = null;
            try
            {
                
                testRequest = await GetMessageFromBodyAsync();
                Models.Responses.TestChangeResult? result = null;
                if (testRequest.MessageType == "CreateTest")
                {
                    result = await CreateTestAsync(testRequest);                    
                }
                else if(testRequest.MessageType == "UpdateTest")
                {
                    

                    result = await UpdateTestAsync(testRequest);
                }
                else
                {
                    result = CreateTestResult(testRequest.Id, "UnknownRequest", "Understand only CreateTest or UpdateTest", 1, false);
                    
                }
                
                await PublishTestResultAsync(result);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creatring or updating a test: {ex.Message}");
                var result = CreateTestResult( testRequest?.Id ?? Guid.Empty.ToString(), "Unknown" ,$"InternalServerError: {ex.Message}", 666, false);
                await PublishTestResultAsync(result);
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);

        }
        private Models.Responses.TestChangeResult CreateTestResult(string testId, string messageType, string reason, int reasonId, bool isSuccess=true )
        {
            return new Models.Responses.TestChangeResult()
            {
                Id = testId,
                MessageType = messageType,
                ActionResult = isSuccess? "Success" : "Error",
                Reason = reason,
                Sender = "Catalog",
                ReasonId = reasonId,
                ResourceType = "Test"
            };
        }
        
        
        private async Task<Models.Responses.TestChangeResult> CreateTestAsync(Models.Requests.Test testRequest)
        {

            var dbTest = _mapper.Map<Models.DB.Test>(testRequest);
            var result = await _cosmosDBWrapper.CreateItemAsync(dbTest);
            if(result.StatusCode.IsOk())
            {
                return CreateTestResult(dbTest.Id, "Create", $"Test {dbTest.Title} has been created", 2);

            }
            //else
            return CreateTestResult(dbTest.Id, "Create", $"Test {dbTest.Title} creation has been failed", 3, false);

        }

        private async Task PublishTestResultAsync(Models.Responses.TestChangeResult testChangeResult)
        {
            try
            {
                await _daprClient.PublishEventAsync("pubsub", "test-topic", testChangeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetTestFromQueueRequestAsync: error getting test from queue error while getting all tests: {ex.Message}");
            }
        }
        public async Task<Models.Responses.TestChangeResult> UpdateTestAsync(Models.Requests.Test testRequest)
        {
           
            var newDbTest = _mapper.Map<Models.DB.Test>(testRequest);
            var result = await _cosmosDBWrapper.UpdateItemAsync(newDbTest, item => item._etag, item =>  Guid.Parse(item.Id), TestMerger);
            if(result != null && result.StatusCode.IsOk())
            {
                return CreateTestResult(newDbTest.Id, "Update", $"Test {newDbTest.Title} has been updated", 4);
            }
            //else
            return CreateTestResult(newDbTest.Id, "Update", $"Test {newDbTest.Title} update has been failed", 5, false);


            Models.DB.Test TestMerger(Models.DB.Test dbItem, Models.DB.Test newItem)
            {
                dbItem.AuthorId = newItem.AuthorId;
                dbItem.Description = newItem.Description;
                dbItem.Status = newItem.Status;
                dbItem.LastUpdateCreationTimeUTC = newItem.LastUpdateCreationTimeUTC;
                dbItem.Title = newItem.Title;
                dbItem.Questions = newItem.Questions;
                dbItem.Tags = newItem.Tags;
                return dbItem;
            }
        }

        private async Task<Models.Requests.Test> GetMessageFromBodyAsync()
        {
            var streamReader = new StreamReader(Request.Body);
            var body = await streamReader.ReadToEndAsync();
            _logger?.LogInformation($"Here is the test that goona try to enter the database {body}");
            var newTest = JsonConvert.DeserializeObject<Models.Requests.Test>(body);
            return newTest ?? throw new Exception("Error when deserialize message body");
        }

        [HttpGet("/tests/count")]
        public async Task<IActionResult> GetTotalTests()
        {
            try
            {
                int? dbTotalTests = await _cosmosDBWrapper.GetScalarCosmosQueryResult<int?>(new QueryDefinition("SELECT VALUE COUNT(1) FROM c"));
           
                if (dbTotalTests == null)
                {
                    return new NotFoundObjectResult("No Tests found");
                }
                //else
                return new OkObjectResult(dbTotalTests);
            }
            catch (Exception ex) 
            {
                    _logger.LogError($"Error while getting tests: {ex.Message}");

            }
           
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }
    }
}
