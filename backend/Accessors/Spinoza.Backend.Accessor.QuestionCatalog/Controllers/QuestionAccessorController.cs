using AutoMapper;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;
using System;

namespace Spinoza.Backend.Accessor.QuestionCatalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionAccessorController : ControllerBase
    {
        private readonly ILogger<QuestionAccessorController> _logger;

        private readonly DaprClient _daprClient;
        private readonly ICosmosDBWrapper _cosmosDBWrapper;
        private readonly IMapper _mapper;

        public QuestionAccessorController(ILogger<QuestionAccessorController> logger, DaprClient daprClient, ICosmosDBWrapper cosmosDBWrapper, IMapper mapper)
        {
            _logger = logger;
            _daprClient = daprClient;
            _cosmosDBWrapper = cosmosDBWrapper;
            _mapper = mapper;
        }

        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestions(int? offset, int? limit)
        {
            throw new NotImplementedException();
        }
    }
}
