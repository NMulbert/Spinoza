using System;
using System.Net.Http;
using Xunit;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Spinoza.Backend.Managers.TestCatalog.Tests.Models;
using System.Linq;

namespace Spinoza.Backend.Managers.TestCatalog.Tests
{


    public class TestTests
    {
        private readonly HttpClient _httpClient;
        private readonly ISignalRWrapper _signalR;
        private readonly ITestOutputHelper _testOutputHelper;

        public TestTests(IHttpClientFactory httpClientFactory, ISignalRWrapperFactory signalRWrapperFactory, ITestOutputHelper testOutputHelper)
        {
            _signalR = signalRWrapperFactory.Create(testOutputHelper);
            _testOutputHelper = testOutputHelper;
            _httpClient = httpClientFactory.CreateClient("TestCatalogManager");

           
        }

        [Fact]
        public async Task TestCreateTest()
        {
            var test = new Test
            {
                AuthorId = "test@test.com",
                Description = "This is the description",
                Id = Guid.NewGuid().ToString().ToUpper(),
                QuestionsRefs = new[] { Guid.NewGuid().ToString().ToUpper(), Guid.NewGuid().ToString().ToUpper() },
                SchemaVersion = "1.0",
                Tags = new[] { "tag1", "tag2", "tag3" },
                TestVersion = "1.0",
                Title = $"Test1 Title - {DateTimeOffset.UtcNow}"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(test, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await _signalR.StartSignalR();
            var response = await _httpClient.PostAsync("test", data);
            Assert.NotNull(response);
            Assert.Equal(200, (int)response.StatusCode);

            var result = await _signalR.WaitForSignalREventAsync();

            if (result)
            {
                Assert.NotEmpty(_signalR.Messages);
                Assert.Contains("Create", _signalR.Messages.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalR.Messages.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }
            

            var testResponse = await _httpClient.GetAsync($"test/{test.Id.ToUpper()}");
            Assert.NotNull(testResponse);
            Assert.Equal(200, (int)response.StatusCode);
            var responseJson = await testResponse.Content.ReadAsStringAsync();


            var savedTest = JsonSerializer.Deserialize<Test>(responseJson, serializeOptions);
            Assert.NotNull(savedTest);

            Assert.Equal(test.AuthorId, savedTest!.AuthorId);
            Assert.Equal(test.Description, savedTest.Description);
            Assert.Equal(test.Id, savedTest.Id);
            Assert.Equal(test.Title, savedTest.Title);
            Assert.Equal(test.Tags.Length, savedTest.Tags.Length);
            Assert.Equal(test.QuestionsRefs.Length, savedTest.QuestionsRefs.Length);

        }

        [Fact]
        public async Task TestDeleteTest()
        {
            var test = new Test
            {
                AuthorId = "test@test.com",
                Description = "This is the description",
                Id = Guid.NewGuid().ToString().ToUpper(),
                QuestionsRefs = new[] { Guid.NewGuid().ToString().ToUpper(), Guid.NewGuid().ToString().ToUpper() },
                SchemaVersion = "1.0",
                Tags = new[] { "tag1", "tag2", "tag3" },
                TestVersion = "1.0",
                Title = $"Test1 Title - {DateTimeOffset.UtcNow}"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(test, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await _signalR.StartSignalR();
            var response = await _httpClient.PostAsync("test", data);
            Assert.NotNull(response);
            Assert.Equal(200, (int)response.StatusCode);

            var result = await _signalR.WaitForSignalREventAsync();

            if (result)
            {
                Assert.NotEmpty(_signalR.Messages);
                Assert.Contains("Create", _signalR.Messages.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalR.Messages.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }

            await _signalR.StartSignalR();
            var testResponse = await _httpClient.DeleteAsync($"test/{test.Id.ToUpper()}");
            Assert.NotNull(testResponse);
            Assert.Equal(200, (int)response.StatusCode);

            result = await _signalR.WaitForSignalREventAsync();

            if (result)
            {
                Assert.NotEmpty(_signalR.Messages);
                Assert.Contains("Deleted", _signalR.Messages.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalR.Messages.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }

        }


        [Fact]
        public async Task TestTitleTooLong()
        {
            var test = new Test
            {
                AuthorId = "test@test.com",
                Description = "This is the description",
                Id = Guid.NewGuid().ToString().ToUpper(),
                QuestionsRefs = new[] { Guid.NewGuid().ToString().ToUpper(), Guid.NewGuid().ToString().ToUpper() },
                SchemaVersion = "1.0",
                Tags = new[] { "tag1", "tag2", "tag3" },
                TestVersion = "1.0",
                Title = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean m1"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(test, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("test", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe title is  bigger than 100 characters", result);
        }


        [Fact]
        public async Task TestTitleTooShort()
        {
            var test = new Test
            {
                AuthorId = "test@test.com",
                Description = "This is the description",
                Id = Guid.NewGuid().ToString().ToUpper(),
                QuestionsRefs = new[] { Guid.NewGuid().ToString().ToUpper(), Guid.NewGuid().ToString().ToUpper() },
                SchemaVersion = "1.0",
                Tags = new[] { "tag1", "tag2", "tag3" },
                TestVersion = "1.0",
                Title = "ab"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(test, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("test", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe title is less than 3 characters", result);
        }


        [Fact]
        public async Task TestMissingTestTitle()
        {
            var test = new Test
            {
                AuthorId = "test@test.com",
                Description = "This is the description",
                Id = Guid.NewGuid().ToString().ToUpper(),
                QuestionsRefs = new[] { Guid.NewGuid().ToString().ToUpper(), Guid.NewGuid().ToString().ToUpper() },
                SchemaVersion = "1.0",
                Tags = new[] { "tag1", "tag2", "tag3" },
                TestVersion = "1.0",
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(test, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("test", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe Title is missing", result);
        }


        [Fact]
        public async Task TestMissingAuthorId()
        {
            var test = new Test
            {
                Description = "This is the description",
                Id = Guid.NewGuid().ToString().ToUpper(),
                QuestionsRefs = new[] { Guid.NewGuid().ToString().ToUpper(), Guid.NewGuid().ToString().ToUpper() },
                SchemaVersion = "1.0",
                Tags = new[] { "tag1", "tag2", "tag3" },
                TestVersion = "1.0",
                Title = $"Test1 Title - {DateTimeOffset.UtcNow}"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(test, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("test", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe Author Id is missing", result);
        }

        [Fact]
        public async Task TestMissingTestId()
        {
            var test = new Test
            {
                AuthorId = "test@test.com",
                Description = "This is the description",
                QuestionsRefs = new[] { Guid.NewGuid().ToString().ToUpper(), Guid.NewGuid().ToString().ToUpper() },
                SchemaVersion = "1.0",
                Tags = new[] { "tag1", "tag2", "tag3" },
                TestVersion = "1.0",
                Title = $"Test1 Title - {DateTimeOffset.UtcNow}"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(test, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("test", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nthe Id is missing", result);
        }

        [Fact]
        public async Task TestCreateOpenTextQuestion()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "test@test.com",
                Content = $"This is an open text question - {DateTimeOffset.UtcNow}",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion",
                Status = "Draft"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await _signalR.StartSignalR();
            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(200, (int)response.StatusCode);

            var result = await _signalR.WaitForSignalREventAsync();

            if (result)
            {
                Assert.NotEmpty(_signalR.Messages);
                Assert.Contains("Create", _signalR.Messages.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalR.Messages.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }

            var testResponse = await _httpClient.GetAsync($"question/{openTextQuestion.Id.ToUpper()}");
            Assert.NotNull(testResponse);
            Assert.Equal(200, (int)response.StatusCode);
            var responseJson = await testResponse.Content.ReadAsStringAsync();


            var savedQuestion = JsonSerializer.Deserialize<OpenTextQuestion>(responseJson, serializeOptions);
            Assert.NotNull(savedQuestion);

            Assert.Equal(openTextQuestion.Name, savedQuestion!.Name);
            Assert.Equal(openTextQuestion.DifficultyLevel, savedQuestion.DifficultyLevel);
            Assert.Equal(openTextQuestion.Id, savedQuestion.Id);
            Assert.Equal(openTextQuestion.Type, savedQuestion.Type);
            Assert.Equal(openTextQuestion.Tags.Length, savedQuestion.Tags.Length);
            Assert.Equal(openTextQuestion.AuthorId, savedQuestion.AuthorId);
            Assert.Equal(openTextQuestion.Content, savedQuestion.Content);
        }


        [Fact]
        public async Task TestCreateMultipleChoiceQuestion()
        {
            var openTextQuestion = new MultipleChoiceQuestion
            {
                AuthorId = "test@test.com",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "MultipleChoiceQuestion",
                Content = new Content
                {
                    QuestionText = $"This is a question - {DateTimeOffset.UtcNow}",
                    AnswerOptions = new []
                    {
                        new AnswerOption
                        {
                            Description = "Option 1",
                            IsCorrect = false
                        },
                        new AnswerOption
                        {
                            Description = "Option 2",
                            IsCorrect = true
                        },
                        new AnswerOption
                        {
                            Description = "Option 3",
                            IsCorrect = false
                        }
                    }
                }
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await _signalR.StartSignalR();
            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(200, (int)response.StatusCode);

            var result = await _signalR.WaitForSignalREventAsync();
            if (result)
            {
                Assert.NotEmpty(_signalR.Messages);
                Assert.Contains("Create", _signalR.Messages.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalR.Messages.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }

            var testResponse = await _httpClient.GetAsync($"question/{openTextQuestion.Id.ToUpper()}");
            Assert.NotNull(testResponse);
            Assert.Equal(200, (int)response.StatusCode);
            var responseJson = await testResponse.Content.ReadAsStringAsync();


            var savedQuestion = JsonSerializer.Deserialize<MultipleChoiceQuestion>(responseJson, serializeOptions);
            Assert.NotNull(savedQuestion);

            Assert.Equal(openTextQuestion.Name, savedQuestion!.Name);
            Assert.Equal(openTextQuestion.DifficultyLevel, savedQuestion.DifficultyLevel);
            Assert.Equal(openTextQuestion.Id, savedQuestion.Id);
            Assert.Equal(openTextQuestion.Type, savedQuestion.Type);
            Assert.Equal(openTextQuestion.Tags.Length, savedQuestion.Tags.Length);
            Assert.Equal(openTextQuestion.AuthorId, savedQuestion.AuthorId);
            Assert.Equal(openTextQuestion.Content.QuestionText, savedQuestion.Content.QuestionText);
            Assert.True(savedQuestion.Content.AnswerOptions[1].IsCorrect);
            Assert.Equal(3, savedQuestion.Content.AnswerOptions.Length);
        }

        [Fact]
        public async Task TestDeleteOpenTextQuestion()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "test@test.com",
                Content = $"This is an open text question - {DateTimeOffset.UtcNow}",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion",
                Status="Draft"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await _signalR.StartSignalR();
            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(200, (int)response.StatusCode);

            var result = await _signalR.WaitForSignalREventAsync();

            if (result)
            {
                Assert.NotEmpty(_signalR.Messages);
                Assert.Contains("Create", _signalR.Messages.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalR.Messages.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }

            await _signalR.StartSignalR();
            var testResponse = await _httpClient.DeleteAsync($"question/{openTextQuestion.Id.ToUpper()}");
            Assert.NotNull(testResponse);
            Assert.Equal(200, (int)response.StatusCode);
            result = await _signalR.WaitForSignalREventAsync();

            if (result)
            {
                Assert.NotEmpty(_signalR.Messages);
                Assert.Contains("Deleted", _signalR.Messages.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalR.Messages.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }
        }

        [Fact]
        public async Task TestMissingOpenTextQuestionId()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "question@question.com",
                Content = $"This is an open text question - {DateTimeOffset.UtcNow}",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nthe Id is missing", result);
        }

        [Fact]
        public async Task TestMissingOpenTextAuthorId()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                Content = $"This is an open text question - {DateTimeOffset.UtcNow}",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe Author Id is missing", result);
        }


        [Fact]
        public async Task TestMissingOpenTextContent()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "question@question.com",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nPlease write your question content", result);
        }

        [Fact]
        public async Task TestMissingOpenTextMissingDifficultyLevel()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "question@question.com",
                Content = $"This is an open text question - {DateTimeOffset.UtcNow}",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nPlease enter question difficulty", result);
        }

        [Fact]
        public async Task TestMissingOpenTextDifficultyLevelOutOfRange()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "question@question.com",
                Content = $"This is an open text question - {DateTimeOffset.UtcNow}",
                DifficultyLevel = "6",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nPlease enter question difficulty btewwen 1-5", result);
        }

        [Fact]
        public async Task TestOpenTextMissingQuestionName()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "question@question.com",
                Content = $"This is an open text question - {DateTimeOffset.UtcNow}",
                DifficultyLevel = "5",
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe question name is missing", result);
        }
        [Fact]
        public async Task TestOpenTextToShortQuestionName()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "question@question.com",
                Content = $"This is an open text question - {DateTimeOffset.UtcNow}",
                DifficultyLevel = "5",
                Name = "Q",
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe title is less than 3 characters", result);
        }

        [Fact]
        public async Task TestOpenTextToLongQuestionName()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "question@question.com",
                Content = $"This is an open text question - {DateTimeOffset.UtcNow}",
                DifficultyLevel = "5",
                Name = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghij",
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "OpenTextQuestion"
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe title is  bigger than 100 characters", result);
        }

        [Fact]
        public async Task TestMissingMultipleChoiceQuestionAuthorId()
        {
            var openTextQuestion = new MultipleChoiceQuestion
            {
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "MultipleChoiceQuestion",
                Content = new Content
                {
                    QuestionText = $"This is a question - {DateTimeOffset.UtcNow}",
                    AnswerOptions = new[]
                    {
                        new AnswerOption
                        {
                            Description = "Option 1",
                            IsCorrect = false
                        },
                        new AnswerOption
                        {
                            Description = "Option 2",
                            IsCorrect = true
                        },
                        new AnswerOption
                        {
                            Description = "Option 3",
                            IsCorrect = false
                        }
                    }
                }
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe Author Id is missing", result);
        }

        [Fact]
        public async Task TestMissingMultipleChoiceQuestionId()
        {
            var openTextQuestion = new MultipleChoiceQuestion
            {
                AuthorId = "question@question.com",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "MultipleChoiceQuestion",
                Content = new Content
                {
                    QuestionText = $"This is a question - {DateTimeOffset.UtcNow}",
                    AnswerOptions = new[]
                    {
                        new AnswerOption
                        {
                            Description = "Option 1",
                            IsCorrect = false
                        },
                        new AnswerOption
                        {
                            Description = "Option 2",
                            IsCorrect = true
                        },
                        new AnswerOption
                        {
                            Description = "Option 3",
                            IsCorrect = false
                        }
                    }
                }
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nthe Id is missing", result);
        }

        [Fact]
        public async Task TestMissingMultipleChoiceQuestionQuestionText()
        {
            var openTextQuestion = new MultipleChoiceQuestion
            {
                AuthorId = "question@question.com",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "MultipleChoiceQuestion",
                Content = new Content
                {
                    AnswerOptions = new[]
                    {
                        new AnswerOption
                        {
                            Description = "Option 1",
                            IsCorrect = false
                        },
                        new AnswerOption
                        {
                            Description = "Option 2",
                            IsCorrect = true
                        },
                        new AnswerOption
                        {
                            Description = "Option 3",
                            IsCorrect = false
                        }
                    }
                }
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nPlease write your question", result);
        }

        [Fact]
        public async Task TestMultipleChoiceQuestionLessThanTwoQuestionsOptions()
        {
            var openTextQuestion = new MultipleChoiceQuestion
            {
                AuthorId = "question@question.com",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "MultipleChoiceQuestion",
                Content = new Content
                {
                    QuestionText = $"This is a question - {DateTimeOffset.UtcNow}",
                    AnswerOptions = new[]
                    {
                        new AnswerOption
                        {
                            Description = "Option 1",
                            IsCorrect = false
                        }
                    }
                }
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nPlease enter more than one option answer", result);
        }

        [Fact]
        public async Task TestMultipleChoiceQuestionAtLeastOneTrueAnswer()
        {
            var openTextQuestion = new MultipleChoiceQuestion
            {
                AuthorId = "question@question.com",
                DifficultyLevel = "5",
                Name = "Q" + DateTimeOffset.UtcNow,
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "MultipleChoiceQuestion",
                Content = new Content
                {
                    QuestionText = $"This is a question - {DateTimeOffset.UtcNow}",
                    AnswerOptions = new[]
                    {
                        new AnswerOption
                        {
                            Description = "Option 1",
                            IsCorrect = false
                        },
                        new AnswerOption
                        {
                            Description = "Option 2",
                            IsCorrect = false
                        },
                        new AnswerOption
                        {
                            Description = "Option 3",
                            IsCorrect = false
                        }
                    }
                }
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nthere is no correct answers please correct you multipalcechoise question", result);
        }

        [Fact]
        public async Task TestMissingMultipleChoiceQuestionName()
        {
            var openTextQuestion = new MultipleChoiceQuestion
            {
                AuthorId = "question@question.com",
                DifficultyLevel = "5",
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "MultipleChoiceQuestion",
                Content = new Content
                {
                    QuestionText = $"This is a question - {DateTimeOffset.UtcNow}",
                    AnswerOptions = new[]
                    {
                        new AnswerOption
                        {
                            Description = "Option 1",
                            IsCorrect = true
                        },
                        new AnswerOption
                        {
                            Description = "Option 2",
                            IsCorrect = false
                        },
                        new AnswerOption
                        {
                            Description = "Option 3",
                            IsCorrect = false
                        }
                    }
                }
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe question name is missing", result);
        }

        [Fact]
        public async Task TestMultipleChoiceQuestionToShortName()
        {
            var openTextQuestion = new MultipleChoiceQuestion
            {
                AuthorId = "question@question.com",
                DifficultyLevel = "5",
                Name = "Q",
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "MultipleChoiceQuestion",
                Content = new Content
                {
                    QuestionText = $"This is a question - {DateTimeOffset.UtcNow}",
                    AnswerOptions = new[]
                    {
                        new AnswerOption
                        {
                            Description = "Option 1",
                            IsCorrect = true
                        },
                        new AnswerOption
                        {
                            Description = "Option 2",
                            IsCorrect = false
                        },
                        new AnswerOption
                        {
                            Description = "Option 3",
                            IsCorrect = false
                        }
                    }
                }
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe title is less than 3 characters", result);
        }

        [Fact]
        public async Task TestMultipleChoiceQuestionToLongName()
        {
            var openTextQuestion = new MultipleChoiceQuestion
            {
                AuthorId = "question@question.com",
                DifficultyLevel = "5",
                Name = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcd",
                PreviousVersionId = "F317129A-FFFF-4772-3344-A166C5808998",
                QuestionVersion = "1.0",
                Id = Guid.NewGuid().ToString().ToUpper(),
                SchemaVersion = "1.0",
                Tags = new[] { "tag4", "tag5", "tag6" },
                Type = "MultipleChoiceQuestion",
                Content = new Content
                {
                    QuestionText = $"This is a question - {DateTimeOffset.UtcNow}",
                    AnswerOptions = new[]
                    {
                        new AnswerOption
                        {
                            Description = "Option 1",
                            IsCorrect = true
                        },
                        new AnswerOption
                        {
                            Description = "Option 2",
                            IsCorrect = false
                        },
                        new AnswerOption
                        {
                            Description = "Option 3",
                            IsCorrect = false
                        }
                    }
                }
            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(openTextQuestion, serializeOptions);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("question", data);
            Assert.NotNull(response);
            Assert.Equal(400, (int)response.StatusCode);
            string result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Errors: \nThe title is  bigger than 100 characters", result);
        }
    }
}
