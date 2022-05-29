param branchName string
@secure()
param containerRegistryPassword string

@secure()
param cosmosDBConnectionString string

param tags object = {}

param location string = resourceGroup().location

var spinozaBackendAccessorsTestAccessorImage = 'spinoza.backend.accessors.testaccessor:${branchName}'
var spinozaBackendAccessorsTestAccessorPort = 80
var spinozaBackendAccessorsTestAccessorIsExternalIngress = false

var spinozaBackendAccessorsQuestionAccessorImage = 'spinoza.backend.accessors.qustionaccessor:${branchName}'
var spinozaBackendAccessorsQuestionAccessorPort = 80
var spinozaBackendAccessorsQuestionAccessorIsExternalIngress = false

var spinozaBackendAccessorsTagAccessorImage  = 'spinoza.backend.accessors.tagaccessor:${branchName}'
var spinozaBackendAccessorsTagAccessorPort = 80
var spinozaBackendAccessorsTagAccessorIsExternalIngress = false

var spinozaBackendManagersCatalogManagerImage = 'spinoza.backend.managers.catalogmanager:${branchName}'
var spinozaBackendManagersCatalogManagerPort = 80
var spinozaBackendManagersCatalogManagerIsExternalIngress = true

var spinozaBackendSignalrNegotiateImage = 'spinoza.backend.signalr.negotiate:${branchName}'
var spinozaBackendSignalrNegotiatePort = 80
var spinozaBackendSignalrNegotiateIsExternalIngress = true

var containerRegistry  = 'spinozaacr.azurecr.io'
var containerRegistryUsername = 'spinozaacr'

var minReplicas = 0
var maxReplicas = 1

var branch = toLower(last(split(branchName, '/')))

var signalRName = '${branch}-spinoza-signalr'

var environmentName = 'shared-env'
var workspaceName = '${branch}-log-analytics'
var appInsightsName = '${branch}-app-insights'
var spinozaBackendSignalrNegotiateServiceContainerAppName = 'signalrnegotiate'
var spinozaBackendAccessorsTestAccessorServiceContainerAppName = 'testaccessor' //'${branch}-accessors-test'
var spinozaBackendAccessorsQuestionAccessorServiceContainerAppName = 'questionaccessor'//'${branch}-accessors-question'
var spinozaBackendAccessorsTagAccessorServiceContainerAppName = 'tagaccessor' //'${branch}-accessors-tag'
var spinozaBackendManagersCatalogManagerServiceContainerAppName = 'catalogmanager' //'${branch}-managers-catalog'

module signalr 'modules/signalr.bicep' = {
  name: 'signalrDeployment'
  params: {
    signalRName: signalRName
    location: location
  }
}
var signalrKey = signalr.outputs.signalrKey


module servicebus 'modules/servicebus.bicep' = {
  name: 'servicebusQueuesAndPubSubDeployment'
  params: {
    location: location
  }
}
var serviceBusConnectionString = servicebus.outputs.serviceBusConnectionString

module containersAppInfra 'modules/containers-app-infra.bicep' = {
  name: 'containersAppInfraDeployment'
  params: {
    location: location
    appInsightsName: appInsightsName
    environmentName: environmentName
    workspaceName: workspaceName
    tags: tags
  }
}
var environmentId = containersAppInfra.outputs.environmentId

module daprComponentPubSub 'modules/dapr-component-pubsub.bicep' = {
  name: 'daprComponentPubSubDeployment'
  params: {
    environmentName: environmentName
    serviceBusConnectionString: serviceBusConnectionString
    appScope: [
      spinozaBackendAccessorsTestAccessorServiceContainerAppName
      spinozaBackendAccessorsQuestionAccessorServiceContainerAppName
      spinozaBackendAccessorsTagAccessorServiceContainerAppName
      spinozaBackendManagersCatalogManagerServiceContainerAppName
    ]
  }
  dependsOn:  [
    containersAppInfra
    servicebus
  ]
}

module daprComponentSignalr 'modules/dapr-component-signalr.bicep' = {
  name: 'daprComponentSignalRDeployment'
  params: {
    environmentName: environmentName
    signalrKey: signalrKey
    appScope: [
      spinozaBackendManagersCatalogManagerServiceContainerAppName
    ]
  }
  dependsOn:  [
    containersAppInfra
    signalr
  ]
}

module daprComponentTestAccessorRequestQueue 'modules/dapr-component-queue.bicep' = {
  name: 'daprComponentTestAccessorRequestQueueDeployment'
  params: {
    queueName:'testaccessor'
    environmentName: environmentName
    serviceBusConnectionString: serviceBusConnectionString
    appScope: [
      '${spinozaBackendAccessorsTestAccessorServiceContainerAppName}'
      '${spinozaBackendManagersCatalogManagerServiceContainerAppName}'
    ]
  }
  dependsOn:  [
    containersAppInfra
    servicebus
  ]
}


module daprComponentQuestionAccessorRequestQueue 'modules/dapr-component-queue.bicep' = {
  name: 'daprComponentQuestionAccessorRequestQueueDeployment'
  params: {
    queueName:'questionaccessor'
    environmentName: environmentName
    serviceBusConnectionString: serviceBusConnectionString
    appScope: [
      '${spinozaBackendAccessorsQuestionAccessorServiceContainerAppName}'
      '${spinozaBackendManagersCatalogManagerServiceContainerAppName}'
    ]
  }
  dependsOn:  [
    containersAppInfra
    servicebus
  ]
}


module daprComponentTagAccessorRequestQueue 'modules/dapr-component-queue.bicep' = {
  name: 'daprComponentTagAccessorRequestQueueDeployment'
  params: {
    queueName:'tagaccessor'
    environmentName: environmentName
    serviceBusConnectionString: serviceBusConnectionString
    appScope: [
      '${spinozaBackendAccessorsTagAccessorServiceContainerAppName}'
      '${spinozaBackendManagersCatalogManagerServiceContainerAppName}'
    ]
  }
  dependsOn:  [
    containersAppInfra
    servicebus
  ]
}


resource SignalrNegotiateContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: spinozaBackendSignalrNegotiateServiceContainerAppName
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      dapr: {
        enabled: true
        appPort: spinozaBackendSignalrNegotiatePort
        appId: spinozaBackendSignalrNegotiateServiceContainerAppName
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
        external: spinozaBackendSignalrNegotiateIsExternalIngress
        targetPort: spinozaBackendSignalrNegotiatePort
      }
    }
    template: {
      containers: [
        {
          image: '${containerRegistry}/${spinozaBackendSignalrNegotiateImage}'
          name: spinozaBackendSignalrNegotiateServiceContainerAppName
          resources: {
            cpu: 1
            memory: '2.0Gi'
          }
          env: [
            {
              name: 'AzureSignalRConnectionString'
              value: signalrKey
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        rules: [
          {
            name: 'http-rule'
            http: {
              metadata: {
                concurrentRequests: '20'
              }
            }
          }
        ]
      }
    }
  }
  dependsOn:  [
    containersAppInfra
    signalr
  ]
}


resource SpinozaBackendAccessorsTestAccessorContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: spinozaBackendAccessorsTestAccessorServiceContainerAppName
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environmentId
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
              value: 'http://localhost:80'
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
          {
            name: 'http-rule'
            http: {
              metadata: {
                concurrentRequests: '20'
              }
            }
          }
          ]
       }
    }
  }
  dependsOn:  [
    containersAppInfra
    servicebus
  ]
}

resource SpinozaBackendAccessorsQuestionAccessorContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: spinozaBackendAccessorsQuestionAccessorServiceContainerAppName
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environmentId
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
              value: 'http://localhost:80'
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
          {
            name: 'http-rule'
            http: {
              metadata: {
                concurrentRequests: '20'
              }
            }
          }
          ]
       }
    }
  }
  dependsOn:  [
    containersAppInfra
    servicebus
  ]
}


resource SpinozaBackendAccessorsTagAccessorContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: spinozaBackendAccessorsTagAccessorServiceContainerAppName
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environmentId
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
              value: 'http://localhost:80'
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
          {
            name: 'http-rule'
            http: {
              metadata: {
                concurrentRequests: '20'
              }
            }
          }
          ]
       }
    }
  }
  dependsOn:  [
    containersAppInfra
    servicebus
  ]
}


resource SpinozaBackendManagersCatalogManagerContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: spinozaBackendManagersCatalogManagerServiceContainerAppName
  tags: tags
  location: location
  properties: {
    managedEnvironmentId: environmentId
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
              value: 'http://localhost:80'
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        rules: [
          {
            name: 'http-rule'
            http: {
              metadata: {
                concurrentRequests: '20'
              }
            }
          }
        ]
      }
    }
  }
  dependsOn: [
    SpinozaBackendAccessorsTestAccessorContainerApp
    SpinozaBackendAccessorsQuestionAccessorContainerApp
    SpinozaBackendAccessorsTagAccessorContainerApp
    containersAppInfra
    servicebus
    signalr
  ]
}

output webServiceUrl string = SpinozaBackendManagersCatalogManagerContainerApp.properties.latestRevisionFqdn
