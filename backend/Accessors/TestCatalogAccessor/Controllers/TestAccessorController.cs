using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using Spinoza.Backend.Accessor.TestCatalog.DataBases;
using Spinoza.Backend.Accessor.TestCatalog.Models;
using System.Net;


namespace Spinoza.Backend.Accessor.TestCatalog.Controllers 
{
    [ApiController]
    [Route("[controller]")]
    public class TestAccessorController : ControllerBase
    {
        private readonly ILogger<TestAccessorController> _logger;

        private readonly DaprClient _daprClient;
        private readonly ICosmosDBWrapper _cosmosDBWrapper;

        public TestAccessorController(ILogger<TestAccessorController> logger, DaprClient daprClient, ICosmosDBWrapper cosmosDBWrapper)
        {
            _logger = logger;
            _daprClient = daprClient;
            _cosmosDBWrapper = cosmosDBWrapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTests()
        {
            try
            {
                List<TestModel> tests = new List<TestModel>();
                using (FeedIterator<TestModel> setIterator = _cosmosDBWrapper.Container.GetItemLinqQueryable<TestModel>()
                          .ToFeedIterator())
                {
                    //Asynchronous query execution
                    while (setIterator.HasMoreResults)
                    {
                        foreach (var item in await setIterator.ReadNextAsync())
                        {
                            tests.Add(item);
                        }
                    }
                }
                return new OkObjectResult(tests);
            }
            catch (Exception ex)
            {
                _logger.LogError($"error whilte getting all tests: {ex.Message}");

            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        [HttpPost("/azurequeueinput")]
        public async Task<IActionResult> GetTestDataInputFromQueueBinding()
        {
            try
            {
                var testInfo = await GetMessageFromBodyAsync();
                var response = await _cosmosDBWrapper.CreateItemAsync(testInfo);
                //var response = await AddNewTestToDataBase(testInfo);
                await PublishTestResultAsync(testInfo);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while getting all tests: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }
        private async Task PublishTestResultAsync(TestModel testInfo)
        {
            try
            {
                await _daprClient.PublishEventAsync("pubsub", "test-topic", testInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetTestFromQueueRequestAsync: error getting test from queue error while getting all tests: {ex.Message}");
            }
        }
        public async Task<IActionResult> UpdateTest(TestModel testModel)
        {
            var result = await _cosmosDBWrapper.UpdateItemAsync(testModel, item => item.ETag, item => item.Id, TestMerger);
            return Ok();
        }

        private TestModel TestMerger(TestModel dbItem, TestModel newItem )
        {
            dbItem.Author = newItem.Author;
            dbItem.Description = newItem.Description;
            dbItem.Status = newItem.Status;
            dbItem.Questions = dbItem.Questions.Union(newItem.Questions).ToList();
            dbItem.Title = newItem.Title;
            return dbItem;
        }

        private async Task<TestModel> GetMessageFromBodyAsync()
        {
            var streamReader = new StreamReader(Request.Body);
            var body = await streamReader.ReadToEndAsync();
            _logger?.LogInformation($"Here is the test that goona try to enter the database {body}");
            var newTest = JsonConvert.DeserializeObject<TestModel>(body);
            return newTest ?? throw new Exception("Error when deserialize message body");
        }

        //private async Task<string> AddNewTestToDataBase(TestModel body)
        //{
        //   // try
        //   // {
        //   //     ItemResponse<TestModel> respons = await container.CreateItemAsync<TestModel>(body, new PartitionKey(body.Title));

        //   //     return $"Test Created {body.Title}";
        //   // }
        //   // catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        //   // {
        //   //     return $"The test already exists! {body.Title}";
        //   // }
           
        //    return $"Test created {body.Title}";

        //}
    }
}
