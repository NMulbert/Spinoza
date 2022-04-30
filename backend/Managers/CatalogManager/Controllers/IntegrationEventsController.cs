﻿using AutoMapper;
using CatalogManager.Helpers;
using CatalogManager.Models;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
                _logger?.LogInformation($"Message received: {frontendTestChangeResult.Id}");
               await PublishMessageToSignalRAsync(frontendTestChangeResult);
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error Pubsub reciver: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        private async Task<IActionResult> PublishMessageToSignalRAsync(Models.FrontendResponses.TestChangeResult frontendTestChangeResult)
        {
            Data data = new ();
            Argument  argument = new Argument();
            argument.Sender = "dapr";
            argument.Text = frontendTestChangeResult;
            data.Arguments = new Argument [] {argument};
            //Dictionary<string, string> newmetadata = new Dictionary<string, string>() { { "hub", "spinozahub" } };
            //var metadata = new Dictionary<string, string>() { { "spinozaHub", "Test" } };
            await _daprClient.InvokeBindingAsync("azuresignalroutput", "create", data);
            return Ok();
        }
    }
}
