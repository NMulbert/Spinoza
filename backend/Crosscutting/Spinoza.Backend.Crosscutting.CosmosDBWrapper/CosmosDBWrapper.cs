using Microsoft.Azure.Cosmos;
using Polly;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace Spinoza.Backend.Crosscutting.CosmosDBWrapper;

public class CosmosDBWrapper : ICosmosDBWrapper
{
    private readonly ILogger<CosmosDBWrapper> _logger;

    public Database Database { get; init; }
    public Container Container { get; init; }

    public CosmosClient CosmosClient { get; init; }

    public CosmosDBWrapper(IConfiguration configuration, ILogger<CosmosDBWrapper> logger, ICosmosDbInformationProvider cosmosDbInformationProvider)
    {

        _logger = logger;
        try
        {
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            CosmosSystemTextJsonSerializer cosmosSystemTextJsonSerializer
            = new CosmosSystemTextJsonSerializer(jsonSerializerOptions);
            CosmosClientOptions cosmosClientOptions = new CosmosClientOptions()
            {
                Serializer = cosmosSystemTextJsonSerializer,
                 HttpClientFactory = () =>
                 {
                     HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                     {
                         ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                     };
                     return new HttpClient(httpMessageHandler);
                 },
                ConnectionMode = ConnectionMode.Gateway
            };

            // Create a new instance of the Cosmos Client
            //CosmosClient = new CosmosClient(configuration["ConnectionStrings:CosmosDB"], cosmosClientOptions);
            //todo: inject the correct cosmosDB client
            //CosmosClientOptions cosmosClientOptions = new CosmosClientOptions()
            //{

            //};

            //CosmosClient = new CosmosClient(configuration["ConnectionStrings:CosmosDB"], cosmosClientOptions);
            //todo: move this to compose environment variable
            CosmosClient = new CosmosClient(configuration["ConnectionStrings:CosmosDB"], cosmosClientOptions);
            Database = CreateDataBase(CosmosClient, logger, cosmosDbInformationProvider);
            Container = CreateDataBaseContainer(logger, Database, cosmosDbInformationProvider);
        }
        catch (Exception ex)
        {
            _logger.LogError($"CosmosDBWrapper threw an exception: {ex.Message}");
            throw;
        }
    }


    private static Database CreateDataBase(CosmosClient cosmosClient, ILogger<CosmosDBWrapper> logger, ICosmosDbInformationProvider cosmosDbInformationProvider)
    {

        return (CreateCosmosElementAsync<Database, DatabaseProperties>(logger,
                async () => await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbInformationProvider.DataBaseName),
                () => cosmosClient.GetDatabase(cosmosDbInformationProvider.DataBaseName))).Result;
    }

    private static Container CreateDataBaseContainer(ILogger<CosmosDBWrapper> logger, Database database, ICosmosDbInformationProvider cosmosDbInformationProvider)
    {

        if (cosmosDbInformationProvider.UniqueKeys.Any())
        {
            return (CreateCosmosElementAsync<Container, ContainerProperties>(logger,
                async () => await database
                .DefineContainer(name: cosmosDbInformationProvider.ContainerName, partitionKeyPath: $"/{cosmosDbInformationProvider.PartitionKey}")
                .WithUniqueKey()
                .Path(cosmosDbInformationProvider.UniqueKeys.Aggregate(new StringBuilder(), (sb, p) => sb.Append($"/{p},"), sb => { sb.Length -= 1; return sb.ToString(); }))
                .Attach()
                .CreateIfNotExistsAsync(),
                () => database.GetContainer(cosmosDbInformationProvider.ContainerName))).Result;
        }
        //else
        return (CreateCosmosElementAsync<Container, ContainerProperties>(logger,
                async () => await database
                .DefineContainer(name: cosmosDbInformationProvider.ContainerName, partitionKeyPath: $"/{cosmosDbInformationProvider.PartitionKey}")
                .CreateIfNotExistsAsync(),
                () => database.GetContainer(cosmosDbInformationProvider.ContainerName))).Result;
    }

    private static async Task<TOut> CreateCosmosElementAsync<TOut, TProperties>(ILogger<CosmosDBWrapper> logger, Func<Task<Response<TProperties>?>> createFuncAsync, Func<TOut> returnFunc)
    {
        var polly = Policy
         .Handle<CosmosException>(ex => ex.StatusCode != System.Net.HttpStatusCode.Conflict)
         //Todo: move 3 to configuration
         .RetryAsync(3, (exception, retryCount, _) => logger.LogError($"try: {retryCount}, Exception: {exception.Message}"));

        var result = await polly.ExecuteAsync(async () =>
        {
            try
            {
                var response = await createFuncAsync();
                logger.LogInformation($"create cosmosDB element returns: {response?.StatusCode}, cost: {response?.RequestCharge} RU/S");
                var element = returnFunc();
                return element;
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    logger.LogWarning($"Too many requests when accessing cosmosDB, waiting: {cosmosException.RetryAfter}");
                    await Task.Delay(cosmosException.RetryAfter ?? TimeSpan.FromSeconds(1));
                    //Todo: move 1 to configuration
                    throw;
                }
                
                //else
                logger.LogError($"CreateCosmosElementAsync: Error accessing cosmosDB, Error : {cosmosException.Message}");
                throw;// new Exception("Error accessing cosmosDB", cosmosException);
            }
        }
        );
        return result;
    }

    public async Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        ItemResponse<T>? response = null;
        return await CreateCosmosElementAsync<ItemResponse<T>, T>(_logger,
                async () => response = await Container.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken),
                () => response!);
    }

    public async IAsyncEnumerable<JsonNode?> EnumerateItemsAsJsonAsync(string sqlQueryText)
    {
        QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);

        await foreach (JsonNode? item in EnumerateItemsAsJsonAsync(queryDefinition))
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<JsonNode?> EnumerateItemsAsJsonAsync(QueryDefinition sqlQueryDefinition)
    {
        var queryIterator = Container.GetItemQueryStreamIterator(sqlQueryDefinition);


        while (queryIterator.HasMoreResults)
        {
            var currentResultSet = await queryIterator.ReadNextAsync();

            var jsonNode = JsonNode.Parse(currentResultSet.Content);
            var items = jsonNode!.Root["Documents"]!.AsArray();

            while (items.Any())
            {
                var item = items[0];
                items.Remove(item);

                yield return item;
            }

        }
    }



    public async Task<IList<TOut>> GetCosmosElementsAsync<TOut>(QueryDefinition queryDefinition)
    {
        try
        {
            List<TOut> itemList = new List<TOut>();
            double totalRequestCharge = 0;
            using FeedIterator<TOut> setIterator = Container.GetItemQueryIterator<TOut>(queryDefinition);

            //Asynchronous query execution
            while (setIterator.HasMoreResults)
            {
                FeedResponse<TOut> response = await setIterator.ReadNextAsync();

                _logger.LogInformation($"Get cosmosDB element returns: {response?.StatusCode}, cost: {response?.RequestCharge} RU/S, number of items{response?.Count}");
                totalRequestCharge += response?.RequestCharge ?? 0;

                if (response == null)
                {
                    throw new Exception("Error reading items from cosmos");
                }

                if (response.Count == 0)
                {
                    await Task.Delay(1000);
                    //Todo: move 1000 to configuration: Default429Delay
                }
                itemList.AddRange(response);

            }
            _logger.LogInformation($"Number of elements that query returned: {itemList.Count}, at cost of: {totalRequestCharge} RU/S");
            return itemList;

        }
        catch (Exception ex)
        {

            _logger.LogError($"GetCosmosElementsAsync: Error accessing cosmosDB, Error : {ex.Message}");
            throw;
        }
    }

    public async Task<IList<TOut>> GetAllCosmosElementsAsync<TOut>(int skip = 0, int count = 50 )
    {
        var query = new QueryDefinition($"SELECT * FROM c OFFSET @skip LIMIT @count").WithParameter("@skip", skip)
            .WithParameter("@count", count);
        return await GetCosmosElementsAsync<TOut>(query);
    }
    

    public async Task<TOut?> GetScalarCosmosQueryResult<TOut>(QueryDefinition queryDefinition)
    {
        var queryResult = await GetCosmosElementsAsync<TOut>(queryDefinition);
        return queryResult.FirstOrDefault();
    }

    public async Task<ItemResponse<T>?> UpdateItemAsync<T>(T newItem, Func<T, string?> eTagSelector, Func<T, Guid> idSelector, Func<T, T, T> merger, bool createIfNotExist = true, PartitionKey? partitionKey = null, CancellationToken cancellationToken = default(CancellationToken))
    {

        var polly = Policy
        .Handle<CosmosException>()
        //Todo: move 3 to configuration
        .RetryAsync(3, (exception, retryCount, _) => _logger.LogError($"try: {retryCount}, Exception: {exception.Message}"));

        var result = await polly.ExecuteAsync(async () =>
        {

            try
            {

                var query = new QueryDefinition("SELECT * FROM ITEMS item WHERE item.id = @id").WithParameter("@id", idSelector(newItem).ToString().ToUpper());
                var dbItem = (await GetCosmosElementsAsync<T>(query)).FirstOrDefault();
                if (dbItem == null)
                {
                    _logger.LogInformation($"UpdateItemAsync doesn't exist:");
                    return await CreateIfItemIsNeededAsync();
                }
                var mergedItem = merger(dbItem, newItem);
                var requestOption = new ItemRequestOptions()
                {
                    IfMatchEtag = eTagSelector(mergedItem)
                };
                // ReSharper disable once MethodSupportsCancellation
                var result = await Container.ReplaceItemAsync(mergedItem, idSelector(mergedItem).ToString().ToUpper(), partitionKey, requestOption);
                _logger.LogInformation($"create cosmosDB element returns: {result?.StatusCode}, cost: {result?.RequestCharge} RU/S");
                return result;
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning($"Too many requests when accessing cosmosDB, waiting: {cosmosException.RetryAfter}");
                    // ReSharper disable once MethodSupportsCancellation
                    await Task.Delay(cosmosException.RetryAfter ?? TimeSpan.FromSeconds(1));
                    //Todo: move 1 to configuration
                    throw;
                }
                if (cosmosException.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                {
                    _logger.LogWarning("eTag was changed");
                    throw;
                }

                //else
                _logger.LogError($"UpdateItemAsync: Error accessing cosmosDB, Error : {cosmosException.Message}");
                throw;// new Exception("Error accessing cosmosDB", cosmosException);
            }

            async Task<ItemResponse<T>?> CreateIfItemIsNeededAsync()
            {
                if (!createIfNotExist)
                {
                    _logger.LogInformation("CreateIfItemIsNeededAsync: nothing to do");
                    return null;
                }
                return await CreateItemAsync(newItem, partitionKey, null, cancellationToken);
            }
        }
        );
        return result;

    }

    public async Task<bool> DeleteItemAsync(string id, string partitionKey)
    {
        using (ResponseMessage response = await Container.DeleteItemStreamAsync(id, new PartitionKey(partitionKey)))
        {
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Failed to delete the Item, id: {id} Error code:{response.StatusCode}");
                return false;
            }
            _logger.LogInformation($"Item {id} was deleted");
            return true;
        }
    }

}
