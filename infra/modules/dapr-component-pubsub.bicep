param environmentName string
param serviceBusConnectionString string
param appScope array

resource daprComponentPubSub 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: '${environmentName}/pubsub'
  properties: {
    componentType: 'pubsub.azure.servicebus'
    version: 'v1'
    secrets: [
      {
        name: 'servicebuskeyref'
        value: serviceBusConnectionString
      }
    ]
    metadata: [
      {
        name: 'connectionString'
        secretRef: 'servicebuskeyref'
      }
    ]
    // Application scopes
    scopes: appScope
  }
}
