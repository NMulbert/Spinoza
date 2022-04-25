using Microsoft.Azure.Cosmos;
using Polly;
using Microsoft.Azure.Cosmos.Linq;
using System.Web;

namespace Spinoza.Backend.Accessor.TestCatalog.DataBases
{
    public class CosmosDBWrapper : ICosmosDBWrapper
    {
        private readonly ILogger<CosmosDBWrapper> _logger;
        private readonly Lazy<Database> _lazyDatabase;
        private readonly Lazy<Container> _lazyContainer;
        public Database Database => _lazyDatabase.Value;
        public Container Container => _lazyContainer.Value;

        public CosmosClient CosmosClient { get; init; }

        public CosmosDBWrapper(IConfiguration configuration, ILogger<CosmosDBWrapper> logger, ICosmosDbInformationProvider cosmosDbInformationProvider)
        {
            _logger = logger;
            CosmosClient = new CosmosClient(configuration["ConnectionStrings:CosmosDB"]);
            _lazyDatabase = new Lazy<Database>(() => CreateDataBase(CosmosClient, logger, cosmosDbInformationProvider),true);
            _lazyContainer = new Lazy<Container>(() => CreateDataBaseContainer(CosmosClient, logger, Database, cosmosDbInformationProvider),true);
        }

        public static  Database CreateDataBase(CosmosClient cosmosClient, ILogger<CosmosDBWrapper> logger , ICosmosDbInformationProvider cosmosDbInformationProvider)
        {
            return  (CreateCosmosElementAsync<Database, DatabaseProperties>(cosmosClient, logger,
                    async () => await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbInformationProvider.DataBaseName),
                    () => cosmosClient.GetDatabase(cosmosDbInformationProvider.DataBaseName))).Result;
        }

        public static Container CreateDataBaseContainer(CosmosClient cosmosClient, ILogger<CosmosDBWrapper> logger, Database database , ICosmosDbInformationProvider cosmosDbInformationProvider)
        {
            return  (CreateCosmosElementAsync<Container, ContainerProperties>(cosmosClient, logger,
                async () => await database
                .DefineContainer(name: cosmosDbInformationProvider.ContainerName, partitionKeyPath: $"/{cosmosDbInformationProvider.PartitionKey}")
                .WithUniqueKey()
                .Path($"/{cosmosDbInformationProvider.UniqueKey}")
                .Attach()
                .CreateIfNotExistsAsync(),
                () => database.GetContainer(cosmosDbInformationProvider.ContainerName))).Result;
        }
        public static async Task<TOut> CreateCosmosElementAsync<TOut, TProperties>(CosmosClient cosmosClient, ILogger<CosmosDBWrapper> logger, Func<Task<Response<TProperties>?>> createFuncAsync, Func<TOut> returnFunc)
        {
            var polly = Policy
             .Handle<CosmosException>()
             //Todo: move 3 to configuration
             .RetryAsync(3, (exception, retryCount, context) => logger.LogError($"try: {retryCount}, Exception: {exception.Message}"));

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
            return await CreateCosmosElementAsync<ItemResponse<T>, T>(CosmosClient, _logger,
                    async () => response = await Container.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken),
                    () => response!);
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

        public async Task<IList<TOut>> GetAllCosmosElementsAsync<TOut>(int skip = 0, int count = 50)
        {
            var query = new QueryDefinition($"SELECT * FROM c OFFSET @skip LIMIT @count").WithParameter("@skip", skip)
                .WithParameter("@count", count);
            return await GetCosmosElementsAsync<TOut>(query); 
        }

        public async Task<ItemResponse<T>?> UpdateItemAsync<T>(T newItem, Func<T,string?> eTagSelector, Func<T,Guid> idSelector, Func<T,T,T> merger, bool createIfNotExist=true, PartitionKey? partitionKey = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            
            var polly = Policy
            .Handle<CosmosException>()
            //Todo: move 3 to configuration
            .RetryAsync(3, (exception, retryCount, context) => _logger.LogError($"try: {retryCount}, Exception: {exception.Message}"));

            var result = await polly.ExecuteAsync(async () =>
            {
                
                try
                {
                    var query = new QueryDefinition("SELECT * FROM ITEMS item WHERE item.id = @id").WithParameter("@id", idSelector(newItem));
                    var dbItem = (await GetCosmosElementsAsync<T>(query)).FirstOrDefault();
                    if (dbItem == null)
                    {
                        _logger.LogInformation($"UpdateItemAsync doesn't exist:");
                        return await CreateIfItemIsNeededAsync();
                    }
                    var mergedItem = merger(dbItem,newItem);
                    var requestOption = new ItemRequestOptions()
                    {
                        IfMatchEtag = eTagSelector(mergedItem)
                    };
                    var result = await Container.ReplaceItemAsync(mergedItem, idSelector(mergedItem).ToString(), partitionKey, requestOption);
                    _logger.LogInformation($"create cosmosDB element returns: {result?.StatusCode}, cost: {result?.RequestCharge} RU/S");
                    return result;
                }
                catch (CosmosException cosmosException)
                {
                    if (cosmosException.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        _logger.LogWarning($"Too many requests when accessing cosmosDB, waiting: {cosmosException.RetryAfter}");
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
    }
}


    

