
function Get-CurrentScriptDirectory { 
	$Invocation = (Get-Variable MyInvocation -Scope 1).Value 
	Split-Path $Invocation.MyCommand.Path 
}

$invocationDir = Get-CurrentScriptDirectory # Split-Path -Parent $MyInvocation.MyCommand.Definition

$outDir = "$invocationDir\"

# Add-Type -Path "$invocationDir\System.Fabric.CSMTemplate\System.Fabric.CSMTemplate.dll"

$ErrorActionPreference = "Stop";

# get storage account name from blob url 
function Get-StorageAccountNameFromUrl($url)
{
    $url -match "^(http|https)://(\w+).blob.(\w+).(\w+).(\w+)"

    return $Matches[2];
}


## Get application ports from probs object
function Get-ApplicationPorts($probes)
{
    $inputendpoints= @(); 

    foreach($prob in $probes)
    {
        if(!($prob.Name -ieq "FabricGatewayProbe" -or $prob.Name -ieq "FabricHttpGatewayProbe"))
        {
            $prob.Properties.Port
        }
    }

    return $inputendpoints;
}


function Invoke-ServiceFabricRPClusterScaleUpgrade
{
<#
.SYNOPSIS
Scale service fabric cluster created using PORTAL. 

.DESCRIPTION
Helper command to scale service fabric cluster created using portal experience. For cluster generated using ARM templates directly , do not use this script. You should just modify your ARM template and deploy  

.PARAMETER
.INPUTS
.OUTPUTS
.EXAMPLE
.EXAMPLE
.LINK
#>
[CmdletBinding()]
param
(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    [Parameter(Mandatory=$true)]
    [string]$SubscriptionId,
    [Parameter(Mandatory=$false)]
    [bool]$PerformAction = $true
)
    Write-Warning "This script is mainly intended to help with the scale up scenario. The Script will add the new VMs and also make the necessary cluster setting changes" 
    Write-Warning "`For scale down the script does not remove the VM resources, it just makes the necessary cluster setting changes. You will need to delete the VMs via portal or ARM PS, once this srcipt finshes running"
    Write-Warning "`Please run Login-AzureRMAccount before running this script."

    Select-AzureRmSubscription -SubscriptionId $SubscriptionId

    Write-Verbose "Getting resource group information"

    $resources = Find-AzureRmResource -ResourceGroupNameContains  $ResourceGroupName 
     
    $rd = New-Object System.Fabric.CSMTemplate.ResourceDescriptor
    $rd.CreateAllowedParamValues = $false
    $rd.CreateStorageAccount = $true
    
    $RGDeploymentParams = @{};
    $vmResource = $null;
    $inputendpoints= @(); 
    $enableApplicationDiagnosticsCollect = $false;
    $applicationDiagnosticsStorageAccountName = '';
    $vmDiagnostics = $null
    $secureCluster = $false;
    $dnsName ='';
    $vmResources = @();

    foreach($resource in  $resources){
        
         Write-Verbose "Resource name : $($resource.Name)"
         Write-Verbose "Resource type : $($resource.ResourceType)"

         if($resource.ResourceId.Contains("Microsoft.ServiceFabric"))
         {
            $clusterResource = Get-AzureRmResource -ResourceId $resource.ResourceId
         }

         if(!$vmResource -and $resource.ResourceId.Contains("Microsoft.Compute/virtualMachines"))
         {
            $vmResource = Get-AzureRmResource -ResourceId $resource.ResourceId
         }

         if($resource.ResourceType.Equals("Microsoft.Compute/virtualMachines"))
         {
            $vmResources += Get-AzureRmResource -ResourceId $resource.ResourceId
         }

         if($resource.Name -ieq "Microsoft.Insights.VMDiagnosticsSettings"){
            $vmDiagnostics = Get-AzureRmResource -ResourceId $resource.ResourceId
         }

         if($resource.ResourceType -ieq "Microsoft.Network/loadBalancers")
         {
            $loadBalancerResource = Get-AzureRmResource -ResourceId $resource.ResourceId
            $inputendpointsL_ = Get-ApplicationPorts($loadBalancerResource.Properties.Probes);

            foreach($port in $inputendpointsL_) { 
                if(!$inputendpoints.Contains($port)){
                    $inputendpoints += $port;
                }
            }
         }

         
         if($resource.ResourceType -ieq "Microsoft.Network/publicIPAddresses" -and $resource.Name.EndsWith("-0"))
         {
            $primaryPublicIp = Get-AzureRmResource -ResourceId $resource.ResourceId;
            $RGDeploymentParams['dnsName'] = $primaryPublicIp.Properties.DnsSettings.DomainNameLabel;
            $RGDeploymentParams['lbIPName'] = $resource.Name.TrimEnd("-0");
         }

         if($resource.ResourceType -ieq "Microsoft.Network/loadBalancers" -and $resource.Name.EndsWith("-0"))
         {           
            $primaryLB = Get-AzureRmResource -ResourceId $resource.ResourceId;            
            $RGDeploymentParams['lbName'] = $resource.Name.TrimEnd("-0");
         }
    }

    # check if diagnostic collection is enabled 
    if($vmDiagnostics){
        $enableApplicationDiagnosticsCollect = $true;
        # $applicationDiagnosticsStorageAccountName = $vmDiagnostics.Properties.Settings.StorageAccount
        $RGDeploymentParams['applicationDiagnosticsStorageAccountName'] =  $vmDiagnostics.Properties.Settings.StorageAccount;
    }

    $enableSupportLog = $false;
    # diagnostic storage account name 
    if($clusterResource.Properties.DiagnosticsStorageAccountConfig.StorageAccountName)
    {
        # $dignosticStorageAccountName = $clusterResource.Properties.DiagnosticsStorageAccountConfig.StorageAccountName
        $RGDeploymentParams['supportLogStorageAccountName'] = $clusterResource.Properties.DiagnosticsStorageAccountConfig.StorageAccountName; 
        $enableSupportLog = $true;
    }

    # get cert thumbprints
    if($clusterResource.Properties.Certificate.Thumbprint){
        $secureCluster = $true; 
        # $certThumbprintPrimary = $clusterResource.Properties.Certificate.Thumbprint   
        $RGDeploymentParams['certificateThumbprint'] = $clusterResource.Properties.Certificate.Thumbprint;  
    }

    # get node types and node associated with them 
    $nodeTypeInfo = @{};  
    foreach($nodeType in  $clusterResource.Properties.NodeTypes){        
        $nodeTypeInfo[$nodeType.Name] = @(); 
        foreach($vmResource_ in  $vmResources){
        
            if($vmResource_.ResourceName.Contains($nodeType.Name)){

                $nodeTypeInfo[$nodeType.Name] += $vmResource_;
            }
        }
    }


    $clusterResourceLocation = $clusterResource.Location;
    $RGDeploymentParams['clusterLocation'] = $clusterResource.Location;
    $RGDeploymentParams['clusterName'] = $clusterResource.Name;
    $RGDeploymentParams['nicName'] =  "NIC-$($clusterResource.Name)"
    $RGDeploymentParams['availSetName'] = "AS-$($clusterResource.Name)"
    $RGDeploymentParams['virtualNetworkName']= "VNet-$($clusterResource.Name)"
    $RGDeploymentParams['publicIPAddressName'] =   "$($clusterResource.Name)-PubIP"

    # vhd storage acccount name 
    # cert vault info , url, and store 
    if($vmResource)
    {
       # $computeLocation = $vmResource.Location;
       $RGDeploymentParams['computeLocation'] = $vmResource.Location;

       $vhdStorageAccountName = Get-StorageAccountNameFromUrl $vmResource.Properties.StorageProfile.OsDisk.Vhd.Uri
       $RGDeploymentParams['vmStorageAccountName'] = $vhdStorageAccountName[1];

       # $adminUserName = $vmResource.Properties.OsProfile.AdminUsername;
       $RGDeploymentParams['adminusername'] = $vmResource.Properties.OsProfile.AdminUsername;
       if($vmResource.Properties.OsProfile.Secrets)
       {
           $RGDeploymentParams['sourceVaultValue'] = $vmResource.Properties.OsProfile.Secrets[0].SourceVault.Id;
           $RGDeploymentParams['certificateUrlValue'] = $vmResource.Properties.OsProfile.Secrets[0].VaultCertificates[0].CertificateUrl;
       }
    }
    
    #@# ask user for instance count input 
    $rd.NodeSettings = New-Object "System.Collections.Generic.List[System.Fabric.CSMTemplate.DefineNodeSettings]";
    $nodeTypeIndex = 0;
    
    Write-Host "Fetching all the Node types that make up your cluster"
    Write-Host "`n"
    
    foreach($nodeType in $clusterResource.Properties.NodeTypes)
    {
     #   $instanceCount = Read-Host "Current instance count:$($clusterResource.Properties.ExpectedVmResources[$nodeTypeIndex].VMInstanceCount) `Please input new instance count for $($nodeType.Name)";
      $instanceCount = Read-Host "Node Type : $($nodeType.Name)   Current VM instance count : $($clusterResource.Properties.ExpectedVmResources[$nodeTypeIndex].VMInstanceCount) `nProvide the new instance count for this node type, Specify the same instance count as current if you do not want to scale up or down ";
  
        if($nodeType.IsPrimary){
   #         while($instanceCount -lt $clusterResource.Properties.ExpectedVmResources[$nodeTypeIndex].VMInstanceCount){
                while($instanceCount -lt 5){ 
               Write-Warning "Primary instance count cannot go below $($clusterResource.Properties.ExpectedVmResources[$nodeTypeIndex].VMInstanceCount) instance count"
               $instanceCount = Read-Host "Current instance count:$($clusterResource.Properties.ExpectedVmResources[$nodeTypeIndex].VMInstanceCount) `Please input new instance count for $($nodeType.Name)";
            }
        }
        
        $nodeTypeVmSize = $nodeTypeInfo[$nodeType.Name][$nodeTypeInfo[$nodeType.Name].Count -1].Properties.HardwareProfile.VmSize;

        $placementPropertyNames = @();
        $placementPropertyValues = @();

        # Ref : converting .NET custom object into hash table 
        # http://blogs.msdn.com/b/timid/archive/2013/03/05/converting-pscustomobject-to-from-hashtables.aspx
        foreach ($myPsObject in $nodeType.PlacementProperties) { 
            $myPsObject | Get-Member -MemberType *Property | % { 
                $placementPropertyNames += $_.name
                $placementPropertyValues +=$myPsObject.($_.name)               
            }            
        }

        if($placementPropertyNames.Count -eq 0 -or $placementPropertyValues.Count -eq 0)
        {
            $placementPropertyNames = $null
            $placementPropertyValues = $null
        }

        $nodeSetting = New-Object System.Fabric.CSMTemplate.DefineNodeSettings $clusterResource.Properties.ExpectedVmResources[$nodeTypeIndex].Name , $clusterResource.Properties.ExpectedVmResources[$nodeTypeIndex].Name, $instanceCount, $placementPropertyNames, $placementPropertyValues, $false, $nodeTypeVmSize;

        $rd.NodeSettings.Add($nodeSetting);
        
        $nodeTypeIndex++;
    }

    
    $rd.ClientCertificateThumbprints = New-Object "System.Collections.Generic.List[System.Fabric.CSMTemplate.Model.ClientCertificateThumbprint]"
    $rd.ClientCertificateCommonNames  = New-Object "System.Collections.Generic.List[System.Fabric.CSMTemplate.Model.ClientCertificateCommonName]"
   
    # get client commonnames    
    if($clusterResource.Properties.ClientCertificateCommonNames){
        foreach($clientCommonName in $settings.ClientCommonNames){
         $clientCertCommonNames.Add([System.Fabric.CSMTemplate.ClusterResourceTemplateHelper]::GetClientCertificateCommonName($clientCommonName.CertificateIssuerThumbprint, $clientCommonName.CertificateCommonName, $clientCommonName.IsAdmin)); 
        }
    }
    
    # get client certificates   
    if($clusterResource.Properties.ClientCertificateThumbprints){
        foreach($clientThumbprint in $clusterResource.Properties.ClientCertificateThumbprints){
             $rd.ClientCertificateThumbprints.Add([System.Fabric.CSMTemplate.ClusterResourceTemplateHelper]::GetClientCertificateThumbprint($clientThumbprint.CertificateThumbprint, $clientThumbprint.IsAdmin)); 
        }
    }
    
    # generate new template 

    $templateFilePath = "$($env:TEMP)\$([Guid]::NewGuid()).json"
    
    $rd.EnableApplicationDiagnosticsCollection  = $enableApplicationDiagnosticsCollect
    $rd.EnableSupportLogCollection = $enableSupportLog

    $rd.InputEndpoints =  New-Object "System.Collections.Generic.List[System.Int32]"

    $inputEndpoints | % { $rd.InputEndpoints.Add([int]$_) }
 
    $rd.UseProtectedAccountKeyForSupportLogCollection = $true;
    $rd.SecureUserCluster = $secureCluster;
    #### fabric settings is missing in this #### 
    
    $resultTemplatePath = [System.Fabric.CSMTemplate.ClusterResourceTemplateHelper]::GenerateCSMTemplateFile($rd, $templateFilePath);

    $rd = $null;

    Write-Verbose "Template file : $templateFilePath "
    
    if(!$PerformAction)
    {
        Write-Host "Scale action not performed";

        return;
    }
    $now = [DateTime]::Now; 
    $deploymentName = "Deployment-{0:yyyy-MM-dd-HH-mm-ss}" -f $now
    $RGDeploymentParams['DeploymentName'] = $deploymentName;
    $RGDeploymentParams['TemplateFile'] = $templateFilePath;
    $RGDeploymentParams['ResourceGroupName'] = $ResourceGroupName;
    
    Write-Verbose "$($RGDeploymentParams | ConvertTo-Json)"

    Write-Host ("Deployment started at {0:O}" -f $now)
    
    $deploymentResult = New-AzureRmResourceGroupDeployment @RGDeploymentParams -WarningAction SilentlyContinue -Verbose
    
    Write-Host ("Deployment completed at {0:O}" -f [DateTime]::Now)

     if($deploymentResult.ProvisioningState -ieq 'Failed'){ 
        Write-Warning "Deployment Failed - review the event log for details"       
    }

    Remove-Item $templateFilePath -Force
}

function Invoke-AddCertToKeyVault
{
<#
.SYNOPSIS
Upload certificate to Azure KeyVault

.DESCRIPTION
This command takes an existing pfx or creates a new self-signed certificate and uploads it as a secret to Azure KeyVault. The output of this command should be used during creation of secure cluster
through portal or for adding new certificates on VMs provisioned by Compute Resource Provider

.PARAMETER
.PARAMETER
.INPUTS
.OUTPUTS
.EXAMPLE
.EXAMPLE
.LINK
#>

[CmdletBinding()]
param(
  [Parameter(Mandatory=$true)]
  [string] $SubscriptionId,

  [Parameter(Mandatory=$true)]
  [string] $ResourceGroupName,

  [Parameter(Mandatory=$true)]
  [string] $Location,

  [Parameter(Mandatory=$true)]
  [string] $VaultName,

  [Parameter(Mandatory=$true)]
  [string] $CertificateName,
   
  [Parameter(Mandatory=$true)]
  [string] $Password,   

  [Parameter(Mandatory=$true, ParameterSetName="CreateNewCertificate")]
  [switch] $CreateSelfSignedCertificate,

  [Parameter(Mandatory=$true, ParameterSetName="CreateNewCertificate")]
  [string] $DnsName,

  [Parameter(Mandatory=$true, ParameterSetName="CreateNewCertificate")]
  [string] $OutputPath,

  [Parameter(Mandatory=$true, ParameterSetName="UseExistingCertificate")]
  [switch] $UseExistingCertificate,

  [Parameter(Mandatory=$true, ParameterSetName="UseExistingCertificate")] 
  [string] $ExistingPfxFilePath
)

$ErrorActionPreference = 'Stop'

Write-Host "Switching context to SubscriptionId $SubscriptionId"
Set-AzureRmContext -SubscriptionId $SubscriptionId | Out-Null

# New-AzureRmResourceGroup is idempotent as long as the location matches
Write-Host "Ensuring ResourceGroup $ResourceGroupName in $Location"
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location -Force | Out-Null
$resourceId = $null

try
{
    $existingKeyVault = Get-AzureRmKeyVault -VaultName $VaultName -ResourceGroupName $ResourceGroupName
    $resourceId = $existingKeyVault.ResourceId

    Write-Host "Using existing valut $VaultName in $($existingKeyVault.Location)"
}
catch
{
}

if(!$existingKeyVault)
{
    Write-Host "Creating new vault $VaultName in $location"
    $newKeyVault = New-AzureRmKeyVault -VaultName $VaultName -ResourceGroupName $ResourceGroupName -Location $Location -EnabledForDeployment
    $resourceId = $newKeyVault.ResourceId
}

if($CreateSelfSignedCertificate)
{
    $securePassword = ConvertTo-SecureString -String $password -AsPlainText -Force

    $NewPfxFilePath = Join-Path $OutputPath $($CertificateName+".pfx")

    Write-Host "Creating new self signed certificate at $NewPfxFilePath"
    New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My -DnsName $DnsName -Provider 'Microsoft Enhanced Cryptographic Provider v1.0' | Export-PfxCertificate -FilePath $NewPfxFilePath -Password $securePassword | Out-Null
    
    $ExistingPfxFilePath = $NewPfxFilePath
}

Write-Host "Reading pfx file from $ExistingPfxFilePath"
$cert = new-object System.Security.Cryptography.X509Certificates.X509Certificate2 $ExistingPfxFilePath, $Password

$bytes = [System.IO.File]::ReadAllBytes($ExistingPfxFilePath)
$base64 = [System.Convert]::ToBase64String($bytes)

$jsonBlob = @{
   data = $base64
   dataType = 'pfx'
   password = $Password
   } | ConvertTo-Json

    $contentbytes = [System.Text.Encoding]::UTF8.GetBytes($jsonBlob)
    $content = [System.Convert]::ToBase64String($contentbytes)

    $secretValue = ConvertTo-SecureString -String $content -AsPlainText -Force

Write-Host "Writing secret to $CertificateName in vault $VaultName"
$secret = Set-AzureKeyVaultSecret -VaultName $VaultName -Name $CertificateName -SecretValue $secretValue

$output = @{};
$output.SourceVault = $resourceId;
$output.CertificateURL = $secret.Id;
$output.CertificateThumbprint = $cert.Thumbprint;

return $output;
}

Export-ModuleMember -Function Invoke-ServiceFabricRPClusterScaleUpgrade
Export-ModuleMember -Function Invoke-AddCertToKeyVault