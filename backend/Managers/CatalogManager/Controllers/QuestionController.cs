using AutoMapper;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using CatalogManager.Models.FrontendRequests;

namespace CatalogManager.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly ILogger<QuestionController> _logger;
        private readonly DaprClient _daprClient;
        private readonly IMapper _mapper;

        public QuestionController(ILogger<QuestionController> logger, DaprClient daprClient, IMapper mapper)
        {
            _logger = logger;
            _daprClient = daprClient;
            _mapper = mapper;
        }


        [HttpPost("/question")]
        public async Task<IActionResult> PostNewQuestionToQueue()
        {
            return await PostNewOrUpdateQuestionToQueue(false);
        }


        [HttpPut("/question")]
        public async Task<IActionResult> PutNewQuestiontToQueue()
        {
            return await PostNewOrUpdateQuestionToQueue(true);
        }

        [HttpGet("/allquestions")]
        public async Task<IActionResult> GetAllQuestions(int? offset, int? limit)
        {
            
            try
            {
                var allAccessorQuestions = await _daprClient.InvokeMethodAsync<JsonArray>(HttpMethod.Get, "questionaccessor", $"questionaccessor/allquestions?offset={offset ?? 0 }&limit={limit ?? 100}");

                _logger?.LogInformation($"returned {allAccessorQuestions.Count} questions");

                return new OkObjectResult(allAccessorQuestions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting all questions: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        [HttpGet("/question/{id:Guid}")]

        public async Task<IActionResult> GetQuestionById(Guid id)
        {
            try
            {
                var accessorQuestionType = await _daprClient.InvokeMethodAsync<Models.AccessorResults.CommonQuestion>(HttpMethod.Get, "questionaccessor", $"question/{id}");

                if (accessorQuestionType == null)
                {
                    _logger.LogWarning($"GetQuestionById: accessor returnes null for question: {id}");
                    return new NotFoundObjectResult(id);
                }

                switch (accessorQuestionType.Type)
                {
                    case "MultipleChoiceQuestion":

                        return await GetQuestion<Models.AccessorResults.MultipleChoiceQuestion>();

                    case "OpenTextQuestion":

                        return await GetQuestion<Models.AccessorResults.OpenTextQuestion>();

                    default:

                        _logger?.LogInformation($"Question type: {accessorQuestionType.Type} is incompatible");

                        return new BadRequestObjectResult(id);
                }


                async Task<IActionResult> GetQuestion<TQuestion>() where TQuestion : Models.AccessorResults.IQuestion
                {
                    var questionModel = await _daprClient.InvokeMethodAsync<TQuestion>(HttpMethod.Get, "questionaccessor", $"question/{id}");

                    var questionResult = _mapper.Map<TQuestion>(questionModel);

                    _logger?.LogInformation($"returned question id: {id}");

                    return new OkObjectResult(questionResult);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting a question: {id} Error: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }


        private async Task<IActionResult> PostNewOrUpdateQuestionToQueue(bool isUpdate)
        {
            try
            {
                using var streamReader = new StreamReader(Request.Body);
                var body = await streamReader.ReadToEndAsync();
                var requestQuestionType = JsonConvert.DeserializeObject<Models.FrontendRequests.CommonQuestion>(body);

                if (requestQuestionType == null)
                {
                    _logger.LogWarning($"Request question type is missing");
                    return new NotFoundObjectResult(body);
                }

                switch (requestQuestionType.Type)
                {
                    case "MultipleChoiceQuestion":

                        return await PostQueue<Models.FrontendRequests.MultipleChoiceQuestion>();

                    case "OpenTextQuestion":

                        return await PostQueue<Models.FrontendRequests.OpenTextQuestion>();

                    default:
                        break;
                }

                async Task<IActionResult> PostQueue<TQuestion>() where TQuestion : Models.FrontendRequests.IQuestion
                {
                  
                    var questionRequest = JsonConvert.DeserializeObject<TQuestion>(body);

                    var questionModel = _mapper.Map<TQuestion>(questionRequest);

                    _logger.LogInformation("the message is going to queue");

                    questionModel.QuestionVersion = "1.0";
                    questionModel.MessageType = isUpdate ? "UpdateQuestion" : "AddQuestion";
                    
                    await _daprClient.InvokeBindingAsync("questionaccessorrequestqueue", "create", questionModel);

                    if (questionModel.Tags.Any())
                    {
                          _logger.LogInformation($"pushing {questionModel.Tags.Length} tags to the tagaccessorrequestqueue");  
                         await _daprClient.InvokeBindingAsync("tagaccessorrequestqueue", "create", questionModel.Tags);
                    }
                    else
                    {
                        _logger.LogInformation("questions with no tags");  
                    }
                    
                    return Ok("Accepted");
                }
                
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when {0} ending addquestion post: {1}", isUpdate ? "updating" : "creating", ex.Message);
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        
        [HttpGet("/questions/count")]
        public async Task<IActionResult> GetTotalQuestionCount()
        {
            try
            {
                var dbTotalQuestions = await _daprClient.InvokeMethodAsync<int>(HttpMethod.Get, "questionaccessor", "/questions/count");
                _logger.LogInformation($"GetTotalQuestionCount: accessor returns {dbTotalQuestions}");
                return new OkObjectResult(dbTotalQuestions);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while getting total questions number. Error: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

    }
    
   

}
