targetScope = 'subscription'

@description('Base name used to derive resource names.')
param baseName string = 'taskmgr'

@description('Azure region for all resources. Static Web Apps supports a limited set (e.g. westeurope, eastus2, centralus, westus2, eastasia).')
param location string = 'westeurope'

@description('Environment suffix, e.g. dev, test, prod.')
param environmentName string = 'dev'

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: 'rg-${baseName}-${environmentName}'
  location: location
}

module resources 'resources.bicep' = {
  scope: rg
  params: {
    baseName: baseName
    location: location
    environmentName: environmentName
  }
}

output resourceGroupName string = rg.name
output staticWebAppName string = resources.outputs.staticWebAppName
output staticWebAppHostname string = resources.outputs.staticWebAppHostname
output functionAppName string = resources.outputs.functionAppName
output appInsightsName string = resources.outputs.appInsightsName
