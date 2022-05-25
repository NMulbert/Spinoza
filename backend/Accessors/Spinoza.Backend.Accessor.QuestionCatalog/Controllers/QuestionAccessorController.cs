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

        [HttpGet("allquestions")]
        public async Task<IActionResult> QueryAllQuestionsAsync(int? offset, int? limit)
        {

            var sqlQueryText = new QueryDefinition("SELECT * FROM c OFFSET @offset LIMIT @limit").WithParameter("@offset", offset).WithParameter("@limit", limit);

            JsonArray allQuestions = new JsonArray();


            await foreach (JsonNode? item in _cosmosDBWrapper.EnumerateItemsAsJsonAsync(sqlQueryText))
            {
                if (item == null)
                {
                    _logger.LogWarning("QueryAllQuestionsAsync: Skipping null item.");
                    continue;
                }

                RemoveDBRelatedProperties(item);

                _logger.LogInformation("Question with id: {0} has been added to the array.", item["id"]);


                allQuestions.Add(item);

            }

            return new OkObjectResult(allQuestions);
        }

        private static void RemoveDBRelatedProperties(JsonNode item)
        {
            item.AsObject().Remove("_rid");
            item.AsObject().Remove("_etag");
            item.AsObject().Remove("_self");
            item.AsObject().Remove("_attachments");
            item.AsObject().Remove("_ts");
        }

        [HttpGet("/testquestions")]
        public async Task<IActionResult> GetTestQuestions([FromQuery(Name = "questionsids")] string allTestQuestionsIds)
        {
            try
            {
                char[] separators = { '[', ']', ',' };

                string[] ids = allTestQuestionsIds.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                string strQuery = $"SELECT * FROM ITEMS item WHERE item.id IN ({string.Join(",", ids)})";

                JsonArray allTestQuestions = new JsonArray();

                await foreach (JsonNode? item in _cosmosDBWrapper.EnumerateItemsAsJsonAsync(strQuery))
                {
                    if (item == null)
                    {
                        _logger.LogWarning("GetTestQuestions: Skipping null item.");
                        continue;
                    }
                    _logger.LogInformation("Question with id: {0} has been added to the array.", item["id"]);

                    RemoveDBRelatedProperties(item);

                    allTestQuestions.Add(item);
                }
                return new OkObjectResult(allTestQuestions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting test questions: {allTestQuestionsIds} Error: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        [HttpGet("/question/{id:Guid}")]

        public async Task<IActionResult> GetQuestionById(Guid id)
        {
            try
            {
                var query = new QueryDefinition("SELECT * FROM ITEMS item WHERE item.id = @id").WithParameter("@id", id.ToString().ToUpper());

                var dbQuestion = await _cosmosDBWrapper.EnumerateItemsAsJsonAsync(query).FirstOrDefaultAsync();

                if (dbQuestion == null)
                {
                    _logger.LogWarning($"GetQuestionById: accessor returnes null for question: {id}");
                    return new NotFoundObjectResult(id);
                }

                RemoveDBRelatedProperties(dbQuestion);

                return new OkObjectResult(dbQuestion);
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
            try
            {
                Models.Responses.QuestionChangeResult? result = null;

                if ((string)question["messageType"]! == "AddQuestion")
                {
                    result = await CreateQuestionAsync(question);
                }
                else if ((string)question["messageType"]! == "UpdateQuestion")
                {
                    result = await UpdateQuestionsAsync(question);
                }
                else
                {
                    result = CreateQuestionResult((string)question["id"]!, "UnknownRequest", "Understand only CreateQuestion or UpdateQuestion", 1, false);
                }

                _logger.LogInformation("QuestionChangeResult reason: {0}", result.Reason);

                await PublishQuestionResultAsync(result);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creatring or updating a question: {ex.Message}");
                var result = CreateQuestionResult((string)question["id"] ?? Guid.Empty.ToString(), "Unknown", $"InternalServerError: {ex.Message}", 666, false);
                await PublishQuestionResultAsync(result);

            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);
        }

        public async Task<Models.Responses.QuestionChangeResult?> UpdateQuestionsAsync(JsonNode question)
        {
            question["lastUpdateCreationTimeUTC"] = DateTimeOffset.UtcNow;
            question.AsObject().Remove("messageType");
            var result = await _cosmosDBWrapper.UpdateItemAsync<JsonNode>(question, item => (string?)item["_etag"], item => Guid.Parse(item["id"]!.ToString()), TestMerger);
            if (result != null && result.StatusCode.IsOk())
            {
                return CreateQuestionResult((string)question["id"]!, "Update", $"Question: {question["name"]} has been updated", 8);
            }
            //else
            return CreateQuestionResult((string)question["id"]!, "Update", $"Question {question["name"]} Update has failed", 9, false);

            JsonNode TestMerger(JsonNode dbItem, JsonNode newItem)
            {
                return newItem;
            }
        }

        private async Task<Models.Responses.QuestionChangeResult> CreateQuestionAsync(JsonNode question)
        {
            question["creationTimeUTC"] = DateTimeOffset.UtcNow;
            question["lastUpdateCreationTimeUTC"] = DateTimeOffset.UtcNow;
            question["previousVersionId"] = "none";
            question.AsObject().Remove("messageType");
            var response = await _cosmosDBWrapper.CreateItemAsync(question);

            if (response.StatusCode.IsOk())
            {
                return CreateQuestionResult((string)question["id"]!, "Create", $"Question: {question["name"]} has been created", 2);
            }


            return CreateQuestionResult((string)question["id"]!, "Create", $"Question {question["name"]} creation has been failed", 3, false);

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

        private async Task PublishQuestionResultAsync(Models.Responses.QuestionChangeResult questionChangeResult)
        {
            try
            {
                await _daprClient.PublishEventAsync("pubsub", "question-topic", questionChangeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"InputFromQueueBinding: error getting question from queue: {ex.Message}");
            }
        }

        [HttpGet("/questions/count")]
        public async Task<IActionResult> GetTotalQuestionCount()
        {
            try
            {
                int? dbTotalQuestions = await _cosmosDBWrapper.GetScalarCosmosQueryResult<int?>(new QueryDefinition("SELECT VALUE COUNT(1) FROM c"));
                if (dbTotalQuestions == null)
                {
                    return new NotFoundObjectResult("No Questions found");
                }
                //else
                return new OkObjectResult(dbTotalQuestions);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting questions: {ex.Message}");
            }
            return Problem(statusCode: (int)StatusCodes.Status500InternalServerError);

        }

    }

}
