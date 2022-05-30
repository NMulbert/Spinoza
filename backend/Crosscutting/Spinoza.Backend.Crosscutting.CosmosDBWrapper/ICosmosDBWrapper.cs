using Microsoft.Azure.Cosmos;
using System.Text.Json.Nodes;

namespace Spinoza.Backend.Crosscutting.CosmosDBWrapper;

public interface ICosmosDBWrapper
{
    CosmosClient CosmosClient { get; }
    Database Database { get; }
    Container Container { get; }

    Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken));
    Task<IList<TOut>> GetCosmosElementsAsync<TOut>(QueryDefinition queryDefinition);
    Task<IList<TOut>> GetAllCosmosElementsAsync<TOut>(int skip = 0, int count = 50);
    IAsyncEnumerable<JsonNode?> EnumerateItemsAsJsonAsync(string sqlQueryText);
    IAsyncEnumerable<JsonNode?> EnumerateItemsAsJsonAsync(QueryDefinition sqlQueryDefinition);
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

    Task<TOut?> GetScalarCosmosQueryResult<TOut>(QueryDefinition queryDefinition);
    Task<bool> DeleteItemAsync(string id, string partitionKey);
}