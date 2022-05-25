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

param signalRName string

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
var serviceBusConnectionStringKey = '${branch}-serviceBusConnectionStringKey'
var signalRConnectionStringKey = '${branch}-signalRConnectionStringKey'
var containerRegistryPasswordRef = 'container-registry-password'


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

//todo: ComosDB connection string should go to environement variable
resource cosmosDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-04-15' = {
  name: '${cosmosAccount.name}/${toLower(databaseName)}'
  properties: {
    resource: {
      id: databaseName
    }
    options: {
      throughput: 1000
    }
  }
}

param serviceBusNamespaceName string = 'myapp${uniqueString(resourceGroup().id)}'
param skuName string = 'Basic'

param queueNames array = [
  'testaccessor'
  'questionaccessor'
  'tagaccessor'
]

var deadLetterFirehoseQueueName = 'deadletterfirehose'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2018-01-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: skuName
  }
}

resource deadLetterFirehoseQueue 'Microsoft.ServiceBus/namespaces/queues@2018-01-01-preview' = {
  name: deadLetterFirehoseQueueName
  parent: serviceBusNamespace
  properties: {
    requiresDuplicateDetection: false
    requiresSession: false
    enablePartitioning: false
  }
}

resource queues 'Microsoft.ServiceBus/namespaces/queues@2018-01-01-preview' = [for queueName in queueNames: {
  parent: serviceBusNamespace
  name: queueName
  dependsOn: [
    deadLetterFirehoseQueue
  ]
  properties: {
    forwardDeadLetteredMessagesTo: deadLetterFirehoseQueueName
  }
}]

resource pubsub 'Microsoft.ServiceBus/namespaces/topics@2021-11-01' = {
  parent: serviceBusNamespace
  name: 'pubsub'
  properties: {
    defaultMessageTimeToLive:'60'
  }
}

var serviceBusEndpoint = '${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey'
var serviceBusConnectionString = listKeys(serviceBusEndpoint, serviceBusNamespace.apiVersion).primaryConnectionString

resource signalR 'Microsoft.SignalRService/signalR@2022-02-01' = {
  name: signalRName
  location: location
  kind: 'SignalR'
  sku: {
    name: 'Free_F1'
    tier: 'Free'
    capacity: 1
  }
  properties: {
    features: [
      {
        flag: 'ServiceMode'
        value: 'Serverless'
      }
      {
        flag: 'EnableConnectivityLogs'
        value: 'true'
      }
      {
        flag: 'EnableMessagingLogs'
        value: 'true'
      }
      {
        flag: 'EnableLiveTrace'
        value: 'true'
      }
    ]
    liveTraceConfiguration: {
      enabled: 'true'
      categories: [
        {
          name: 'ConnectivityLogs'
          enabled: 'true'
        }
        {
          name: 'MessagingLogs'
          enabled: 'true'
        }
        {
          name: 'HttpRequestLogs'
          enabled: 'true'
        }
      ]
    }
    cors: {
      allowedOrigins: [
        '*'
      ]
    }
    upstream: {
      templates: [
        {
          hubPattern: '*'
          eventPattern: '*'
          categoryPattern: '*'
          auth: {
            type: 'None'
          }
          urlTemplate: 'https://${signalRName}.azurewebsites.net/runtime/webhooks/signalr?code=TBD'
        }
      ]
    }
  }
}

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

resource daprComponentPubSub 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: 'pubsub'
  properties: {
    componentType: 'pubsub.azure.servicebus'
    version: 'v1'
    metadata: [
      {
        name: 'connectionString'
        value: serviceBusConnectionString
      }
    ]
    // Application scopes
    scopes: [
      'SpinozaBackendAccessorsTestAccessorContainerApp'
      'SpinozaBackendAccessorsQuestionAccessorContainerApp'
      'SpinozaBackendAccessorsTagAccessorContainerApp'
      'SpinozaBackendManagersCatalogManagerContainerApp'
    ]
  }
}


resource daprComponentSignalR 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: 'azuresignalroutput'
  properties: {
    componentType: 'bindings.azure.signalr'
    version: 'v1'
    metadata: [
      {
        name: 'connectionString'
        value: listkeys(resourceId('Microsoft.SignalRService/SignalR', 'resourceName'), '2022-02-01' )
      }
      {
        name: 'hub'
        value: 'spinozahub'
      }
    ]
    // Application scopes
    scopes: [
      'SpinozaBackendManagersCatalogManagerContainerApp'
    ]
  }
}


resource daprComponentTestAccessorRequestQueue 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: 'testaccessorrequestqueue'
  properties: {
    componentType: 'bindings.azure.servicebusqueues'
    version: 'v1'
    metadata: [
      {
        name: 'connectionString'
        value: serviceBusConnectionString
      }
      {
        name: 'queueName'
        value: 'testaccessor'
      }
      {
        name: 'ttlInSeconds'
        value: '60'
      }
    ]
    // Application scopes
    scopes: [
      'SpinozaBackendAccessorsTestAccessorContainerApp'
      'SpinozaBackendManagersCatalogManagerContainerApp'
    ]
  }
}

resource daprComponentQuestionAccessorRequestQueue 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: 'questionaccessorrequestqueue'
  properties: {
    componentType: 'bindings.azure.servicebusqueues'
    version: 'v1'
    metadata: [
      {
        name: 'connectionString'
        value: serviceBusConnectionString
      }
      {
        name: 'queueName'
        value: 'questionaccessor'
      }
      {
        name: 'ttlInSeconds'
        value: '60'
      }
    ]
    // Application scopes
    scopes: [
      'SpinozaBackendAccessorsQuestionAccessorContainerApp'
      'SpinozaBackendManagersCatalogManagerContainerApp'
    ]
  }
}


resource daprComponentTagAccessorRequestQueue 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: 'tagaccessorrequestqueue'
  properties: {
    componentType: 'bindings.azure.servicebusqueues'
    version: 'v1'
    metadata: [
      {
        name: 'connectionString'
        value: serviceBusConnectionString
      }
      {
        name: 'queueName'
        value: 'tagaccessor'
      }
      {
        name: 'ttlInSeconds'
        value: '60'
      }
    ]
    // Application scopes
    scopes: [
      'SpinozaBackendAccessorsTagAccessorContainerApp'
      'SpinozaBackendManagersCatalogManagerContainerApp'
    ]
  }
}

resource SpinozaBackendAccessorsTestAccessorContainerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: spinozaBackendAccessorsTestAccessorServiceContainerAppName
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
          resources: {
            cpu: 1
            memory: '1.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'spinoza.backend.accessors.testaccessor'
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

resource SpinozaBackendAccessorsQuestionAccessorContainerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: spinozaBackendAccessorsQuestionAccessorServiceContainerAppName
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      dapr: {
        enabled: true
        appPort: spinozaBackendAccessorsQuestionAccessorPort
        appId: spinozaBackendAccessorsQuestionAccessorServiceContainerAppName
        appProtocol: 'http'
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
        external: spinozaBackendAccessorsQuestionAccessorIsExternalIngress
        targetPort: spinozaBackendAccessorsQuestionAccessorPort
      }
    }
    template: {
      containers: [
        {
          image: spinozaBackendAccessorsQuestionAccessorImage
          name: spinozaBackendAccessorsQuestionAccessorServiceContainerAppName
          resources: {
            cpu: 1
            memory: '1.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'spinoza.backend.accessors.questionaccessor'
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


resource SpinozaBackendAccessorsTagAccessorContainerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: spinozaBackendAccessorsTagAccessorServiceContainerAppName
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      dapr: {
        enabled: true
        appPort: spinozaBackendAccessorsTagAccessorPort
        appId: spinozaBackendAccessorsTagAccessorServiceContainerAppName
        appProtocol: 'http'
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
        external: spinozaBackendAccessorsTagAccessorIsExternalIngress
        targetPort: spinozaBackendAccessorsTagAccessorPort
      }
    }
    template: {
      containers: [
        {
          image: spinozaBackendAccessorsTagAccessorImage
          name: spinozaBackendAccessorsTagAccessorServiceContainerAppName
          resources: {
            cpu: 1
            memory: '1.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'spinoza.backend.accessors.tagaccessor'
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


resource SpinozaBackendManagersCatalogManagerContainerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: spinozaBackendManagersCatalogManagerServiceContainerAppName
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      dapr: {
        enabled: true
        appPort: spinozaBackendManagersCatalogManagerPort
        appId: spinozaBackendManagersCatalogManagerServiceContainerAppName
        appProtocol: 'http'
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
        external: spinozaBackendManagersCatalogManagerIsExternalIngress
        targetPort: spinozaBackendManagersCatalogManagerPort
      }
    }
    template: {
      containers: [
        {
          image: spinozaBackendManagersCatalogManagerImage
          name: spinozaBackendAccessorsTagAccessorServiceContainerAppName
          resources: {
            cpu: 1
            memory: '1.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'spinoza.backend.managers.catalogmanager'
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
  dependsOn: [
    SpinozaBackendAccessorsTestAccessorContainerApp
    SpinozaBackendAccessorsQuestionAccessorContainerApp
    SpinozaBackendAccessorsTagAccessorContainerApp
  ]
}

output webServiceUrl string = SpinozaBackendManagersCatalogManagerContainerApp.properties.latestRevisionFqdn
