using AutoMapper;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;

namespace Spinoza.Backend.Accessor.TagCatalog.Controllers;

[ApiController]
[Route("[controller]")]
public class TagAccessorController :ControllerBase
{
    private readonly ILogger<TagAccessorController> _logger;

    private readonly DaprClient _daprClient;
    private readonly ICosmosDBWrapper _cosmosDBWrapper;
    private readonly IMapper _mapper;
     
    public TagAccessorController(ILogger<TagAccessorController> logger, DaprClient daprClient, ICosmosDBWrapper cosmosDBWrapper, IMapper mapper)
    {
        _logger = logger;
        _daprClient = daprClient;
        _cosmosDBWrapper = cosmosDBWrapper;
        _mapper = mapper;
    }

    
}