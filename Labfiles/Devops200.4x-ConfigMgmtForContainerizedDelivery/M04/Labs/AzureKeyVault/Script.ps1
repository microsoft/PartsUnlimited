param(
 
 [Parameter(Mandatory=$False)]
 [string]
 $subscriptionId = "8791549a-6c99-40d9-8390-d316ee32d284"
 )


#login
$ErrorActionPreference = "SilentlyContinue"; #This will hide errors
$sub = Select-AzureRmSubscription -SubscriptionId $SubscriptionId 
#$ErrorActionPreference = "Continue"; #Turning errors back on
if (!$sub)
{
	Login-AzureRmAccount
}

#register resource provider
Register-AzureRmResourceProvider -ProviderNamespace "Microsoft.KeyVault" -Force

#set subscription context
Set-AzureRmContext -SubscriptionId $subscriptionId

#create a resource group, for easy deletion:
$group = Get-AzureRmResourceGroup -Name 'TestingKeyVault' -Location 'West Europe'
if (!$group)
{
    $group = New-AzureRmResourceGroup –Name 'TestingKeyVault' –Location 'West Europe'
    write "created resource group"
}

#create a key vault to test
$vaultName = 'vlt' + (get-date).Ticks.ToString()
$keyVault = New-AzureRmKeyVault -VaultName $vaultName -ResourceGroupName $group.ResourceGroupName -Location $group.Location
write "created key vault"
Start-Sleep -Seconds 2

#create a secret
$secretvalue = ConvertTo-SecureString 'SecretValue' -AsPlainText -Force
write "Defined a local secret, with value 'SecretValue'."

$secret = Set-AzureKeyVaultSecret -VaultName $keyVault.VaultName -Name 'SecretName' -SecretValue $secretvalue
write "added the secret to the vault, under the name 'SecretName'"
Start-Sleep -Seconds 2

#query secret
$queriedSecret = Get-AzureKeyVaultSecret -VaultName $keyVault.VaultName -Name $secret.Name 

write "Queried secret named 'SecretName' from the vault:"
write $queriedSecret.SecretValueText
Start-Sleep -Seconds 2
 
#cleanup
write "cleaning up..."
Remove-AzureRmResourceGroup -ResourceGroupName $group.ResourceGroupName -Force