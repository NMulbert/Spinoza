﻿namespace Spinoza.Backend.Accessor.TestCatalog.DataBases
{
    public interface ICosmosDbInformationProvider
    {
        string ContainerName { get; }
        string DataBaseName { get; }

        string PartitionKey { get; }

        string[] UniqueKeys { get; }

    }
}