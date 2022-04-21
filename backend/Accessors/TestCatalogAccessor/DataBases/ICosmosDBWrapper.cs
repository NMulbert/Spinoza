using Microsoft.Azure.Cosmos;

namespace Spinoza.Backend.Accessor.TestCatalog.DataBases
{
    public interface ICosmosDBWrapper
    {
        CosmosClient CosmosClient { get; }
        Database TestDatabase { get; }
        Container TestContainer { get; }

        Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}