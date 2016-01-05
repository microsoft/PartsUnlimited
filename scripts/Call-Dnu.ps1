# Temporarily, add web tools to path, if not already included.
$pathToWebTools = "$env:VS140COMNTOOLS..\IDE\Extensions\Microsoft\Web Tools\External\"
if (!$env:Path.Contains($pathToWebTools)){
	Write-Host "Adding '$pathToWebTools' to path for this session."
	$env:Path += ";$pathToWebTools"
}
Write-Host "Current Path: $env:Path"

#Call dnu
Write-Host "Executing: dnu $args"
& dnu $args 