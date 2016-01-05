# Temporarily, add web tools to path, if not already included.
$pathToWebTools = "$env:VS140COMNTOOLS..\IDE\Extensions\Microsoft\Web Tools\External\"
if (!$env:Path.Contains($pathToWebTools)){
	Write-Debug "Adding '$pathToWebTools' to path for this session."
	$env:Path += ";$pathToWebTools"
}

#Call dnu
Write-Debug "Executing: dnu $args"
& dnu $args 