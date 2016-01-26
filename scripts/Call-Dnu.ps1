#Add dnu to path
Write-Verbose "Add dnu to path for this session."
Invoke-Expression "$env:USERPROFILE\.dnx\bin\dnvm.ps1 use default"

# Temporarily, add web tools to path, if not already included.
$pathToWebTools = "$env:VS140COMNTOOLS..\IDE\Extensions\Microsoft\Web Tools\External\"
if (!$env:Path.Contains($pathToWebTools)){
	Write-Verbose "Adding '$pathToWebTools' to path for this session."
	$env:Path += ";$pathToWebTools"
}
Write-Debug "Current Path: $env:Path"

#Setting npm loglevel to 'error' to avoid build failures due to package warnings.
Write-Verbose "Setting npm log level to 'error'"
$env:npm_config_loglevel="error"

#Call dnu
Write-Verbose "Executing: dnu $args"
& dnu $args