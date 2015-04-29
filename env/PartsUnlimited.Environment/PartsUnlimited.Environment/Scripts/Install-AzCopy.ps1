Param(
  [string] $toolsPath = "$PSScriptRoot\..\tools"
)

$azcopy = "$toolsPath\azcopy.exe"

if(Test-Path $azcopy){
	Write-Output "AzCopy.exe has already been downloaded."
} else {
	$bootstrap = "$env:TEMP\azcopy_"+[System.Guid]::NewGuid()
	$output = "$bootstrap\extracted"
	$msi = "$bootstrap\MicrosoftAzureStorageTools.msi"
	
	Write-Output "Downloading AzCopy."
	Write-Output "Bootstrap directory: '$bootstrap'"

	mkdir $toolsPath -ErrorAction Ignore | Out-Null
	mkdir $bootstrap | Out-Null

	Invoke-WebRequest -Uri "http://aka.ms/downloadazcopy" -OutFile $msi
	Unblock-File $msi

	Write-Host "Extracting AzCopy"
	Start-Process msiexec -Argument "/a $msi /qb TARGETDIR=$output /quiet" -Wait

	Copy-Item "$output\Microsoft SDKs\Azure\AzCopy\*" $toolsPath -Force

	Remove-Item $bootstrap -Recurse -Force
}

# Display version of AzCopy.exe downloaded
Get-ChildItem $azcopy |% VersionInfo | Select ProductVersion,FileVersion