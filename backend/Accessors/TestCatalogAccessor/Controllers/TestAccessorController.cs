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
        private IConfiguration _configuration;
        private readonly ICosmosDBWrapper _cosmosDBWrapper;

        public TestAccessorController(ILogger<TestAccessorController> logger, DaprClient daprClient, IConfiguration configuration, ICosmosDBWrapper cosmosDBWrapper)
        {
            _logger = logger;
            _daprClient = daprClient;
            _configuration = configuration;
            _cosmosDBWrapper = cosmosDBWrapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTests()
        {
            try
            {
                List<TestModel> tests = new List<TestModel>();
                CosmosClient cosmosClient = new CosmosClient(_configuration["ConnectionStrings:Tests"]);
                Database database = cosmosClient.GetDatabase("Catalog");
                Container container = database.GetContainer("Tests");
                using (FeedIterator<TestModel> setIterator = container.GetItemLinqQueryable<TestModel>()
                          .ToFeedIterator<TestModel>())
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
                var response = await AddNewTestToDataBase(testInfo);
                await PublishTestResultAsync(response);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while getting all tests: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }
        private async Task PublishTestResultAsync(string testInfo)
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

        private async Task<TestModel> GetMessageFromBodyAsync()
        {
            var streamReader = new StreamReader(Request.Body);
            var body = await streamReader.ReadToEndAsync();
            _logger?.LogInformation($"Here is the test that goona try to enter the database {body}");
            var newTest = JsonConvert.DeserializeObject<TestModel>(body);
            return newTest ?? throw new Exception("Error when deserialize message body");
        }

        private async Task<string> AddNewTestToDataBase(TestModel body)
        {
           // CosmosClient cosmosClient = new CosmosClient(_configuration["ConnectionStrings:Tests"]);
           // await cosmosClient.GetDatabase($"Catalog")
           //.DefineContainer(name: $"Tests", partitionKeyPath: "/Title")
           //.WithUniqueKey()
           //.Path("/Title")
           //.Attach()
           //.CreateIfNotExistsAsync();
           // Database database = cosmosClient.GetDatabase("Catalog");
           // Container container = database.GetContainer("Tests");
           // try
           // {
           //     ItemResponse<TestModel> respons = await container.CreateItemAsync<TestModel>(body, new PartitionKey(body.Title));

           //     return $"Test Created {body.Title}";
           // }
           // catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
           // {
           //     return $"The test already exists! {body.Title}";
           // }
           var response = await _cosmosDBWrapper.CreateItemAsync(body);
            return $"Test created {body.Title}";

        }
    }
}
