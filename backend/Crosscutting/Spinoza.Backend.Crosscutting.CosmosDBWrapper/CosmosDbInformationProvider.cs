namespace Spinoza.Backend.Crosscutting.CosmosDBWrapper;

public class CosmosDbInformationProvider : ICosmosDbInformationProvider
{
    public string DataBaseName { get; private set; }
    public string ContainerName { get; private set; }
    public string PartitionKey { get; private set; }
    public string[] UniqueKeys { get; private set; }
    public CosmosDbInformationProvider(string dataBaseName, string containerName, string partitionKey, params string[] uniqueKeys)
    {
        DataBaseName = dataBaseName;
        ContainerName = containerName;
        PartitionKey = partitionKey;
        UniqueKeys = uniqueKeys;
    }

}
