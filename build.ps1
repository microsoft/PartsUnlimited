[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True)] [string] $BuildConfiguration,
    [Parameter(Mandatory=$True)] [string] $BuildStagingDirectory
)

$ErrorActionPreference = "Stop"

# Restore and build projects
& dotnet restore
& dotnet build .\src\PartsUnlimitedWebsite --configuration $BuildConfiguration
& dotnet build .\test\PartsUnlimited.UnitTests --configuration $BuildConfiguration

# Run tests
& dotnet test .\test\PartsUnlimited.UnitTests -xml testresults.xml

# Publish
$publishDirectory = Join-Path $BuildStagingDirectory "Publish"
$outputDirectory = Join-Path $publishDirectory "PartsUnlimited"
& dotnet publish .\src\PartsUnlimitedWebsite --framework netcoreapp1.0 --output $outputDirectory --configuration $BuildConfiguration --no-build

# Package to MSDeploy format
$manifestFile = Join-Path $publishDirectory "manifest.xml"
$sourceManifest = @'
<?xml version="1.0" encoding="utf-8"?>
<sitemanifest MSDeploy.ObjectResolver.createApp="Microsoft.Web.Deployment.CreateApplicationObjectResolver" MSDeploy.ObjectResolver.dirPath="Microsoft.Web.Depl
oyment.DirPathObjectResolver" MSDeploy.ObjectResolver.filePath="Microsoft.Web.Deployment.FilePathObjectResolver">
  <iisApp path="PATH_TO_REPLACE">
    <createApp path="PATH_TO_REPLACE" MSDeploy.path="2" isDest="AA==" MSDeploy.isDest.Type="Microsoft.Web.Deployment.DeploymentObjectBooleanAttributeValue" managedRuntimeVersion="" MSDeploy.managedRuntimeVersion="2" enable32BitAppOnWin64="" MSDeploy.enable32BitAppOnWin64="2" managedPipelineMode="" MSDeploy.managedPipelineMode="2" applicationPool="" MSDeploy.applicationPool="1" appExists="True" MSDeploy.appExists="1" MSDeploy.MSDeployLinkName="createApp" MSDeploy.MSDeployKeyAttributeName="path" />
    <contentPath path="PATH_TO_REPLACE" MSDeploy.path="2" MSDeploy.MSDeployLinkName="contentPath">
      <MSDeploy.dirPath path="PATH_TO_REPLACE" MSDeploy.MSDeployLinkName="contentPath" />
    </contentPath>
  </iisApp>
</sitemanifest>
'@
$sourceManifest = $sourceManifest.Replace('PATH_TO_REPLACE',$outputDirectory)
Set-Content -Path $manifestFile -Value $sourceManifest

$destinationFile = Join-Path $BuildStagingDirectory "partsunlimited.zip"

$regexOutputDirectory = $outputDirectory.Replace('\','\\')

Invoke-Expression "& 'C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe' --% -verb:sync -source:manifest=`'$manifestFile`' -dest:package=`'$destinationFile`' -declareParam:name='IIS Web Application Name',kind=ProviderPath,scope=IisApp,match=^$regexOutputDirectory,defaultValue='Default Web Site/site1',tags=IisApp -declareParam:name='IIS Web Application Name',kind=ProviderPath,scope=setAcl,match=^$regexOutputDirectory,tags=IisApp"