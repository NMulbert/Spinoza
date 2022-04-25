using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;
using System;
using System.IO;


namespace Spinoza.Backend.Accessor.TestCatalog.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICosmosDBWrapper, CosmosDBWrapper>();
            services.AddSingleton<ICosmosDbInformationProvider>(new CosmosDbInformationProvider("Catalog", "Tests", "Version","Title"));
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", false, false);

            var configuration = builder.Build();

            if (configuration == null)
                throw new ArgumentException("Can't get configuration instance");

            services.AddSingleton<IConfiguration>(_ => configuration);

        }
    }
}
