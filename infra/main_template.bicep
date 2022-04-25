param environmentName string
param appName string
param appId string
param logAnalyticsWorkspaceName string = 'logs-${environmentName}'
param appInsightsName string = 'appins-${environmentName}'
param location string = resourceGroup().location

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId:logAnalyticsWorkspace.id
  }
}

resource environment 'Microsoft.App/managedEnvironments@2022-01-01-preview' = {
  name: environmentName
  location: location
  properties: {
    daprAIInstrumentationKey:appInsights.properties.InstrumentationKey
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspace.properties.customerId
        sharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
      }
    }
  }

  resource daprComponent 'daprComponents@2022-01-01-preview' = {
    name: 'mycomponent'
    properties: {
      componentType: 'state.azure.cosmosdb'
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
      scopes:[
        appId
      ]
    }
  }

}

resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' ={
  name: appName
  location: location
  properties:{
    managedEnvironmentId: environment.id
    configuration: {
      ingress: {
        targetPort: 80
        external: true
      }
      dapr: {
        enabled: true
        appId: appId
        appProtocol: 'http'
        appPort: 80
      }
    }
    template: {
      containers: [
        {
          image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
          name: 'simple-hello-world-container'
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 1
      }
    }
  }
}

output location string = location
output environmentId string = environment.id

