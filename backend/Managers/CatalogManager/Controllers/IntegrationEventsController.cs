using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace CatalogManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IntegrationEventsController : ControllerBase
    {

        private readonly ILogger<IntegrationEventsController> _logger;
        private readonly DaprClient _daprClient;

        public IntegrationEventsController(ILogger<IntegrationEventsController> logger, DaprClient daprClient)
        {
            System.Diagnostics.Debugger.Launch();
            System.Diagnostics.Debugger.Break();
            _logger = logger;
            _daprClient = daprClient;
        }

        [Topic("pubsub", "test-topic")]
        public async Task<IActionResult> OnTestCreated([FromBody] string result)
        {
            System.Diagnostics.Debugger.Launch();
            try
            {
                _logger?.LogInformation($"Message received: {result}");
               //await PublishMessageToSignalRAsync(result);
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error Pubsub reciver: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        public async Task<IActionResult> PublishMessageToSignalRAsync(string massege)
        {
            await _daprClient.InvokeBindingAsync("azuresignalroutput", "create", massege);
            return Ok();
        }
    }
}
