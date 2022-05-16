
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Spinoza.Backend.Accessor.TagCatalog.Models.Responses;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;

namespace Spinoza.Backend.Accessor.TagCatalog.Controllers;

[ApiController]
[Route("[controller]")]
public class TagAccessorController :ControllerBase
{
    private readonly ILogger<TagAccessorController> _logger;
    private readonly ICosmosDBWrapper _cosmosDBWrapper;
    private readonly DaprClient _daprClient;
     
    public TagAccessorController(ILogger<TagAccessorController> logger, DaprClient daprClient, ICosmosDBWrapper cosmosDBWrapper)
    {
        _logger = logger;
        _daprClient = daprClient;
        _cosmosDBWrapper = cosmosDBWrapper;
    }

    [HttpGet("/tags")]
    public async Task<IActionResult> GetTags()
    {
        try
        {
            var dbTags = await _cosmosDBWrapper.GetAllCosmosElementsAsync<Models.DB.Tag>();
            var tags = dbTags.Select(t => t.TagName).ToArray();
            return new OkObjectResult(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while getting tags: {ex.Message}");

        }
        return Problem(statusCode: StatusCodes.Status500InternalServerError);
    }
    
    
    
    [HttpPost("/tagaccessorrequestqueue")]
    public async Task<IActionResult> GetTagDataInputFromQueueBinding([FromBody] string[] tags)
    {
        try
        {
            var newTagList = new List<string>();
            
            foreach (var tag in tags)
            {
                try
                {
                    var result = await CreateTagAsync(new Models.DB.Tag {Id =Guid.NewGuid().ToString() , TagName = tag});
                    if (result)
                    {
                        newTagList.Add(tag);
                        _logger.LogInformation($"new tag was added: {tag}");
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogWarning($"The tag :{tag} has not been added. Reason: {ex.Message}");
                }
            }

            if (newTagList.Any())
            {
                var tagChangeResult = new TagChangeResult()
                {
                    Id = "null",
                    MessageType = "CreateTag",
                    ActionResult = "Success",
                    Reason = string.Join(",", newTagList),
                    Sender = "Catalog",
                    ReasonId = 6,
                    ResourceType = "Tag"
                };
                await PublishTagResultAsync(tagChangeResult); 
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while creating or updating a tags: {ex.Message}");
           
        }
        
        return Problem(statusCode: StatusCodes.Status500InternalServerError);
    }
    
    private async Task<bool> CreateTagAsync(Models.DB.Tag dbTag)
    {
        var result = await _cosmosDBWrapper.CreateItemAsync(dbTag);
        return result.StatusCode.IsOk();
    }
    
    
    private async Task PublishTagResultAsync(TagChangeResult tagChangeResult)
    {
        try
        {
            await _daprClient.PublishEventAsync("pubsub", "tag-topic", tagChangeResult);
        }
        catch (Exception ex)
        {
            _logger.LogError($"PublishTagResultAsync: error in publishing tag results: {ex.Message}");
        }
    }
    

}