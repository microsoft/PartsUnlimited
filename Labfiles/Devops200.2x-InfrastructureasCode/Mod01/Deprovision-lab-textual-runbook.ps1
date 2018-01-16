 $c = Get-AutomationConnection -Name 'AzureRunAsConnection'  
 Add-AzureRmAccount -ServicePrincipal -Tenant $c.TenantID -ApplicationID $c.ApplicationID -CertificateThumbprint $c.CertificateThumbprint 
 $resourceGroupName = Get-AutomationVariable -Name 'ResourceGroupName'  
 Remove-AzureRmResourceGroup -Name $resourceGroupName -Force 