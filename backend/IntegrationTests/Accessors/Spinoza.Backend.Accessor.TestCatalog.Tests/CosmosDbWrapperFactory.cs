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

        private IConfiguration Configuration { get; }
        private ILogger<CosmosDBWrapper> Logger { get; }
        private ICosmosDbInformationProvider CosmosDbInformationProvider { get; }

        public ICosmosDBWrapper CreateCosmosDBWrapper() => new CosmosDBWrapper(Configuration,Logger,CosmosDbInformationProvider);
       
    }
}