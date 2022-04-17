using Dapr;
using Microsoft.AspNetCore.Mvc;

namespace CatalogManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IntegrationEventsController : ControllerBase
    {
        private readonly ILogger<IntegrationEventsController> _logger;

        public IntegrationEventsController(ILogger<IntegrationEventsController> logger)
        {
            _logger = logger;
        }

        [Topic("pubsub", "test-topic")]
        public async Task<IActionResult> OnTestCreated([FromBody] string result)
        {
            try
            {
                _logger?.LogInformation($"Message received: {result}");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error Pubsub reciver: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }
    }
}
