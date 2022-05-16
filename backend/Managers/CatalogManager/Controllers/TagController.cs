using Microsoft.AspNetCore.Mvc;
using Dapr.Client;

namespace CatalogManager.Controllers;

[ApiController]
[Route("[controller]")]

public class TagController : ControllerBase
{
    private readonly ILogger<TagController> _logger;

    private readonly DaprClient _daprClient;
    
    public TagController(ILogger<TagController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
   
    }
    
    [HttpGet("/tags")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var allTags = await _daprClient.InvokeMethodAsync<List<string>>(HttpMethod.Get, "tagaccessor", $"/tags");
            
            _logger.LogInformation($"returned {allTags.Count} tags");
            return new OkObjectResult(allTags);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while getting all tags: {ex.Message}");
        }
        return Problem(statusCode: StatusCodes.Status500InternalServerError);
    }

}