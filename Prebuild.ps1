# bootstrap DNVM into this session.
Write-Debug "Install DNVM into this session."
&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}

# load up the global.json so we can find the DNX version
$globalJson = Get-Content -Path $PSScriptRoot\global.json -Raw -ErrorAction Ignore | ConvertFrom-Json -ErrorAction Ignore
if($globalJson.sdk.version)
{
	Write-Debug = "Using $globalJson.sdk.version"
    $dnxVersion = $globalJson.sdk.version
}
else
{
    Write-Warning "Unable to locate global.json to determine target version.  Using 'latest'."
    $dnxVersion = "latest"
}
# install DNX.  Alias as 'default' for so projects are built with this version later.
& $env:USERPROFILE\.dnx\bin\dnvm.ps1 install $dnxVersion -Persistent -Alias default

# Add grunt to path.  This is needed for 'dnu restore' to succeed for some of the projects.
Write-Debug "Add grunt to path for this session."
$env:path += ";$env:VS140COMNTOOLS\..\IDE\Extensions\Microsoft\Web Tools\External"
#Setting npm loglevel to 'error' to avoid build failures due to package warnings.
$env:npm_config_loglevel="error"

# Run DNU restore on all project.json files in the src and test folders.
Write-Debug "Run DNU restore on all project.json files in the src and test folders"
Get-ChildItem -Path $PSScriptRoot\src -Filter project.json -Recurse -ErrorAction SilentlyContinue | Where-Object {$_.FullName -notmatch "\\node_modules\\?"} | ForEach-Object { & dnu restore $_.FullName }
Get-ChildItem -Path $PSScriptRoot\test -Filter project.json -Recurse -ErrorAction SilentlyContinue | Where-Object {$_.FullName -notmatch "\\node_modules\\?"} | ForEach-Object { & dnu restore $_.FullName }