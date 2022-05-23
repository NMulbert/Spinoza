using AutoMapper;
using CatalogManager.Helpers;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace CatalogManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IntegrationEventsQuestionController : ControllerBase
    {
        private readonly ILogger<IntegrationEventsQuestionController> _logger;
        private readonly DaprClient _daprClient;
        private readonly IMapper _mapper;

        public IntegrationEventsQuestionController(ILogger<IntegrationEventsQuestionController> logger, DaprClient daprClient, IMapper mapper)
        {

            _logger = logger;
            _daprClient = daprClient;
            _mapper = mapper;
        }

        [Topic("pubsub", "question-topic")]
        public async Task<IActionResult> OnQuestionCreated([FromBody] Models.AccessorResults.QuestionChangeResult accessorQuestionChangeResult)
        {
            //System.Diagnostics.Debugger.Launch();
            try
            {
                var frontendQuestionChangeResult = _mapper.Map<Models.FrontendResponses.QuestionChangeResult>(accessorQuestionChangeResult);
                _logger?.LogInformation($"OnQuestionCreated: Message received: {frontendQuestionChangeResult.Id}");
                await PublishQuestionMessageToSignalRAsync(frontendQuestionChangeResult);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"OnQuestionCreated: Error Pubsub reciver: {ex.Message}");
                if (ex.InnerException != null)
                    _logger.LogError($"OnQuestionCreated: Error Pubsub reciver, inner exception: {ex.InnerException.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }


        private async Task<IActionResult> PublishQuestionMessageToSignalRAsync(Models.FrontendResponses.QuestionChangeResult frontendQuestionChangeResult)
        {
            Data data = new();
            Argument argument = new Argument();
            argument.Sender = "dapr";
            argument.Text = frontendQuestionChangeResult;
            data.Arguments = new Argument[] { argument };
            //Dictionary<string, string> newmetadata = new Dictionary<string, string>() { { "hub", "spinozahub" } };
            //var metadata = new Dictionary<string, string>() { { "spinozaHub", "Test" } };
            await _daprClient.InvokeBindingAsync("azuresignalroutput", "create", data);
            return Ok();
        }
    }
}
