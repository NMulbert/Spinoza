using CatalogManager.Models;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CatalogManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        private readonly DaprClient _daprClient;
        public TestController(ILogger<TestController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [HttpPost("/addtest")]
        public async Task<IActionResult> PostNewTestToQueue()
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                var body = await streamReader.ReadToEndAsync();
                var newTestModel = JsonConvert.DeserializeObject<TestModel>(body);
                _logger.LogInformation("the message is going to queue");
                await _daprClient.InvokeBindingAsync("azurequeueoutput", "create", newTestModel);
                return Ok(newTestModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"error then ending addtest post: {ex.Message}");

            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }
        [HttpGet("/alltests")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<TestModel> allTests = new List<TestModel>();
                allTests = await _daprClient.InvokeMethodAsync<List<TestModel>>(HttpMethod.Get, "testaccessor", "testaccessor/all");
                _logger?.LogInformation($"All the test in the test database {allTests}");
                return new OkObjectResult(allTests);
            }
            catch (Exception ex)
            {
                _logger.LogError($"error whilte getting all tests: {ex.Message}");

            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }
    }
}
