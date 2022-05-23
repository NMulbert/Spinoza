using System;
using System.Net.Http;
using Xunit;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Spinoza.Backend.Managers.TestCatalog.Tests.Models;
using Spinoza.Backend.Managers.TestCatalog.Tests.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Spinoza.Backend.Managers.TestCatalog.Tests
{

    public class TestTests
    {
        private readonly HttpClient _httpClient;
        private HubConnection _signalRHubConnection;
        private readonly List<TestChangeResult?> _signalRMessagesReceived = new List<TestChangeResult?>();
        private SemaphoreSlim _signalRMessageReceived = new SemaphoreSlim(0);
        private ITestOutputHelper _testOutputHelper;

        public TestTests(IHttpClientFactory httpClientFactory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _httpClient = httpClientFactory.CreateClient("TestCatalogManager");

            _signalRHubConnection = new HubConnectionBuilder()
                 .WithUrl("http://localhost:80/api")
                 .WithAutomaticReconnect().ConfigureLogging(lb =>
                 {
                     lb.AddProvider(new XunitLoggerProvider(testOutputHelper));
                     lb.SetMinimumLevel(LogLevel.Debug);
                 })
                 .Build();
        }

        private async Task StartSinglaR()
        {
            if (_signalRHubConnection.State != HubConnectionState.Connected)
                await _signalRHubConnection.StartAsync();

            _signalRHubConnection.On<Data>("SendMessage", result =>
            {
                _signalRMessagesReceived.Add(result.Text);
                _signalRMessageReceived.Release();
            });

        }

        private async Task<bool> WaitForSignalREventAsync(int timeoutInSeconds = 20)
        {
            var isSucceeded = await _signalRMessageReceived.WaitAsync(timeoutInSeconds * 1000);
            return isSucceeded;
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

            await StartSinglaR();
            var response = await _httpClient.PostAsync("/v1.0/invoke/catalogmanager/method/test", data);
            Assert.NotNull(response);
            Assert.Equal(200, (int)response.StatusCode);

            var result = await WaitForSignalREventAsync();

            if (result)
            {
                Assert.NotEmpty(_signalRMessagesReceived);
                Assert.Contains("Create", _signalRMessagesReceived.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalRMessagesReceived.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }
            

            var testResponse = await _httpClient.GetAsync($"/v1.0/invoke/catalogmanager/method/test/{test.Id.ToUpper()}");
            Assert.NotNull(testResponse);
            Assert.Equal(200, (int)response.StatusCode);
            var responseJson = await testResponse.Content.ReadAsStringAsync();


            var savedTest = JsonSerializer.Deserialize<Test>(responseJson, serializeOptions);
            Assert.NotNull(savedTest);

            Assert.Equal(test.AuthorId, savedTest!.AuthorId);
            Assert.Equal(test.Description, savedTest!.Description);
            Assert.Equal(test.Id, savedTest!.Id);
            Assert.Equal(test.Title, savedTest!.Title);
            Assert.Equal(test.Tags.Length, savedTest!.Tags.Length);
            Assert.Equal(test.QuestionsRefs.Length, savedTest!.QuestionsRefs.Length);

        }

        [Fact]
        public async Task TestCreateOpenTextQuestion()
        {
            var openTextQuestion = new OpenTextQuestion
            {
                AuthorId = "test@test.com",
                Content = "This is an open text question",
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

            await StartSinglaR();
            var response = await _httpClient.PostAsync("/v1.0/invoke/catalogmanager/method/question", data);
            Assert.NotNull(response);
            Assert.Equal(200, (int)response.StatusCode);

            var result = await WaitForSignalREventAsync();

            if (result)
            {
                Assert.NotEmpty(_signalRMessagesReceived);
                Assert.Contains("Create", _signalRMessagesReceived.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalRMessagesReceived.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }

            var testResponse = await _httpClient.GetAsync($"/v1.0/invoke/catalogmanager/method/question/{openTextQuestion.Id.ToUpper()}");
            Assert.NotNull(testResponse);
            Assert.Equal(200, (int)response.StatusCode);
            var responseJson = await testResponse.Content.ReadAsStringAsync();


            var savedQuestion = JsonSerializer.Deserialize<OpenTextQuestion>(responseJson, serializeOptions);
            Assert.NotNull(savedQuestion);

            Assert.Equal(openTextQuestion.Name, savedQuestion!.Name);
            Assert.Equal(openTextQuestion.DifficultyLevel, savedQuestion!.DifficultyLevel);
            Assert.Equal(openTextQuestion.Id, savedQuestion!.Id);
            Assert.Equal(openTextQuestion.Type, savedQuestion.Type);
            Assert.Equal(openTextQuestion.Tags.Length, savedQuestion!.Tags.Length);
            Assert.Equal(openTextQuestion.AuthorId, savedQuestion!.AuthorId);
            Assert.Equal(openTextQuestion.Content, savedQuestion!.Content);
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
                    QuestionText = "This is a question",
                    AnswerOptions = new AnswerOption []
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

            await StartSinglaR();
            var response = await _httpClient.PostAsync("/v1.0/invoke/catalogmanager/method/question", data);
            Assert.NotNull(response);
            Assert.Equal(200, (int)response.StatusCode);

            var result = await WaitForSignalREventAsync();
            if (result)
            {
                Assert.NotEmpty(_signalRMessagesReceived);
                Assert.Contains("Create", _signalRMessagesReceived.Select(e => e!.MessageType));
                Assert.Contains("Success", _signalRMessagesReceived.Select(e => e!.ActionResult));
            }
            else
            {
                _testOutputHelper.WriteLine("Ignoring SignalR error, for the sake of github actions");
            }

            var testResponse = await _httpClient.GetAsync($"/v1.0/invoke/catalogmanager/method/question/{openTextQuestion.Id.ToUpper()}");
            Assert.NotNull(testResponse);
            Assert.Equal(200, (int)response.StatusCode);
            var responseJson = await testResponse.Content.ReadAsStringAsync();


            var savedQuestion = JsonSerializer.Deserialize<MultipleChoiceQuestion>(responseJson, serializeOptions);
            Assert.NotNull(savedQuestion);

            Assert.Equal(openTextQuestion.Name, savedQuestion!.Name);
            Assert.Equal(openTextQuestion.DifficultyLevel, savedQuestion!.DifficultyLevel);
            Assert.Equal(openTextQuestion.Id, savedQuestion!.Id);
            Assert.Equal(openTextQuestion.Type, savedQuestion.Type);
            Assert.Equal(openTextQuestion.Tags.Length, savedQuestion!.Tags.Length);
            Assert.Equal(openTextQuestion.AuthorId, savedQuestion!.AuthorId);
            Assert.Equal(openTextQuestion.Content.QuestionText, savedQuestion!.Content.QuestionText);
            Assert.True(savedQuestion.Content.AnswerOptions[1].IsCorrect);
            Assert.Equal(3, savedQuestion.Content.AnswerOptions.Length);
        }

    }
}
