using Microsoft.Azure.Cosmos;

namespace Spinoza.Backend.Accessor.TestCatalog.DataBases
{
    public interface ICosmosDBWrapper
    {
        CosmosClient CosmosClient { get; }
        Database Database { get; }
        Container Container { get; }

        Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<IList<TOut>> GetCosmosElementsAsync<TOut>(QueryDefinition queryDefinition);
        Task<IList<TOut>> GetAllCosmosElementsAsync<TOut>(int skip = 0, int count = 50);
    }
}