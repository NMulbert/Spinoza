using Microsoft.Azure.Cosmos;

namespace Spinoza.Backend.Crosscutting.CosmosDBWrapper;

public interface ICosmosDBWrapper
{
    CosmosClient CosmosClient { get; }
    Database Database { get; }
    Container Container { get; }

    Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken));
    Task<IList<TOut>> GetCosmosElementsAsync<TOut>(QueryDefinition queryDefinition);
    Task<IList<TOut>> GetAllCosmosElementsAsync<TOut>(int skip = 0, int count = 50);
    /// <summary>
    /// This function updates item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newItem"></param>
    /// <param name="eTagSelector"></param>
    /// <param name="idSelector"></param>
    /// <param name="merger"></param>
    /// <param name="createIfNotExist"></param>
    /// <param name="partitionKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ItemResponse<T>?> UpdateItemAsync<T>(T newItem, Func<T, string?> eTagSelector, Func<T, Guid> idSelector, Func<T, T, T> merger, bool createIfNotExist = true, PartitionKey? partitionKey = null, CancellationToken cancellationToken = default(CancellationToken));
}