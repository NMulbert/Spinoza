param branchName string
@secure()
param containerRegistryPassword string

@secure()
param cosmosDBConnectionString string

param tags object = {}

param location string = resourceGroup().location

var spinozaBackendAccessorsTestAccessorImage = 'spinoza.backend.accessors.testaccessor:main'
var spinozaBackendAccessorsTestAccessorPort = 80
var spinozaBackendAccessorsTestAccessorIsExternalIngress = false

var spinozaBackendAccessorsQuestionAccessorImage = 'spinoza.backend.accessors.qustionaccessor:main'
var spinozaBackendAccessorsQuestionAccessorPort = 80
var spinozaBackendAccessorsQuestionAccessorIsExternalIngress = false

var spinozaBackendAccessorsTagAccessorImage  = 'spinoza.backend.accessors.tagaccessor:main'
var spinozaBackendAccessorsTagAccessorPort = 80
var spinozaBackendAccessorsTagAccessorIsExternalIngress = false

var spinozaBackendManagersCatalogManagerImage = 'spinoza.backend.managers.catalogmanager:main'
var spinozaBackendManagersCatalogManagerPort = 80
var spinozaBackendManagersCatalogManagerIsExternalIngress = true

var containerRegistry  = 'spinozaacr.azurecr.io'
var containerRegistryUsername = 'spinozaacr'

var minReplicas = 0
var maxReplicas = 1

var branch = toLower(last(split(branchName, '/')))

var signalRName = '${branch}-spinoza-signalr'

var environmentName = 'shared-env'
var workspaceName = '${branch}-log-analytics'
var appInsightsName = '${branch}-app-insights'
var spinozaBackendAccessorsTestAccessorServiceContainerAppName = 'testaccessor' //'${branch}-accessors-test'
var spinozaBackendAccessorsQuestionAccessorServiceContainerAppName = 'questionaccessor'//'${branch}-accessors-question'
var spinozaBackendAccessorsTagAccessorServiceContainerAppName = 'tagaccessor' //'${branch}-accessors-tag'
var spinozaBackendManagersCatalogManagerServiceContainerAppName = 'catalogmanager' //'${branch}-managers-catalog'

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

param serviceBusNamespaceName string = 'spinoza-sb-${uniqueString(resourceGroup().id)}'
param skuName string = 'Standard'

param queueNames array = [
  'testaccessor'
  'questionaccessor'
  'tagaccessor'
]

var deadLetterFirehoseQueueName = 'deadletterfirehose'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: skuName
    tier: skuName
  }
}

resource deadLetterFirehoseQueue 'Microsoft.ServiceBus/namespaces/queues@2021-11-01' = {
  name: deadLetterFirehoseQueueName
  parent: serviceBusNamespace
  properties: {
    requiresDuplicateDetection: false
    requiresSession: false
    enablePartitioning: false
  }
}

resource queues 'Microsoft.ServiceBus/namespaces/queues@2021-11-01' = [for queueName in queueNames: {
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
    defaultMessageTimeToLive:'PT1M' //1 minute
  }
}

var serviceBusEndpoint = '${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey'
var serviceBusConnectionString = listKeys(serviceBusEndpoint, serviceBusNamespace.apiVersion).primaryConnectionString


resource signalR 'Microsoft.SignalRService/signalR@2022-02-01' = {
  name: signalRName
  location: location
  sku: {
    name: 'Standard_S1'
    capacity: 1
  }
  kind: 'SignalR'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    tls: {
      clientCertEnabled: false
    }
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
    cors: {
        allowedOrigins: [
          '*'
        ]
    }
    
    networkACLs: {
      defaultAction: 'Deny'
      publicNetwork: {
        allow: [
          'ClientConnection'
        ]
      }
      privateEndpoints: [
        {
          name: 'mySignalRService.1fa229cd-bf3f-47f0-8c49-afb36723997e'
          allow: [
            'ServerConnection'
          ]
        }
      ]
    }
    /*
    upstream: {
      templates: [
        {
          categoryPattern: '*'
          eventPattern: 'connect,disconnect'
          hubPattern: '*'
          urlTemplate: 'https://spinoza.com/spinozahub/api/connect'
        }
      ]
    }*/
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

resource environment 'Microsoft.App/managedEnvironments@2022-03-01' = {
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
    scopes: [
      '${spinozaBackendAccessorsTestAccessorServiceContainerAppName}'
      '${spinozaBackendAccessorsQuestionAccessorServiceContainerAppName}'
      '${spinozaBackendAccessorsTagAccessorServiceContainerAppName}'
      '${spinozaBackendManagersCatalogManagerServiceContainerAppName}'
    ]
  }
}

var signalrKey = signalR.listKeys().primaryConnectionString

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
    scopes: [
      '${spinozaBackendManagersCatalogManagerServiceContainerAppName}'
    ]
  }
}


resource daprComponentTestAccessorRequestQueue 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: '${environmentName}/testaccessorrequestqueue'
  properties: {
    componentType: 'bindings.azure.servicebusqueues'
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
      '${spinozaBackendAccessorsTestAccessorServiceContainerAppName}'
      '${spinozaBackendManagersCatalogManagerServiceContainerAppName}'
    ]
  }
}

resource daprComponentQuestionAccessorRequestQueue 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: '${environmentName}/questionaccessorrequestqueue'
  properties: {
    componentType: 'bindings.azure.servicebusqueues'
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
      '${spinozaBackendAccessorsQuestionAccessorServiceContainerAppName}'
      '${spinozaBackendManagersCatalogManagerServiceContainerAppName}'
    ]
  }
}


resource daprComponentTagAccessorRequestQueue 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: '${environmentName}/tagaccessorrequestqueue'
  properties: {
    componentType: 'bindings.azure.servicebusqueues'
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
      '${spinozaBackendAccessorsTagAccessorServiceContainerAppName}'
      '${spinozaBackendManagersCatalogManagerServiceContainerAppName}'
    ]
  }
}

resource SpinozaBackendAccessorsTestAccessorContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
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
          name: 'container-registry-password-ref'
          value: containerRegistryPassword
        }
        {
          name: 'servicebuskeyref'
          value: serviceBusConnectionString
        }
      ]
      registries: [
        {
          server: containerRegistry
          username: containerRegistryUsername
          passwordSecretRef: 'container-registry-password-ref'
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
          image: '${containerRegistry}/${spinozaBackendAccessorsTestAccessorImage}'
          name: spinozaBackendAccessorsTestAccessorServiceContainerAppName
          resources: {
            cpu: 1
            memory: '2.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'testaccessor'
            }
            {
              name: 'ConnectionStrings__CosmosDB'
              value: cosmosDBConnectionString
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        rules: [
          {
            name: 'queue-based-scaling'
            custom: {
              type: 'azure-servicebus'
              metadata: {
                queueName: 'testaccessor'
                messageCount: '1'
              }
              auth: [
                 {
                    secretRef: 'servicebuskeyref'
                    triggerParameter: 'connection'
                 }
                ]
            }
          }
          ]
       }
    }
  }
}

resource SpinozaBackendAccessorsQuestionAccessorContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
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
          name: 'container-registry-password-ref'
          value: containerRegistryPassword
        }
        {
          name: 'servicebuskeyref'
          value: serviceBusConnectionString
        }
      ]
      registries: [
        {
          server: containerRegistry
          username: containerRegistryUsername
          passwordSecretRef: 'container-registry-password-ref'
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
          image: '${containerRegistry}/${spinozaBackendAccessorsQuestionAccessorImage}'
          name: spinozaBackendAccessorsQuestionAccessorServiceContainerAppName
          resources: {
            cpu: 1
            memory: '2.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'questionaccessor'
            }
            {
              name: 'ConnectionStrings__CosmosDB'
              value: cosmosDBConnectionString
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        rules: [
          {
            name: 'queue-based-scaling'
            custom: {
              type: 'azure-servicebus'
              metadata: {
                queueName: 'questionaccessor'
                messageCount: '1'
              }
              auth: [
                 {
                    secretRef: 'servicebuskeyref'
                    triggerParameter: 'connection'
                 }
                ]
            }
          }
          ]
       }
    }
  }
}


resource SpinozaBackendAccessorsTagAccessorContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
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
          name: 'container-registry-password-ref'
          value: containerRegistryPassword
        }
        {
          name: 'servicebuskeyref'
          value: serviceBusConnectionString
        }
      ]
      registries: [
        {
          server: containerRegistry
          username: containerRegistryUsername
          passwordSecretRef: 'container-registry-password-ref'
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
          image: '${containerRegistry}/${spinozaBackendAccessorsTagAccessorImage}'
          name: spinozaBackendAccessorsTagAccessorServiceContainerAppName
          resources: {
            cpu: 1
            memory: '2.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'tagaccessor'
            }
            {
              name: 'ConnectionStrings__CosmosDB'
              value: cosmosDBConnectionString
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        rules: [
          {
            name: 'queue-based-scaling'
            custom: {
              type: 'azure-servicebus'
              metadata: {
                queueName: 'tagaccessor'
                messageCount: '1'
              }
              auth: [
                 {
                    secretRef: 'servicebuskeyref'
                    triggerParameter: 'connection'
                 }
                ]
            }
          }
          ]
       }
    }
  }
}


resource SpinozaBackendManagersCatalogManagerContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
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
          name: 'container-registry-password-ref'
          value: containerRegistryPassword
        }
      ]
      registries: [
        {
          server: containerRegistry
          username: containerRegistryUsername
          passwordSecretRef: 'container-registry-password-ref'
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
          image: '${containerRegistry}/${spinozaBackendManagersCatalogManagerImage}'
          name: spinozaBackendManagersCatalogManagerServiceContainerAppName
          resources: {
            cpu: 1
            memory: '2.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'catalogmanager'
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
