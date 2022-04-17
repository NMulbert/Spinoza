using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using System.Net;
using TestCatalogAccessor.Models;

namespace TestCatalogAccessor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestAccessorController : ControllerBase
    {
        private readonly ILogger<TestAccessorController> _logger;

        private readonly DaprClient _daprClient;
        private IConfiguration _configuration;
        public TestAccessorController(ILogger<TestAccessorController> logger, DaprClient daprClient, IConfiguration configuration)
        {
            _logger = logger;
            _daprClient = daprClient;
            _configuration = configuration;
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
                var command = await GetTestFromQueueRequestAsync();
                _logger?.LogInformation($"here is a massege from queuetest: {command}");
                return Ok(command);
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while getting all tests: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }
        private async Task<TestModel> GetTestFromQueueRequestAsync()
        {
            using var streamReader = new StreamReader(Request.Body);
            var body = await streamReader.ReadToEndAsync();
            _logger?.LogInformation($"Here is the test that goona try to enter the database {body}");
            TestModel newTest = JsonConvert.DeserializeObject<TestModel>(body);
            await AddNewTestToDataBase(newTest);
            //string something = "hell";
            //_logger?.LogInformation($"Here is the test after dsiarilization {responce.ToString()}");
            //await _daprClient.PublishEventAsync("pubsub", "test-topic", responce.ToString());
            //var bytes = Convert.FromBase64String(body);
            //var decodedString = System.Text.Encoding.UTF8.GetString(bytes);
            //var command = JsonConvert.DeserializeObject(decodedString);
            return newTest;
        }

        private async Task<IActionResult> AddNewTestToDataBase(TestModel body)
        {
            CosmosClient cosmosClient = new CosmosClient(_configuration["ConnectionStrings:Tests"]);
            await cosmosClient.GetDatabase($"Catalog")
           .DefineContainer(name: $"Tests", partitionKeyPath: "/Title")
           .WithUniqueKey()
           .Path("/Title")
           .Attach()
           .CreateIfNotExistsAsync();
            Database database = cosmosClient.GetDatabase("Catalog");
            Container container = database.GetContainer("Tests");
            try
            {
                ItemResponse<TestModel> respons = await container.CreateItemAsync<TestModel>(body, new PartitionKey(body.Title));

                return Ok("Test Created");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return BadRequest(ex);
            }
        }
    }
}
