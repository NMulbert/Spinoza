/*
@description('Cosmos DB account name')
param accountName string = 'cosmos-${uniqueString(resourceGroup().id)}'


@description('The name for the Core (SQL) database')
param databaseName string

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' = {
  name: toLower(accountName)
  location: location
  properties: {
    enableFreeTier: true
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Strong'
    }
    locations: [
      {
        locationName: location
      }
    ]
  }
}
*/
//todo: ComosDB connection string should go to environement variable
//resource cosmosDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-04-15' = {
//  name: '${cosmosAccount.name}/${toLower(databaseName)}'
//  properties: {
//    resource: {
//      id: databaseName
//    }
//    options: {
//      throughput: 1000
//    }
//  }
//}
