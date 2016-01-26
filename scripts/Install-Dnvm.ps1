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

# Install DNVM.  Alias as 'default' for so projects are built with this version later.
& $env:USERPROFILE\.dnx\bin\dnvm.ps1 install $dnxVersion -Persistent -Alias default

