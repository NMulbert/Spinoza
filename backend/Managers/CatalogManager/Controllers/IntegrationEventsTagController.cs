using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CatalogManager.Helpers;
using Dapr;
using Dapr.Client;

namespace CatalogManager.Controllers;

[ApiController]
[Route("[controller]")]
public class IntegrationEventsTagController : ControllerBase
{

        private readonly ILogger<IntegrationEventsTagController> _logger;
        private readonly DaprClient _daprClient;
        private readonly IMapper _mapper;

        public IntegrationEventsTagController(ILogger<IntegrationEventsTagController> logger, DaprClient daprClient, IMapper mapper)
        {

            _logger = logger;
            _daprClient = daprClient;
            _mapper = mapper;
        }

        [Topic("pubsub", "tag-topic")]
        public async Task<IActionResult> OnTagCreated([FromBody] Models.AccessorResults.TagChangeResult accessorTagChangeResult)
        {
            //System.Diagnostics.Debugger.Launch();
            try
            {
                var frontendTagChangeResult = _mapper.Map<Models.FrontendResponses.TagChangeResult>(accessorTagChangeResult);
                _logger.LogInformation($"OnTagCreated : Message received: {frontendTagChangeResult.Id}");
                return await PublishTagMessageToSignalRAsync(frontendTagChangeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"OnTagCreated: Error Pubsub receiver: {ex.Message}");
                if (ex.InnerException != null)
                    _logger.LogError($"OnTagCreated: Error Pubsub receiver, inner exception: {ex.InnerException.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }


        private async Task<IActionResult> PublishTagMessageToSignalRAsync(Models.FrontendResponses.TagChangeResult frontendTagChangeResult)
        {
            Data data = new();
            Argument argument = new Argument
            {
                Sender = "dapr",
                Text = frontendTagChangeResult
            };
            data.Arguments = new [] { argument };
  
            await _daprClient.InvokeBindingAsync("azuresignalroutput", "create", data);
            return Ok();
        }
    }
