using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace Spinoza.Backend.Managers.TestCatalog.Tests
{
    public class Startup
    {
        private readonly Random _jitterer = new Random();

        public void ConfigureServices(IServiceCollection services)
        {
            var testCatalogUrl = Environment.GetEnvironmentVariable("SPINOZA_TEST_CATALOG_URL");
            if (string.IsNullOrEmpty(testCatalogUrl))
              testCatalogUrl = "http://localhost:50000/v1.0/invoke/catalogmanager/method/";

            services.AddHttpClient("TestCatalogManager", c => c.BaseAddress = new Uri(testCatalogUrl))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy()); 
            services.AddSingleton<ISignalRWrapperFactory, SignalRWrapperFactory>();
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(6, // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(_jitterer.Next(0, 100)));
        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(5));
        }
    }
}

