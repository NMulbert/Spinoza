using AutoMapper;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

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

        [HttpGet("/questions")]
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

        [HttpGet("allquestions")]
        public async Task<IActionResult> QueryAllQuestionsAsync()
        {


            var sqlQueryText = "SELECT * FROM c";


            JsonArray allQuestions = new JsonArray();


            await foreach (JsonNode? item in _cosmosDBWrapper.EnumerateItemsAsJsonAsync(sqlQueryText))
            {
                if (item == null)
                {
                    _logger.LogWarning("QueryAllQuestionsAsync: Skipping null item.");
                    continue;
                }

                item.AsObject().Remove("_rid");
                item.AsObject().Remove("_etag");
                item.AsObject().Remove("_self");
                item.AsObject().Remove("attachments");
                item.AsObject().Remove("_ts");


                _logger.LogInformation("Question with id: {0} has been added to the array.", item["id"]);


                allQuestions.Add(item);

            }

            return new OkObjectResult(allQuestions);
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

        [HttpPost("/questionaccessorrequestqueue")]

        public async Task<IActionResult> InputFromQueueBinding([FromBody] JsonNode question)
        {
            System.Diagnostics.Debugger.Launch();

            try
            {
                var result = await _cosmosDBWrapper.CreateItemAsync(question);
                if (result.StatusCode.IsOk())
                {
                    //return CreateTestResult(dbTest.Id, "Create", $"Test {dbTest.Title} has been created", 2);

                }
                //else
                // return CreateTestResult(dbTest.Id, "Create", $"Test {dbTest.Title} creation has been failed", 3, false);


                //var questionRequest = await GetMessageFromBodyAsync<JsonNode>();
                //Models.Responses.QuestionChangeResult? result = null;

                //switch (questionRequest.MessageType)
                //{
                //    case "CreateQuestion":

                //        result = await CreateQuestionAsync(questionRequest);
                //        await PublishQuestiontResultAsync(result);
                //        return Ok();

                //    case "UpdateQuestion":

                //        // result = await UpdateQuestionAsync(questionRequest);
                //        return Ok();

                //    default:
                //        break;
                //}

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creatring or updating a question: {ex.Message}");

            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        private async Task PublishQuestiontResultAsync(Models.Responses.QuestionChangeResult questionChangeResult)
        {
            try
            {
                await _daprClient.PublishEventAsync("pubsub", "question-topic", questionChangeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetQuestiontFromQueueRequestAsync: error getting question from queue: {ex.Message}");
            }
        }

        private async Task<Models.Responses.QuestionChangeResult> CreateQuestionAsync<TQuestion>(TQuestion questionRequest) where TQuestion : Models.DB.IQuestion
        {

            var dbQuestion = _mapper.Map<TQuestion>(questionRequest);

            var result = await _cosmosDBWrapper.CreateItemAsync(dbQuestion);

            if (result.StatusCode.IsOk())
            {
                return CreateQuestionResult(dbQuestion.Id, "Create", $"Question {dbQuestion.Name} has been created", 2);

            }
            return CreateQuestionResult(dbQuestion.Id, "Create", $"Question {dbQuestion.Name} creation has failed", 3, false);
        }

        private Models.Responses.QuestionChangeResult CreateQuestionResult(string questionId, string messageType, string reason, int reasonId, bool isSuccess = true)
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

        //private async Task<TQuestion> GetMessageFromBodyAsync<TQuestion>() where TQuestion : Models.Requests.IQuestion
        //{
        //    var streamReader = new StreamReader(Request.Body);
        //    var body = await streamReader.ReadToEndAsync();
        //    _logger?.LogInformation($"Here is the question that is going to the datebase {body}");
        //    var newQuestion = JsonConvert.DeserializeObject<Models.Requests.IQuestion>(body);


        //    switch (newQuestion?.Type)
        //    {
        //        case "MultipleChoiceQuestion":

        //            var multipleChoiceQuestion = JsonConvert.DeserializeObject<Models.Requests.MultipleChoiceQuestion>(body);
        //            return multipleChoiceQuestion ?? throw new Exception("Error when deserialize message body");

        //        case "OpenTextQuestion":

        //            var openTextQuestion = JsonConvert.DeserializeObject<Models.Requests.OpenTextQuestion>(body);
        //            return openTextQuestion ?? throw new Exception("Error when deserialize message body");

        //        default:
        //            throw new Exception("Error when deserialize message body");
        //    }




        //}
    }

}
