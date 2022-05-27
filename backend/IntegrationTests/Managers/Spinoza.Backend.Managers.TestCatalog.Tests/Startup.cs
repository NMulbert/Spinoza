using Microsoft.Extensions.DependencyInjection;
using System;

namespace Spinoza.Backend.Managers.TestCatalog.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var testCatalogUrl = Environment.GetEnvironmentVariable("SPINOZA_TEST_CATALOG_URL") ?? "http://localhost:50000/v1.0/invoke/catalogmanager/method";

            services.AddHttpClient("TestCatalogManager", c => c.BaseAddress = new Uri(testCatalogUrl));
            services.AddSingleton<ISignalRWrapperFactory, SignalRWrapperFactory>();
        }
    }
}

