using AutoMapper;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
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
            try
            {
                var dbQuestions = await _cosmosDBWrapper.GetAllCosmosElementsAsync<Models.DB.IQuestion>(offset ?? 0, limit ?? 100);

                var resultQuestions = _mapper.Map<List<Models.Results.IQuestion>>(dbQuestions);

                return new OkObjectResult(resultQuestions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting questions: {ex.Message}");

            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        [HttpGet("/question/{id:Guid}")]

        public async Task<IActionResult> GetQuestionById(Guid id)
        {
            try
            {

                var query = new QueryDefinition("SELECT * FROM ITEMS item WHERE item.id = @id").WithParameter("@id", id.ToString().ToUpper());

                var dbQuestion = (await _cosmosDBWrapper.GetCosmosElementsAsync<Models.DB.IQuestion>(query)).FirstOrDefault();

                if (dbQuestion == null)
                {
                    _logger.LogWarning($"GetQuestionById: accessor returnes null for question: {id}");
                    return new NotFoundObjectResult(id);
                }

                switch (dbQuestion.Type)
                {
                    case "MultipleChoiceQuestion":

                        return MapQuestion<Models.DB.MultipleChoiceQuestion>();

                    case "OpenTextQuestion":

                        return MapQuestion<Models.DB.OpenTextQuestion>();

                    default:
                        break;
                }

                IActionResult MapQuestion<TQuestion>() where TQuestion : Models.DB.IQuestion
                {
                    var questionResultModel = _mapper.Map<TQuestion>(dbQuestion);

                    _logger?.LogInformation($"returned question id: {id}");

                    return new OkObjectResult(questionResultModel);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting a question: {id} Error: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        [HttpPost("/questionqueue")]

        public async Task<IActionResult> GetQuestionDataInputFromQueueBinding()
        {
            try
            {
                var questionRequest = await GetMessageFromBodyAsync();
                Models.Responses.QuestionChangeResult? result = null;

                switch (questionRequest.MessageType)
                {
                    case "CreateQuestion":

                        result = await CreateQuestionAsync(questionRequest);
                        return Ok();

                    case "UpdateQuestion":

                        return Ok();

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creatring or updating a question: {ex.Message}");

            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        private async Task<Models.Responses.QuestionChangeResult> CreateQuestionAsync(Models.Requests.IQuestion questionRequest)
        {

            var dbQuestion = _mapper.Map<Models.DB.IQuestion>(questionRequest);

            var result = await _cosmosDBWrapper.CreateItemAsync(dbQuestion);

            if (result.StatusCode.IsOk())
            {
                return CreateTestResult(dbQuestion.Id, "Create", $"Question {dbQuestion.Name} has been created", 2);

            }
            return CreateTestResult(dbQuestion.Id, "Create", $"Question {dbQuestion.Name} creation has failed", 3, false);
        }

        private Models.Responses.QuestionChangeResult CreateTestResult(string questionId, string messageType, string reason, int reasonId, bool isSuccess = true)
        {
            return new Models.Responses.QuestionChangeResult()
            {
                Id = questionId,
                MessageType = messageType,
                ActionResult = isSuccess ? "Success" : "Error",
                Reason = reason,
                Sender = "Catalog",
                ReasonId = reasonId,
                ResourceType = "Question"
            };
        }

        private async Task<Models.Requests.IQuestion> GetMessageFromBodyAsync()
        {
            var streamReader = new StreamReader(Request.Body);
            var body = await streamReader.ReadToEndAsync();
            _logger?.LogInformation($"Here is the question that is going to the datebase {body}");
            var newQuestion = JsonConvert.DeserializeObject<Models.Requests.IQuestion>(body);
            return newQuestion ?? throw new Exception("Error when deserialize message body");
        }
    }

}
