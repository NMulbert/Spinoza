using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spinoza.Backend.Accessor.TestCatalog.DataBases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spinoza.Backend.Accessor.TestCatalog.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICosmosDBWrapper, CosmosDBWrapper>();
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
