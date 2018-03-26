Param(    
    [Parameter(Mandatory=$True)][string] $subscriptionId,
    [Parameter(Mandatory=$True)][string] $resourceGroupName,
    [Parameter(Mandatory=$True)][string] $resourceGroupLocation,
    [Parameter(Mandatory=$True)][string] $keyVaultName,
    [Parameter(Mandatory=$True)][string] $clusterDns,
    [Parameter(Mandatory=$True)][string] $certName,
    [Parameter(Mandatory=$True)][string] $certPassword,
    [Parameter(Mandatory=$True)][string] $certOutputPath
    )

$certPasswordSec = ConvertTo-SecureString –String $certPassword –AsPlainText -Force
$dns = $clusterDns + "." + $resourceGroupLocation + ".cloudapp.azure.com"
$port = 19000

Write-Output (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)
$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$script = $scriptDir + "\ServiceFabricRPHelpers.psm1"
Import-Module $script

$ErrorActionPreference = "SilentlyContinue"; #This will hide errors
$sub = Select-AzureRmSubscription -SubscriptionId $subscriptionId 
$ErrorActionPreference = "Continue"; #Turning errors back on
if (!$sub)
{
	Login-AzureRmAccount
}
Select-AzureRmSubscription -SubscriptionId $subscriptionId

#create resource group
$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName 
if (!$resourceGroup)
{
    $resourceGroup = New-AzureRmResourceGroup -Name $resourceGroupName -Location $resourceGroupLocation 
}

#create keyvault
$keyVault = Get-AzureRmKeyVault -VaultName $keyVaultName -ResourceGroupName $resourceGroupName
if (!$keyVault)
{
    $keyVault = New-AzureRmKeyVault -VaultName $keyVaultName -ResourceGroupName $resourceGroupName -Location $resourceGroup.Location
    Set-AzureRmKeyVaultAccessPolicy -VaultName $keyVaultName -ResourceGroupName $resourceGroupName -EnabledForTemplateDeployment -EnabledForDeployment
}

#add self signed certificate

$secret = Get-AzureKeyVaultSecret –VaultName $keyVaultName -Name $certName
if ($secret)
{
    Write-Output ("Secret exists")
}

[System.IO.Directory]::CreateDirectory($certOutputPath)

$certFilePath = [System.IO.Path]::Combine($certOutputPath, $certName + ".pfx")
if ([System.IO.File]::Exists($certFilePath) -and !$secret)
{
    Invoke-AddCertToKeyVault -SubscriptionId $subscriptionId -ResourceGroupName $resourceGroupName -Location $resourceGroup.Location -VaultName $keyVaultName -CertificateName $certName -Password $certPassword -ExistingPfxFilePath $certFilePath -UseExistingCertificate
    $secret = Get-AzureKeyVaultSecret –VaultName $keyVaultName -Name $certName
}
elseif (![System.IO.File]::Exists($certFilePath))
{
    Invoke-AddCertToKeyVault -SubscriptionId $subscriptionId -ResourceGroupName $resourceGroupName -Location $resourceGroup.Location -VaultName $keyVaultName -Verbose -CertificateName $certName -Password $certPassword -CreateSelfSignedCertificate -DnsName $dns -OutputPath $certOutputPath

    #import in local machine trusted people
    $imported = Import-PfxCertificate -Exportable -CertStoreLocation Cert:\CurrentUser\My -FilePath $certFilePath -Password $certPasswordSec
    $secret = Get-AzureKeyVaultSecret –VaultName $keyVaultName -Name $certName
    #Write-Output $imported.Thumbprint    
}


$readCert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$readCert.Import($certFilePath, $certPassword, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]"DefaultKeySet")

$output = @{};
$output.SourceVault = $keyVault.ResourceId;
$output.CertificateURL = $secret.Id;
$output.CertificateThumbprint = $readCert.Thumbprint;

Write-Output $output


# If PowerShell was never used to administer Azure from the machine that you are using now, 
# you will need to do a little housekeeping. 
# 1. Enable PowerShell scripting by running the Set-ExecutionPolicy command. 
# For development machines, "unrestricted" policy is usually acceptable. 
# 2. Decide whether to allow diagnostic data collection from Azure PowerShell commands, 
# and run Enable-AzureRmDataCollection or Disable-AzureRmDataCollection as necessary. 
# If you are using Azure PowerShell version 0.9.8 or older, these commands are named 
# Enable-AzureDataCollection and Discable-AzureDataCollection, respectively. 
# This will avoid unnecessary prompts during template deployment. 
