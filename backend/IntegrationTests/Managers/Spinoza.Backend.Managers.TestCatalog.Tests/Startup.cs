using Microsoft.Extensions.DependencyInjection;
using System;

namespace Spinoza.Backend.Managers.TestCatalog.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("TestCatalogManager", c => c.BaseAddress = new Uri("http://localhost:50000"));

        }
    }
}

