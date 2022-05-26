using AutoMapper;
using CatalogManager.Helpers;
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
        private readonly IMapper _mapper;

        public IntegrationEventsController(ILogger<IntegrationEventsController> logger, DaprClient daprClient, IMapper mapper)
        {
            
            _logger = logger;
           _daprClient = daprClient;
            _mapper = mapper;
        }

        [Topic("pubsub", "test-topic")]
        public async Task<IActionResult> OnTestCreated([FromBody] Models.AccessorResults.TestChangeResult accessorTestChangeResult)
        {

            try
            {
                
                var frontendTestChangeResult = _mapper.Map<Models.FrontendResponses.TestChangeResult>(accessorTestChangeResult);
                _logger.LogInformation($"OnTestCreated: Message received: {frontendTestChangeResult.Id}");
                await PublishMessageToSignalRAsync(frontendTestChangeResult);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"OnTestCreated: Error Pubsub receiver: {ex.Message}");
                if (ex.InnerException != null)
                    _logger.LogError($"OnTestCreated: Error Pubsub receiver, inner exception: {ex.InnerException.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        private async Task PublishMessageToSignalRAsync(Models.FrontendResponses.TestChangeResult frontendTestChangeResult)
        {
            Data data = new ();
            Argument  argument = new Argument
            {
                Sender = "dapr",
                Text = frontendTestChangeResult
            };
            data.Arguments = new [] {argument};
            
            await _daprClient.InvokeBindingAsync("azuresignalroutput", "create", data);
            Ok();
        }
    }
}
