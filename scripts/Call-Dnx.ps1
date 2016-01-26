#Add dnu to path
Write-Verbose "Add dnx to path for this session."
Invoke-Expression "$env:USERPROFILE\.dnx\bin\dnvm.ps1 use default"

# Temporarily, add web tools to path, if not already included.
$pathToWebTools = "$env:VS140COMNTOOLS..\IDE\Extensions\Microsoft\Web Tools\External\"
if (!$env:Path.Contains($pathToWebTools)){
	Write-Verbose "Adding '$pathToWebTools' to path for this session."
	$env:Path += ";$pathToWebTools"
}
Write-Debug "Current Path: $env:Path"

#Call dnu
Write-Verbose "Executing: dnx $args"
& dnx $args