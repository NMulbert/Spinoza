param appname string = 'testapp'
param environment string = 'preprod'
param region string = 'westeurope'

targetScope = 'subscription'

//Create the resource group.
resource sa 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: 'rg-${appname}-${environment}'
  location: region
}

//Run the storage module, setting scope to the resource group we just created.
module res './storage_account.bicep' = {
  name: 'resourceDeploy'
  params: {
    appname: appname
    envtype: environment
  }
  scope: resourceGroup(sa.name)
}

module laai './log_insight.bicep'={
  name: 'resourceDeploy2'
  params:{}
  scope: resourceGroup(sa.name)
}

module env './env.bicep'= {
  name:'resourceDeploy3'
  params:{
    insightInstrumentationKey:laai.outputs.insightInstrumentationKey
    logcustomerId:laai.outputs.logCustomerId
    logprimarySharedKey:laai.outputs.logPrimarySharedKey
  }
  scope:resourceGroup(sa.name)
  dependsOn:[
    laai
  ]

}

//The below both feel a bit dirty manually building the url. 
//Please let me know if there is a better way to do this.

//Create the full url for our account download SAS.
output blobDownloadSAS string = '${res.outputs.blobEndpoint}/?${res.outputs.allBlobDownloadSAS}'

//Create the full url for our container upload SAS.
output myContainerUploadSAS string = '${res.outputs.myContainerBlobEndpoint}?${res.outputs.myContainerUploadSAS}'
