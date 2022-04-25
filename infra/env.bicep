targetScope = 'resourceGroup'

param environmentName string = 'preprod'
param insightInstrumentationKey string
param logcustomerId string
param logprimarySharedKey string

resource environment 'Microsoft.App/managedEnvironments@2022-01-01-preview' = {
  name: environmentName
  location:  resourceGroup().location
  properties: {
    daprAIInstrumentationKey:insightInstrumentationKey
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logcustomerId
        sharedKey: logprimarySharedKey
      }
    }
  }
  resource daprComponent 'daprComponents@2022-01-01-preview' = {
    name: 'mycomponent'
    properties: {
      componentType: 'state.azure.storagequeues'
      version: 'v1'
      ignoreErrors: true
      initTimeout: '5s'
      secrets: [
        {
          name: 'masterkeysecret'
          value: 'secretvalue'
        }
      ]
      metadata: [
        {
          name: 'masterkey'
          secretRef: 'masterkeysecret'
        }
        {
          name: 'foo'
          value: 'bar'
        }
      ]
      
    }
  } 
}
