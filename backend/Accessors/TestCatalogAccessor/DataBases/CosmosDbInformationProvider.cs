namespace Spinoza.Backend.Accessor.TestCatalog.DataBases
{
    public class CosmosDbInformationProvider : ICosmosDbInformationProvider
    {
        public string DataBaseName { get; private set; }
        public string ContainerName { get; private set; }
        public string PartitionKey { get; private set; }
        public string? UniqueKey { get; private set; }
        public CosmosDbInformationProvider(string dataBaseName, string containerName, string partitionKey)
        {
            DataBaseName = dataBaseName;
            ContainerName = containerName;
            PartitionKey = partitionKey;
        }
            public CosmosDbInformationProvider(string dataBaseName, string containerName, string partitionKey, string uniqueKey) : base()
        {
            DataBaseName = dataBaseName;
            ContainerName = containerName;
            PartitionKey = partitionKey;
            UniqueKey = uniqueKey;
        }
    }
}


    

