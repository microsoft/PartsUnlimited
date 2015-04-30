#
# New_CdnStorageContainer.psm1
#
Function New-CdnStorageContainer{

Param(
  [Parameter(Mandatory=$true)][string] $StorageAccountName,
  [string] $ContainerName = 'cdn',
  [Parameter(Mandatory=$true)][string] $Location
)

	Switch-AzureMode AzureServiceManagement

	#Create storage account if needed
	if (!(Test-AzureName -Storage $StorageAccountName)) {
		Write-Verbose "Creating new storage account with name $StorageAccountName"
		$storageAccount = New-AzureStorageAccount -StorageAccountName $StorageAccountName -Location $Location -Verbose
		if ($storageAccount)
		{
			Write-Verbose "Created $StorageAccountName storage account in $Location location"
		}
		else
		{
			throw "Failed to create a Microsoft Azure storage account."
		}
	}

	#Set current storage account for subsequents calls
	if ($StorageAccountName) {
		$subscriptionId = (Get-AzureSubscription -Current).SubscriptionId
		Set-AzureSubscription -SubscriptionId $subscriptionId -CurrentStorageAccountName $StorageAccountName
	}

	#Check to see if container exists.
	if (!(Get-AzureStorageContainer | Where-Object {$_.Name -eq $ContainerName})) {
		Write-Verbose "Creating a new storage container named '$ContainerName'"
		$storageContainer = New-AzureStorageContainer -Name $ContainerName
		Write-Verbose "Created a new storage container named '$ContainerName' already exists in the account '$StorageAccountName'"
	} else {
		Write-Verbose "A storage container named '$ContainerName' already exists in the account '$StorageAccountName'"
	}

	#Set container to all for Blob Read.  This is needed to execute the scripts
	if ((Get-AzureStorageContainerAcl -Container $ContainerName).PublicAccess -eq 'Off') {
		Write-Verbose "Setting Permissions for $ContainerName to 'Blob'"
		Set-AzureStorageContainerAcl -Name $ContainerName -Permission Blob
	}

	#Return the url for the cdn storage endpoint
	$url = [string]::Concat((Get-AzureStorageContainer -Name $ContainerName).Context.BlobEndPoint, $ContainerName)
	Write-Verbose "Blob endpoint for $ContainerName is $url"

	return $url
}
