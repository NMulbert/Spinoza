param environmentName string
param signalrKey string
param appScope array

resource daprComponentSignalR 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: '${environmentName}/azuresignalroutput'
  properties: {
    componentType: 'bindings.azure.signalr'
    version: 'v1'
    secrets: [
      {
        name: 'signalrkeyref'
        value:  signalrKey
      }
    ]
    metadata: [
      {
        name: 'connectionString'
        secretRef: 'signalrkeyref'
      }
      {
        name: 'hub'
        value: 'spinozahub'
      }
    ]
    // Application scopes
    scopes: appScope
  }
}

