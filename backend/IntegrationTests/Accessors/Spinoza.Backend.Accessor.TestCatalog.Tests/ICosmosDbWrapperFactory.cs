using Spinoza.Backend.Crosscutting.CosmosDBWrapper;

namespace Spinoza.Backend.Accessor.TestCatalog.Tests
{
    public interface ICosmosDbWrapperFactory
    {
        ICosmosDBWrapper CreateCosmosDBWrapper();
    }
}