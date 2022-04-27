using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;

namespace Spinoza.Backend.Accessor.TestCatalog.Tests
{
    class CosmosDbWrapperFactory : ICosmosDbWrapperFactory
    {
        public CosmosDbWrapperFactory(IConfiguration configuration, ILogger<CosmosDBWrapper> logger, ICosmosDbInformationProvider cosmosDbInformationProvider)
        {
            Configuration = configuration;
            Logger = logger;
            CosmosDbInformationProvider = cosmosDbInformationProvider;
        }

        public IConfiguration Configuration { get; }
        public ILogger<CosmosDBWrapper> Logger { get; }
        public ICosmosDbInformationProvider CosmosDbInformationProvider { get; }

        public ICosmosDBWrapper CreateCosmosDBWrapper() => new CosmosDBWrapper(Configuration,Logger,CosmosDbInformationProvider);
       
    }
}