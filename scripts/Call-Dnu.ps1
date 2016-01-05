[CmdletBinding()]
param($arg1, $arg2, $arg3, $arg4, $arg5, $arg6, $arg7, $arg8, $arg9, $arg10, $arg11, $arg12, $arg13, $arg14, $arg15)

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

#Call dnu
Write-Verbose "Executing: dnu $args"
& dnu $arg1 $arg2 $arg3 $arg4 $arg5 $arg6 $arg7 $arg8 $arg9 $arg10 $arg11 $arg12 $arg13 $arg14 $arg15 