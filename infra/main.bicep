param branchName string

param spinozaBackendAccessorsTestAccessorImage string
param spinozaBackendAccessorsTestAccessorPort int
param spinozaBackendAccessorsTestAccessorIsExternalIngress bool

param spinozaBackendAccessorsQuestionAccessorImage string
param spinozaBackendAccessorsQuestionAccessorPort int
param spinozaBackendAccessorsQuestionAccessorIsExternalIngress bool

param spinozaBackendAccessorsTagAccessorImage string
param spinozaBackendAccessorsTagAccessorPort int
param spinozaBackendAccessorsTagAccessorIsExternalIngress bool

param spinozaBackendManagersCatalogManagerImage string
param spinozaBackendManagersCatalogManagerPort int
param spinozaBackendManagersCatalogManagerIsExternalIngress bool

param containerRegistry string
param containerRegistryUsername string
@secure()
param containerRegistryPassword string

param tags object

param location string = resourceGroup().location

var minReplicas = 0
var maxReplicas = 1

var branch = toLower(last(split(branchName, '/')))

var environmentName = 'shared-env'
var workspaceName = '${branch}-log-analytics'
var appInsightsName = '${branch}-app-insights'
var spinozaBackendAccessorsTestAccessorServiceContainerAppName = '${branch}-spinozaBackendAccessorsTestAccessor'
var spinozaBackendAccessorsQuestionAccessorServiceContainerAppName = '${branch}-spinozaBackendAccessorsQuestionAccessor'
var spinozaBackendAccessorsTagAccessorServiceContainerAppName = '${branch}-spinozaBackendAccessorsTagAccessor'
var spinozaBackendManagersCatalogManagerServiceContainerAppName = '${branch}-spinozaBackendManagersCatalogManager'

var containerRegistryPasswordRef = 'container-registry-password'

resource workspace 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: workspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    workspaceCapping: {}
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
  }
}

resource environment 'Microsoft.App/managedEnvironments@2022-01-01-preview' = {
  name: environmentName
  location: location
  tags: tags
  properties: {
    daprAIInstrumentationKey: appInsights.properties.InstrumentationKey
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: workspace.properties.customerId
        sharedKey: listKeys(workspace.id, workspace.apiVersion).primarySharedKey
      }
    }
  }
}

resource SpinozaBackendAccessorsTestAccessorContainerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: spinozaBackendAccessorsTestAccessorServiceContainerAppName
  kind: 'containerapps'
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      dapr: {
        enabled: true
        appPort: spinozaBackendAccessorsTestAccessorPort
        appId: spinozaBackendAccessorsTestAccessorServiceContainerAppName
      }
      secrets: [
        {
          name: containerRegistryPasswordRef
          value: containerRegistryPassword
        }
      ]
      registries: [
        {
          server: containerRegistry
          username: containerRegistryUsername
          passwordSecretRef: containerRegistryPasswordRef
        }
      ]
      ingress: {
        external: spinozaBackendAccessorsTestAccessorIsExternalIngress
        targetPort: spinozaBackendAccessorsTestAccessorPort
      }
    }
    template: {
      containers: [
        {
          image: spinozaBackendAccessorsTestAccessorImage
          name: spinozaBackendAccessorsTestAccessorServiceContainerAppName
          transport: 'auto'
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: weatherServiceContainerAppName
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

resource webServiceContainerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: webServiceContainerAppName
  kind: 'containerapps'
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      dapr: {
        enabled: true
        appPort: webServicePort
        appId: webServiceContainerAppName
      }
      secrets: [
        {
          name: containerRegistryPasswordRef
          value: containerRegistryPassword
        }
      ]
      registries: [
        {
          server: containerRegistry
          username: containerRegistryUsername
          passwordSecretRef: containerRegistryPasswordRef
        }
      ]
      ingress: {
        external: webServiceIsExternalIngress
        targetPort: webServicePort
      }
    }
    template: {
      containers: [
        {
          image: webServiceImage
          name: webServiceContainerAppName
          transport: 'auto'
          env: [
            {
              name: 'WEATHER_SERVICE_NAME'
              value: weatherServiceContainerAppName
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

output webServiceUrl string = webServiceContainerApp.properties.latestRevisionFqdn
