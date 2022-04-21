using AsyncLazy;
using Microsoft.Azure.Cosmos;
using Polly;

namespace Spinoza.Backend.Accessor.TestCatalog.DataBases
{
    public class CosmosDBWrapper : ICosmosDBWrapper
    {
        private readonly ILogger<CosmosDBWrapper> _logger;
        private readonly AsyncLazy<Database> _lazyTestDatabase;
        private readonly AsyncLazy<Container> _lazyTestContainer;
        public Database TestDatabase => _lazyTestDatabase.Value;
        public Container TestContainer => _lazyTestContainer.Value;

        public CosmosClient CosmosClient { get; init; }

        public CosmosDBWrapper(IConfiguration configuration, ILogger<CosmosDBWrapper> logger)
        {
            _logger = logger;
            CosmosClient = new CosmosClient(configuration["ConnectionStrings:Tests"]);
            _lazyTestDatabase = new AsyncLazy<Database>(() => CreateDataBaseAsync(CosmosClient, logger));
            _lazyTestContainer = new AsyncLazy<Container>(() => CreateDataBaseContainerAsync(CosmosClient, logger, TestDatabase));
        }

        public static async Task<Database> CreateDataBaseAsync(CosmosClient cosmosClient, ILogger<CosmosDBWrapper> logger)
        {
            return await CreateCosmosElementAsync<Database,DatabaseProperties>(cosmosClient, logger, 
                    async () => await cosmosClient.CreateDatabaseIfNotExistsAsync("Catalog"),
                    ()=> cosmosClient.GetDatabase($"Catalog"));
        }

        public static async Task<Container> CreateDataBaseContainerAsync(CosmosClient cosmosClient, ILogger<CosmosDBWrapper> logger, Database database)
        {
            return await CreateCosmosElementAsync<Container,ContainerProperties>(cosmosClient, logger,
                async () => await database
                .DefineContainer(name: $"Tests", partitionKeyPath: "/Title")
                .WithUniqueKey()
                .Path("/Title")
                .Attach()
                .CreateIfNotExistsAsync(),
                () => database.GetContainer($"Tests"));
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
                    throw new Exception("Error accessing cosmosDB", cosmosException);
                }
            }
            );
            return result;
        }
        
        public  async Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            ItemResponse<T>? response =null;
            return await CreateCosmosElementAsync<ItemResponse<T>, T>(CosmosClient, _logger,
                    async () => response =  await TestContainer.CreateItemAsync(item,partitionKey, requestOptions, cancellationToken),
                    () => response!);
        }
    }
}
