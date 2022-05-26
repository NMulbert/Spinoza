using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            services.AddSingleton<ICosmosDbWrapperFactory, CosmosDbWrapperFactory>();
            services.AddSingleton<ICosmosDbInformationProvider>(new CosmosDbInformationProvider("CatalogForTests", "Tests", "testVersion","title"));

            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", false, false);
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var configuration = builder.Build();

            if (configuration == null)
                throw new ArgumentException("Can't get configuration instance");

            services.AddSingleton<IConfiguration>(_ => configuration);

        }
    }
}
