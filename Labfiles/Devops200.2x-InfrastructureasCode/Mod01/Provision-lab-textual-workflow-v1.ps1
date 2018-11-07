workflow Provision-lab-textual-workflow-v1
{   
$c = Get-AutomationConnection -Name 'AzureRunAsConnection' 
Add-AzureRmAccount -ServicePrincipal -Tenant $c.TenantID -ApplicationID $c.ApplicationID -CertificateThumbprint $c.CertificateThumbprint 
$vm1Name = Get-AutomationVariable -Name 'VM1Name' 
$vm2Name = Get-AutomationVariable -Name 'VM2Name' 
$resourceGroupName = Get-AutomationVariable -Name 'ResourceGroupName' 
$location = Get-AutomationVariable -Name 'Location' 
$username = Get-AutomationVariable -Name 'UserName' 
$password = Get-AutomationVariable -Name 'Password' 
 
$vmSize = 'Standard_A1' 

$vnetName = $resourceGroupName + '-vnet1' 
$vnetPrefix = '10.0.0.0/16' 
$subnet1Name = 'subnet1' 
$subnet1Prefix = '10.0.0.0/24' 

$avSetName = $resourceGroupName + '-avset1' 

$publisherName = 'MicrosoftWindowsServer' 
$offer = 'WindowsServer' 
$sku = '2016-Datacenter' 
$version = 'latest' 
$vmosDiskSize = 128 
 
$publicIpvm1Name = $resourceGroupName + $vm1Name + '-pip1' 
$publicIpvm2Name = $resourceGroupName + $vm2Name + '-pip1' 
 
$nic1Name = $resourceGroupName + $vm1Name + '-nic1' 
$nic2Name = $resourceGroupName + $vm2Name + '-nic1' 
 
$vm1osDiskName = $resourceGroupName + $vm1Name + 'osdisk' 
$vm2osDiskName = $resourceGroupName + $vm2Name + 'osdisk' 
 
##$resourceGroup = New-AzureRmResourceGroup -Name $resourceGroupName -Location $location 

InlineScript {

$resourceGroup = Get-AzureRmResourceGroup -Name $using:resourceGroupName -ErrorAction SilentlyContinue
if(!$resourceGroup)
{
    Write-Host "Creating resource group '$resourceGroupName' in location $location";
    New-AzureRmResourceGroup -Name $using:resourceGroupName -Location $using:location -Verbose 
}
else{
    Write-Host "Using existing resource group '$resourceGroupName'";
}
}
 
$securePassword = ConvertTo-SecureString -String $password -AsPlainText -Force 
$credentials = New-Object System.Management.Automation.PSCredential -ArgumentList $username,$securePassword 
 
$avSet = New-AzureRmAvailabilitySet -ResourceGroupName $resourceGroupName -Name $avSetName -Location $location -PlatformUpdateDomainCount 5 -PlatformFaultDomainCount 3 
 
InlineScript { 
    $subnet = New-AzureRmVirtualNetworkSubnetConfig -Name $using:subnet1Name -AddressPrefix $using:subnet1Prefix 
    $vnet = New-AzureRmVirtualNetwork -Name $using:vnetName -ResourceGroupName $using:resourceGroupName -Location $using:location -AddressPrefix $using:vnetPrefix -Subnet $using:subnet
    Set-AzureRmVirtualNetwork -VirtualNetwork $vnet
} 
 
Parallel 
 { 
  InlineScript { 
    $vnet = Get-AzureRmVirtualNetwork -Name $using:vnetName -ResourceGroupName $using:resourceGroupName 

    
    $publicIpvm1 = New-AzureRmPublicIpAddress -Name $using:publicIpvm1Name -ResourceGroupName $using:resourceGroupName -Location $using:location -AllocationMethod Dynamic 
    $nic1 = New-AzureRmNetworkInterface -Name $using:nic1Name -ResourceGroupName $using:resourceGroupName -Location $using:location -SubnetId $vNet.Subnets[0].Id -PublicIpAddressId $publicIpvm1.Id 
    $vm1 = New-AzureRmVMConfig -VMName $using:vm1Name -VMSize $using:vmSize -AvailabilitySetId $using:avSet.Id 
    
   $randomnumber1 = Get-Random -Minimum 0 -Maximum 99999999 
   $tempName1 = $using:resourceGroupName + $using:vm1Name + $randomnumber1 
   $nameAvail1 = Get-AzureRmStorageAccountNameAvailability -Name $tempName1 
   If ($nameAvail1.NameAvailable -ne $true) { 
       Do { 
           $randomNumber1 = Get-Random -Minimum 0 -Maximum 99999999 
           $tempName1 = $using:resourceGroupName + $using:vm1Name + $randomnumber1 
           $nameAvail1 = Get-AzureRmStorageAccountNameAvailability -Name $tempName1 
       } 
       Until ($nameAvail1.NameAvailable -eq $True) 
   } 
   $storageAccountName1 = $tempName1  
   $storageAccount1 = New-AzureRmStorageAccount -ResourceGroupName $using:resourceGroupName -Name $storageAccountName1 -SkuName "Standard_LRS" -Kind "Storage" -Location $using:location 
 
   $vm1 = Set-AzureRmVMOperatingSystem -VM $vm1 -Windows -ComputerName $using:vm1Name -Credential $using:credentials -ProvisionVMAgent EnableAutoUpdate 
   $vm1 = Set-AzureRmVMSourceImage -VM $vm1 -PublisherName $using:publisherName -Offer $using:offer -Skus $using:sku -Version $using:version   
   $blobPath1 = 'vhds/' + $using:vm1osDiskName + '.vhd' 
   $osDiskUri1 = $storageAccount1.PrimaryEndpoints.Blob.ToString() + $blobPath1 
   $vm1 = Set-AzureRmVMOSDisk -VM $vm1 -Name $using:vm1osDiskName -VhdUri $osDiskUri1 -CreateOption fromImage 
 
   $vm1 = Add-AzureRmVMNetworkInterface -VM $vm1 -Id $nic1.Id 
   New-AzureRmVM -ResourceGroupName $using:resourceGroupName -Location $using:location -VM $vm1 
  } 
  InlineScript { 
   $vnet = Get-AzureRmVirtualNetwork -Name $using:vnetName -ResourceGroupName $using:resourceGroupName 
   $publicIpvm2 = New-AzureRmPublicIpAddress -Name $using:publicIpvm2Name -ResourceGroupName $using:resourceGroupName -Location $using:location -AllocationMethod Dynamic 
   $nic2 = New-AzureRmNetworkInterface -Name $using:nic2Name -ResourceGroupName $using:resourceGroupName -Location $using:location -SubnetId $vNet.Subnets[0].Id -PublicIpAddressId $publicIpvm2.Id 
   $vm2 = New-AzureRmVMConfig -VMName $using:vm2Name -VMSize $using:vmSize -AvailabilitySetId $using:avSet.Id 
 
   $randomnumber2 = Get-Random -Minimum 0 -Maximum 99999999 
   $tempName2 = $using:resourceGroupName + $using:vm2Name + $randomnumber2 
   $nameAvail2 = Get-AzureRmStorageAccountNameAvailability -Name $tempName2 
   If ($nameAvail2.NameAvailable -ne $true) { 
       Do { 
           $randomNumber2 = Get-Random -Minimum 0 -Maximum 99999999 
           $tempName2 = $using:resourceGroupName + $using:vm2Name + $randomnumber2 
           $nameAvail2 = Get-AzureRmStorageAccountNameAvailability -Name $tempName2 
       } 
       Until ($nameAvail2.NameAvailable -eq $True) 
   } 
   $storageAccountName2 = $tempName2  
   $storageAccount2 = New-AzureRmStorageAccount -ResourceGroupName $using:resourceGroupName -Name $storageAccountName2 -SkuName "Standard_LRS" -Kind "Storage" -Location $using:location 
 
    $vm2 = Set-AzureRmVMOperatingSystem -VM $vm2 -Windows -ComputerName $using:vm2Name -Credential $using:credentials -ProvisionVMAgent EnableAutoUpdate 
    $vm2 = Set-AzureRmVMSourceImage -VM $vm2 -PublisherName $using:publisherName -Offer $using:offer -Skus $using:sku -Version $using:version 

   $blobPath2 = 'vhds/' + $using:vm2osDiskName + '.vhd' 
   $osDiskUri2 = $storageAccount2.PrimaryEndpoints.Blob.ToString() + $blobPath2 
   $vm2 = Set-AzureRmVMOSDisk -VM $vm2 -Name $using:vm2osDiskName -VhdUri $osDiskUri2 -CreateOption fromImage 
 
   $vm2 = Add-AzureRmVMNetworkInterface -VM $vm2 -Id $nic2.Id 
   New-AzureRmVM -ResourceGroupName $using:resourceGroupName -Location $using:location -VM $vm2 
  } 
}    
 
InlineScript { 
   $publicIplbName = $using:resourceGroupName + 'lb-pip1' 
   $feIplbConfigName = $using:resourceGroupName + '-felbipconfig' 
   $beAddressPoolConfigName = $using:resourceGroupName + '-beipapconfig' 
   $lbName = $using:resourceGroupName + 'lb' 
 
   $publicIplb = New-AzureRmPublicIpAddress -Name $publicIplbName -ResourceGroupName $using:resourceGroupName -Location $using:location -AllocationMethod Dynamic 
   $feIplbConfig = New-AzureRmLoadBalancerFrontendIpConfig -Name $feIplbConfigName -PublicIpAddress $publicIplb 
   $beIpAaddressPoolConfig = New-AzureRmLoadBalancerBackendAddressPoolConfig -Name $beAddressPoolConfigName 
   $healthProbeConfig = New-AzureRmLoadBalancerProbeConfig -Name HealthProbe -RequestPath '\' -Protocol http -Port 80 -IntervalInSeconds 15 -ProbeCount 2 
   $lbrule = New-AzureRmLoadBalancerRuleConfig -Name HTTP -FrontendIpConfiguration $feIplbConfig -BackendAddressPool $beIpAaddressPoolConfig -Probe $healthProbe -Protocol Tcp -FrontendPort 80 -BackendPort 80 
   $lb = New-AzureRmLoadBalancer -ResourceGroupName $using:resourceGroupName -Name $lbName -Location $using:location -FrontendIpConfiguration $feIplbConfig -LoadBalancingRule $lbrule -BackendAddressPool $beIpAaddressPoolConfig -Probe $healthProbeConfig    
   $nic1 = Get-AzureRmNetworkInterface -Name $using:nic1Name -ResourceGroupName $using:resourceGroupName 
   $nic1.IpConfigurations[0].LoadBalancerBackendAddressPools = $beIpAaddressPoolConfig 
   $nic2 = Get-AzureRmNetworkInterface -Name $using:nic2Name -ResourceGroupName $using:resourceGroupName 
   $nic2.IpConfigurations[0].LoadBalancerBackendAddressPools = $beIpAaddressPoolConfig 
 
   Set-AzureRmNetworkInterface -NetworkInterface $nic1 
   Set-AzureRmNetworkInterface -NetworkInterface $nic2 
  } 
} 
