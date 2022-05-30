using AutoMapper;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;
using System.Text.Json.Nodes;

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
        public async Task<IActionResult> GetTests(int? offset, int? limit )
        {
            var tags = Request.Query["tags"].ToString();
            try
            {
                IList<Models.DB.Test>? dbTests;
                   
                if (string.IsNullOrEmpty(tags))
                {
                    dbTests = await _cosmosDBWrapper.GetAllCosmosElementsAsync<Models.DB.Test>(offset ?? 0, limit ?? 100);
                }
                else
                {
                    var query =
                        $" SELECT DISTINCT test FROM test JOIN tag IN test.tags WHERE tag IN ({tags}) OFFSET @skip LIMIT @count";
                    _logger.LogInformation($"GetTests: The query is: {query}");
                    var tests = await _cosmosDBWrapper.GetCosmosElementsAsync<JsonNode>(new QueryDefinition(query).WithParameter("@skip", offset ?? 0)
                        .WithParameter("@count", limit ?? 100));

                    dbTests = tests.Select(j => JsonConvert.DeserializeObject<Models.DB.Test>(j["test"]!.ToJsonString())!).ToList();
                }

                var resultTests = _mapper.Map<List<Models.Results.Test>>(dbTests);
                return new OkObjectResult(resultTests);
               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting tests: {ex.Message}");

            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        
        [HttpGet("/testquestions/{id:Guid}")]
        public async Task<IActionResult> GetQuestionsByTestId(Guid id)
        {
            try
            {
                //System.Diagnostics.Debugger.Launch();

                var query = new QueryDefinition("SELECT item.questionsRefs FROM ITEMS item WHERE item.id = @id").WithParameter("@id", id.ToString().ToUpper());

                var dbTestQuestionsIds = await _cosmosDBWrapper.GetCosmosElementsAsync<JsonNode>(query);

                var allTestQuestionsIds = dbTestQuestionsIds[0]["questionsRefs"]!.AsArray();

                return new OkObjectResult(allTestQuestionsIds);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting test questions: {ex.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
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
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }


        public async Task<Models.Responses.TestChangeResult> DeleteTestAsync(Models.Requests.Test? testDeleteInfo)
        {
            var response = new Models.Responses.TestChangeResult()
            {
                Id = testDeleteInfo?.Id ?? "Unknown",
                MessageType = "Deleted",
                ActionResult = "Error", 
                Reason = "Delete test",
                Sender = "Catalog",
                ReasonId = 12,
                ResourceType = "Test"
            };

            if (testDeleteInfo == null)
            {
                return response;
            }

            try
            {
                bool result = await _cosmosDBWrapper.DeleteItemAsync(testDeleteInfo.Id, testDeleteInfo.TestVersion);
                if (result == false)
                {
                    return response;
                }
                response.ActionResult = "Success";
                //else
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting a test: {ex.Message}");

            }
            response.ActionResult = "Error";
            return response;
        }


        [HttpPost("/testaccessorrequestqueue")]
        public async Task<IActionResult> GetTestDataInputFromQueueBinding()
        {
            Models.Requests.Test? testRequest = null;
            try
            {
                testRequest = await GetMessageFromBodyAsync();
                Models.Responses.TestChangeResult? result;
                if (testRequest.MessageType == "CreateTest")
                {
                    result = await CreateTestAsync(testRequest);
                }
                else if (testRequest.MessageType == "UpdateTest")
                {
                    result = await UpdateTestAsync(testRequest);
                }
                else if (testRequest.MessageType == "DeleteTest")
                {
                    result = await DeleteTestAsync(testRequest);
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
                _logger.LogError($"Error while creating or updating a test: {ex.Message}");
                var result = CreateTestResult(testRequest?.Id ?? Guid.Empty.ToString(), "Unknown", $"InternalServerError: {ex.Message}", 666, false);
                await PublishTestResultAsync(result);
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
        
        private Models.Responses.TestChangeResult CreateTestResult(string testId, string messageType, string reason, int reasonId, bool isSuccess=true )
        {
            return new Models.Responses.TestChangeResult()
            {
                Id = testId,
                MessageType = messageType,
                ActionResult = isSuccess ? "Success" : "Error",
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
            if (result.StatusCode.IsOk())
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
            var result = await _cosmosDBWrapper.UpdateItemAsync(newDbTest, item => item._etag, item => Guid.Parse(item.Id), TestMerger);
            if (result != null && result.StatusCode.IsOk())
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
                dbItem.QuestionsRefs = newItem.QuestionsRefs;
                dbItem.Tags = newItem.Tags;
                return dbItem;
            }
        }

        private async Task<Models.Requests.Test> GetMessageFromBodyAsync()
        {
            var streamReader = new StreamReader(Request.Body);
            var body = await streamReader.ReadToEndAsync();
            _logger.LogInformation($"Here is the test that going to try to enter the database {body}");
            var newTest = JsonConvert.DeserializeObject<Models.Requests.Test>(body);
            return newTest ?? throw new Exception("Error when deserialize message body");
        }

        [HttpGet("/tests/count")]
        public async Task<IActionResult> GetTotalTests()
        {
            var tags = Request.Query["tags"].ToString();
            _logger.LogInformation($"the tags are: {tags}");
            try
            { if (string.IsNullOrEmpty(tags))
                {
                    var dbTotalTestsCount =
                        await _cosmosDBWrapper.GetScalarCosmosQueryResult<int>(
                            new QueryDefinition($"SELECT VALUE COUNT(1) FROM c"));
                    return new OkObjectResult(dbTotalTestsCount);
                }
                //else
                var totalTestsByTags = await _cosmosDBWrapper.GetScalarCosmosQueryResult<JsonNode>(new QueryDefinition
                    ($"SELECT DISTINCT COUNT(1) test FROM test JOIN tag IN test.tags WHERE tag IN ({tags})")
                );
                if (totalTestsByTags == null)
                {
                    _logger.LogWarning("GetTotalTests : No Tests found");
                    return new NotFoundObjectResult("No Tests found");
                }
                //else
                _logger.LogInformation(totalTestsByTags.ToJsonString());
                int count = totalTestsByTags["test"]!.GetValue<int>();
                return new OkObjectResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting tests: {ex.Message}");

            }

            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
