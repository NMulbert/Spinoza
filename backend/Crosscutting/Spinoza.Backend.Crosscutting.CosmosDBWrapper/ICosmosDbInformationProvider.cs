namespace Spinoza.Backend.Crosscutting.CosmosDBWrapper;

public interface ICosmosDbInformationProvider
{
    string ContainerName { get; }
    string DataBaseName { get; }

    string PartitionKey { get; }

    string[] UniqueKeys { get; }

}
