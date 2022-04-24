using AsyncLazy;
using Microsoft.Azure.Cosmos;
using Polly;
using Microsoft.Azure.Cosmos.Linq;
using System.Web;

namespace Spinoza.Backend.Accessor.TestCatalog.DataBases
{
    public class CosmosDBWrapper : ICosmosDBWrapper
    {
        private readonly ILogger<CosmosDBWrapper> _logger;
        private readonly AsyncLazy<Database> _lazyDatabase;
        private readonly AsyncLazy<Container> _lazyContainer;
        public Database Database => _lazyDatabase.Value;
        public Container Container => _lazyContainer.Value;

        public CosmosClient CosmosClient { get; init; }

        public CosmosDBWrapper(IConfiguration configuration, ILogger<CosmosDBWrapper> logger, ICosmosDbInformationProvider cosmosDbInformationProvider)
        {
            _logger = logger;
            CosmosClient = new CosmosClient(configuration["ConnectionStrings:CosmosDB"]);
            _lazyDatabase = new AsyncLazy<Database>(() => CreateDataBaseAsync(CosmosClient, logger, cosmosDbInformationProvider));
            _lazyContainer = new AsyncLazy<Container>(() => CreateDataBaseContainerAsync(CosmosClient, logger, Database, cosmosDbInformationProvider));
        }

        public static async Task<Database> CreateDataBaseAsync(CosmosClient cosmosClient, ILogger<CosmosDBWrapper> logger , ICosmosDbInformationProvider cosmosDbInformationProvider)
        {
            return await CreateCosmosElementAsync<Database, DatabaseProperties>(cosmosClient, logger,
                    async () => await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbInformationProvider.DataBaseName),
                    () => cosmosClient.GetDatabase(cosmosDbInformationProvider.DataBaseName));
        }

        public static async Task<Container> CreateDataBaseContainerAsync(CosmosClient cosmosClient, ILogger<CosmosDBWrapper> logger, Database database , ICosmosDbInformationProvider cosmosDbInformationProvider)
        {
            return await CreateCosmosElementAsync<Container, ContainerProperties>(cosmosClient, logger,
                async () => await database
                .DefineContainer(name: cosmosDbInformationProvider.ContainerName, partitionKeyPath: $"/{cosmosDbInformationProvider.PartitionKey}")
                .WithUniqueKey()
                .Path($"/{cosmosDbInformationProvider.PartitionKey}")
                .Attach()
                .CreateIfNotExistsAsync(),
                () => database.GetContainer(cosmosDbInformationProvider.ContainerName));
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
    }
}


    

