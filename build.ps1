[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True)] [string] $BuildConfiguration
)

#Install dnvm
#& scripts/Install-Dnvm.ps1

# Restore and build projects
& scripts/Call-Dnu.ps1 restore .\src
& scripts/Call-Dnu.ps1 build .\src\PartsUnlimitedWebsite --configuration $BuildConfiguration 

& scripts/Call-Dnu.ps1 restore .\test
& scripts/Call-Dnu.ps1 build .\test\PartsUnlimited.UnitTests --configuration $BuildConfiguration 

# Run tests
& scripts/Call-Dnx.ps1 -p .\test\PartsUnlimited.UnitTests test -xml testresults.xml
