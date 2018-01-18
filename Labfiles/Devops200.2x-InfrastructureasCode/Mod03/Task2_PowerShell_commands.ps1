# Open this file i the PowerShell ISE. Highlight each command and hit F8 to run it as required
# You can modify the commands as needed

#cmdlet to sign in to you azure subscription
Add-AzureRmAccount

#define the variable to your template file. If you placed it in mydocuments, or some other system defined folder you can use the 'getfolderpath' method as below,
$template = [environment]::getfolderpath(“mydocuments”) +"\EmptyTemplate.json" 

#otherwise you can just drop in the path here and define the variable that way, as below
$template = "C:\Labfiles\Devops200.2x-InfrastructureasCode\Mod03" +"\EmptyTemplate.json"


# Command to deploy the ARM template
New-AzureRmResourceGroupDeployment -ResourceGroupName 'DevOpsLab1RG' -Mode Complete -TemplateFile $template -Force
