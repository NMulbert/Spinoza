using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;
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

        public QuestionAccessorController(ILogger<QuestionAccessorController> logger, DaprClient daprClient, ICosmosDBWrapper cosmosDBWrapper)
        {
            _logger = logger;
            _daprClient = daprClient;
            _cosmosDBWrapper = cosmosDBWrapper;
        }

        [HttpGet("allquestions")]
        public async Task<IActionResult> QueryAllQuestionsAsync(int? offset, int? limit)
        {
            var tags = Request.Query["tags"].ToString();
            _logger.LogInformation(tags);
            try
            {
                JsonArray allQuestions = new JsonArray();

                if (string.IsNullOrEmpty(tags))
                {
                    _logger.LogInformation("tags are null");

                    var sqlQueryDefinition = new QueryDefinition("SELECT * FROM c OFFSET @offset LIMIT @limit")
                        .WithParameter("@offset", offset).WithParameter("@limit", limit);

                    return await GetAllQuestionsFromDBAsync(sqlQueryDefinition, item => item);

                }
                //else
                _logger.LogInformation("tags are not null");

                var sqlQueryDefinitionTags = new QueryDefinition(
                    $" SELECT DISTINCT question FROM question JOIN tag IN question.tags WHERE tag IN ({tags}) OFFSET @offset LIMIT @limit")
                    .WithParameter("@offset", offset).WithParameter("@limit", limit);

                return await GetAllQuestionsFromDBAsync(sqlQueryDefinitionTags, item =>
                {
                    var root = item.AsObject();
                    var question = item["question"];
                    root.Remove("question");

                    return question;
                });

                async Task<IActionResult> GetAllQuestionsFromDBAsync(QueryDefinition query, Func<JsonNode, JsonNode?> extractor)
                {
                    _logger.LogInformation($"GetAllQuestions query: {query.QueryText}");

                    await foreach (JsonNode? item in _cosmosDBWrapper.EnumerateItemsAsJsonAsync(query))
                    {
                        if (item == null)
                        {
                            _logger.LogWarning("QueryAllQuestionsAsync: Skipping null item.");
                            continue;
                        }

                        var question = extractor(item);

                        if (question == null)
                        {
                            _logger.LogWarning("GetAllQuestionsFromDBAsync: null question detected.");
                            continue;
                        }

                        RemoveDBRelatedProperties(question);

                        _logger.LogInformation("Question with id: {0} has been added to the array.", question["id"]);

                        allQuestions.Add(question);

                    }

                    return new OkObjectResult(allQuestions);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting questions: {ex.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
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
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
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
                    _logger.LogWarning($"GetQuestionById: accessor returns null for question: {id}");
                    return new NotFoundObjectResult(id);
                }

                RemoveDBRelatedProperties(dbQuestion);

                return new OkObjectResult(dbQuestion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting a question: {id} Error: {ex.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        [HttpPost("/questionaccessorrequestqueue")]

        public async Task<IActionResult> InputFromQueueBinding([FromBody] JsonNode question)
        {
            try
            {
                Models.Responses.QuestionChangeResult? result;

                if ((string)question["messageType"]! == "AddQuestion")
                {
                    result = await CreateQuestionAsync(question);
                }
                else if ((string)question["messageType"]! == "UpdateQuestion")
                {
                    result = await UpdateQuestionsAsync(question);
                }
                else if ((string)question["messageType"]! == "DeleteQuestion")
                {
                    result = await DeleteQuestionAsync(question);
                }
                else
                {
                    result = CreateQuestionResult((string)question["id"]!, "UnknownRequest", "Understand only CreateQuestion or UpdateQuestion", 1, false);
                }

                _logger.LogInformation("QuestionChangeResult reason: {0}", result?.Reason);

                if (result != null)
                {
                    await PublishQuestionResultAsync(result);
                }
                else
                {
                    throw new Exception(
                        "QuestionAccessorController:InputFromQueueBinding result of question creation or update is null");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                string questionId = "Unknown Id";
                try
                {
                    questionId = (string)question["id"]!;
                }
                catch (Exception)
                {
                    _logger.LogError($"EQuestionAccessorController:InputFromQueueBinding, can't get question id");
                }
                _logger.LogError($"Error while creating or updating a question: {ex.Message}");
                var result = CreateQuestionResult(questionId, "Unknown", $"InternalServerError: {ex.Message}", 666, false);
                await PublishQuestionResultAsync(result);
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        public async Task<Models.Responses.QuestionChangeResult?> UpdateQuestionsAsync(JsonNode question)
        {
            question["lastUpdateCreationTimeUTC"] = DateTimeOffset.UtcNow;
            question.AsObject().Remove("messageType");
            var result = await _cosmosDBWrapper.UpdateItemAsync(question, item => (string?)item["_etag"], item => Guid.Parse(item["id"]!.ToString()), TestMerger);
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

        public async Task<Models.Responses.QuestionChangeResult> DeleteQuestionAsync(JsonNode? questionDeleteInfo)
        {
            var response = new Models.Responses.QuestionChangeResult()
            {
                Id = questionDeleteInfo?["id"]?.GetValue<string>() ?? "Unknown",
                MessageType = "Deleted",
                ActionResult = "Error",
                Reason = "Delete question",
                Sender = "Catalog",
                ReasonId = 13,
                ResourceType = "Question"
            };

            var questionVersion = questionDeleteInfo?["QuestionVersion"]?.GetValue<string>();

            if (questionDeleteInfo == null || questionVersion == null)
            {
                return response;
            }

            try
            {
                bool result = await _cosmosDBWrapper.DeleteItemAsync(response.Id, questionVersion);
                if (result == false)
                {
                    return response;
                }
                response.ActionResult = "Success";
                //else
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting a question: {ex.Message}");

            }
            response.ActionResult = "Error";
            return response;
        }


        [HttpGet("/questions/count")]
        public async Task<IActionResult> GetTotalQuestionCount()
        {
            var tags = Request.Query["tags"].ToString();
            _logger.LogInformation($"the tags are: {tags}");

            try
            {

                if (string.IsNullOrEmpty(tags))
                {
                    var dbTotalQuestionsCount =
                        await _cosmosDBWrapper.GetScalarCosmosQueryResult<int>(
                            new QueryDefinition($"SELECT VALUE COUNT(1) FROM c"));
                    return new OkObjectResult(dbTotalQuestionsCount);
                }
                //else
                var totalQuestionsByTags = await _cosmosDBWrapper.GetScalarCosmosQueryResult<JsonNode>(new QueryDefinition
                    ($"SELECT DISTINCT COUNT(1) question FROM question JOIN tag IN question.tags WHERE tag IN ({tags})")
                );
                if (totalQuestionsByTags == null)
                {
                    _logger.LogWarning("GetTotalQuestionCount : No Questions found");
                    return new NotFoundObjectResult("No Questions found");
                }
                //else
                _logger.LogInformation(totalQuestionsByTags.ToJsonString());
                int count = totalQuestionsByTags["question"]!.GetValue<int>();
                return new OkObjectResult(count);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting questions: {ex.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);

        }

    }

}
