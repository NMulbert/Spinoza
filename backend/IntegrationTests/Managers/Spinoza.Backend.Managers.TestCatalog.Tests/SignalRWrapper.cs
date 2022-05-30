using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Spinoza.Backend.Managers.TestCatalog.Tests.Models;
using Spinoza.Backend.Managers.TestCatalog.Tests.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;

namespace Spinoza.Backend.Managers.TestCatalog.Tests
{

    public class SignalRWrapper : ISignalRWrapper
    {
        private readonly HubConnection _signalRHubConnection;
        private readonly List<TestChangeResult?> _signalRMessagesReceived = new List<TestChangeResult?>();
        private readonly SemaphoreSlim _signalRMessageReceived = new SemaphoreSlim(0);

        public SignalRWrapper(ITestOutputHelper testOutputHelper)
        {
            var signalRUrl = Environment.GetEnvironmentVariable("SPINOZA_SIGNALR_URL");
            if (string.IsNullOrEmpty(signalRUrl))
                signalRUrl = "http://localhost:80/api";

            _signalRHubConnection = new HubConnectionBuilder()
                .WithUrl(signalRUrl)
                .WithAutomaticReconnect().ConfigureLogging(lb =>
                {
                    lb.AddProvider(new XunitLoggerProvider(testOutputHelper));
                    lb.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();
        }

        async Task ISignalRWrapper.StartSignalR()
        {
            _signalRMessagesReceived.Clear();

            if (_signalRHubConnection.State == HubConnectionState.Connected)
                return;

            await _signalRHubConnection.StartAsync();

            _signalRHubConnection.On<Data>("SendMessage", result =>
            {
                _signalRMessagesReceived.Add(result.Text);
                _signalRMessageReceived.Release();
            });
        }

        async Task<bool> ISignalRWrapper.WaitForSignalREventAsync(int timeoutInSeconds)
        {
            var isSucceeded = await _signalRMessageReceived.WaitAsync(timeoutInSeconds * 1000);
            await Task.Delay(1000);
            return isSucceeded;
        }

        IList<TestChangeResult?> ISignalRWrapper.Messages => _signalRMessagesReceived;
    }
}
