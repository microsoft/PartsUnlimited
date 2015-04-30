#Requires -Version 3.0

Param(
  [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
  [string] $ResourceGroupName = 'PartsUnlimited',
  [switch] $UploadArtifacts,
  [string] $StorageAccountName, 
  [string] $StorageContainerName = $ResourceGroupName.ToLowerInvariant() + '-stageartifacts',
  [string] $TemplateFile = '..\Templates\DemoEnvironmentSetup.json',
  [string] $TemplateParametersFile = '..\Templates\DemoEnvironmentSetup.param.json',
  [string] $ArtifactStagingDirectory = '..\bin\Debug\Artifacts',
  [string] $AzCopyPath = '..\Tools\AzCopy.exe'
)

# Ensure AzCopy.exe is available
. $PSScriptRoot\Install-AzCopy.ps1

Set-StrictMode -Version 3
Import-Module Azure

try {
    $AzureToolsUserAgentString = New-Object -TypeName System.Net.Http.Headers.ProductInfoHeaderValue -ArgumentList 'VSAzureTools', '1.4'
    [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.UserAgents.Add($AzureToolsUserAgentString)
} catch { }

$OptionalParameters = New-Object -TypeName Hashtable
$TemplateFile = [System.IO.Path]::Combine($PSScriptRoot, $TemplateFile)
$TemplateParametersFile = [System.IO.Path]::Combine($PSScriptRoot, $TemplateParametersFile)

Function Get-TemplateParameters {
    Param(
      [string] [Parameter(Mandatory=$true)] $TemplateParametersFile
    )

    $JsonContent = Get-Content $TemplateParametersFile -Raw | ConvertFrom-Json
    $JsonParameters = $JsonContent | Get-Member -Type NoteProperty | Where-Object {$_.Name -eq "parameters"}

    if ($JsonParameters -eq $null)
    {
        $JsonParameters = $JsonContent
    }
    else
    {
        $JsonParameters = $JsonContent.parameters
    }

    return $JsonParameters
}

if ($UploadArtifacts)
{
    # Convert relative paths to absolute paths if needed
    $AzCopyPath = [System.IO.Path]::Combine($PSScriptRoot, $AzCopyPath)
    $ArtifactStagingDirectory = [System.IO.Path]::Combine($PSScriptRoot, $ArtifactStagingDirectory)

    Set-Variable ArtifactsLocationName '_artifactsLocation' -Option ReadOnly
    Set-Variable ArtifactsLocationSasTokenName '_artifactsLocationSasToken' -Option ReadOnly

    $OptionalParameters.Add($ArtifactsLocationName, $null)
    $OptionalParameters.Add($ArtifactsLocationSasTokenName, $null)

    # Parse the parameter file and update the values of artifacts location and artifacts location SAS token if they are present
    $JsonParameters = Get-TemplateParameters $TemplateParametersFile

    $JsonParameters | Get-Member -Type NoteProperty | ForEach-Object {
        $ParameterValue = $JsonParameters | Select-Object -ExpandProperty $_.Name

        if ($_.Name -eq $ArtifactsLocationName -or $_.Name -eq $ArtifactsLocationSasTokenName)
        {
            $OptionalParameters[$_.Name] = $ParameterValue.value
        }
    }

    Switch-AzureMode AzureServiceManagement
    $StorageAccountKey = (Get-AzureStorageKey -StorageAccountName $StorageAccountName).Primary
    $StorageAccountContext = New-AzureStorageContext $StorageAccountName (Get-AzureStorageKey $StorageAccountName).Primary

    # Generate the value for artifacts location if it is not provided in the parameter file
    $ArtifactsLocation = $OptionalParameters[$ArtifactsLocationName]
    if ($ArtifactsLocation -eq $null)
    {
        $ArtifactsLocation = $StorageAccountContext.BlobEndPoint + $StorageContainerName
        $OptionalParameters[$ArtifactsLocationName] = $ArtifactsLocation
    }

    # Use AzCopy to copy files from the local storage drop path to the storage account container
    & "$AzCopyPath" /Source:""$ArtifactStagingDirectory"" /Dest:$ArtifactsLocation /DestKey:$StorageAccountKey /S /Y /Z:""$env:LocalAppData\Microsoft\Azure\AzCopy\$ResourceGroupName""

    # Generate the value for artifacts location SAS token if it is not provided in the parameter file
    $ArtifactsLocationSasToken = $OptionalParameters[$ArtifactsLocationSasTokenName]
    if ($ArtifactsLocationSasToken -eq $null)
    {
       # Create a SAS token for the storage container - this gives temporary read-only access to the container (defaults to 1 hour).
       $ArtifactsLocationSasToken = New-AzureStorageContainerSASToken -Container $StorageContainerName -Context $StorageAccountContext -Permission r
       $ArtifactsLocationSasToken = ConvertTo-SecureString $ArtifactsLocationSasToken -AsPlainText -Force
       $OptionalParameters[$ArtifactsLocationSasTokenName] = $ArtifactsLocationSasToken
    }
}

# Create or update the resource group using the specified template file and template parameters file
Switch-AzureMode AzureResourceManager
New-AzureResourceGroup -Name $ResourceGroupName `
                       -Location $ResourceGroupLocation `
                       -TemplateFile $TemplateFile `
                       -TemplateParameterFile $TemplateParametersFile `
                        @OptionalParameters `
                        -Force -Verbose

$WebsiteName = (Get-AzureResource -ResourceGroupName $ResourceGroupName -ResourceType 'Microsoft.Web/sites').Name
$WebsiteLocation = (Get-AzureWebApp -ResourceGroupName $ResourceGroupName -Name $WebsiteName).Location

# Parse the parameter file and update the values of CDN Storage names if they are present
$CdnStorageAccountName = $null
$CdnStorageContainerName = $null
$CdnStorageAccountNameForDev = $null
$CdnStorageContainerNameForDev = $null
$CdnStorageAccountNameForStaging = $null
$CdnStorageContainerNameForStaging = $null

$JsonParameters = Get-TemplateParameters $TemplateParametersFile

$JsonParameters | Get-Member -Type NoteProperty | ForEach-Object {
    $ParameterValue = $JsonParameters | Select-Object -ExpandProperty $_.Name

    switch ($_.Name)
    {
        "CdnStorageAccountName"             { $CdnStorageAccountName = $ParameterValue.value }
        "CdnStorageContainerName"           { $CdnStorageContainerName = $ParameterValue.value }
        "CdnStorageAccountNameForDev"       { $CdnStorageAccountNameForDev = $ParameterValue.value }
        "CdnStorageContainerNameForDev"     { $CdnStorageContainerNameForDev = $ParameterValue.value }
        "CdnStorageAccountNameForStaging"   { $CdnStorageAccountNameForStaging = $ParameterValue.value }
        "CdnStorageContainerNameForStaging" { $CdnStorageContainerNameForStaging = $ParameterValue.value }
    }
} 

#Create Storage container needed for website.
#This will not be needed when Azure Resource Management templates support the creation of storage accounts.
Switch-AzureMode AzureServiceManagement

$StorageModule = [System.IO.Path]::Combine($PSScriptRoot, ".\New-CdnStorageContainer.psm1")
Import-Module $StorageModule -Force
$CdnAppSettingName = "CDN:Images"

#Keep track of the new storage accounts created
$createdStorageAccountNames = New-Object -TypeName System.Collections.ArrayList

if ($CdnStorageAccountName) {
    if (!(Test-AzureName -Storage $CdnStorageAccountName))
    {
        $createdStorageAccountNames.Add($CdnStorageAccountName)
    }

    $cdnUrl = New-CdnStorageContainer -StorageAccountName $CdnStorageAccountName -ContainerName $CdnStorageContainerName -Location $WebsiteLocation

    $Website = (Get-AzureWebsite -Name $WebsiteName -Slot production)
    $AppSettings = $Website.AppSettings
    $AppSettings.Add($CdnAppSettingName, $cdnUrl)
    $AppStickySettings = $Website.SlotStickyAppSettingNames
    $AppStickySettings.Add($CdnAppSettingName)
    Set-AzureWebsite -Name $WebsiteName -AppSettings $AppSettings -SlotStickyAppSettingNames $AppStickySettings
    Write-Verbose $Website.AppSettings
    Write-Verbose $Website.SlotStickyAppSettingNames
}

if ($CdnStorageAccountNameForDev) {
    if (!(Test-AzureName -Storage $CdnStorageAccountNameForDev))
    {
        $createdStorageAccountNames.Add($CdnStorageAccountNameForDev)
    }

    $cdnUrlForDev = New-CdnStorageContainer -StorageAccountName $CdnStorageAccountNameForDev -ContainerName $CdnStorageContainerNameForDev -Location $WebsiteLocation

    $AppSettings = (Get-AzureWebsite -Name $WebsiteName -Slot Dev).AppSettings
    $AppSettings.Add($CdnAppSettingName, $cdnUrlForDev)
    Set-AzureWebsite -Name $WebsiteName -Slot Dev -AppSettings $AppSettings
}

if ($CdnStorageAccountNameForStaging) {
    if (!(Test-AzureName -Storage $CdnStorageAccountNameForStaging))
    {
        $createdStorageAccountNames.Add($CdnStorageAccountNameForStaging)
    }

    $cdnUrlForStaging = New-CdnStorageContainer -StorageAccountName $CdnStorageAccountNameForStaging -ContainerName $CdnStorageContainerNameForStaging -Location $WebsiteLocation

    $AppSettings = (Get-AzureWebsite -Name $WebsiteName -Slot Staging).AppSettings
    $AppSettings.Add($CdnAppSettingName, $cdnUrlForStaging)
    Set-AzureWebsite -Name $WebsiteName -Slot Staging -AppSettings $AppSettings
}

#Move only the newly created storage accounts to the Resource Group
Switch-AzureMode AzureResourceManager

$storageAccountResourceIds = New-Object -TypeName System.Collections.ArrayList

foreach ($storageAccountName in $createdStorageAccountNames)
{
    $storageAccountResource = Get-AzureResource -ResourceType 'Microsoft.ClassicStorage/storageAccounts' | Where-Object {$_.Name -eq $storageAccountName}

    if ($storageAccountResource)
    {
        $storageAccountResourceIds.Add($storageAccountResource.ResourceId)
    }
}

if ($storageAccountResourceIds.Count -gt 0)
{
    Move-AzureResource -DestinationResourceGroupName $ResourceGroupName -ResourceId $storageAccountResourceIds.ToArray('string') -Force
}
