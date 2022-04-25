targetScope = 'resourceGroup'

param logAnalyticsWorkspaceName string = 'log-spinoza-test'
param appInsightsName string ='insight-spinoza-test'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: resourceGroup().location
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
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId:logAnalyticsWorkspace.id
  }
}

output insightInstrumentationKey string = appInsights.properties.InstrumentationKey
output logCustomerId string = logAnalyticsWorkspace.properties.customerId
output logPrimarySharedKey string = logAnalyticsWorkspace.listKeys().primarySharedKey

