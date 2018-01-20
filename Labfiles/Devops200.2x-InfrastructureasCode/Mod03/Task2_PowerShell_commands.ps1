# Open this file in the PowerShell ISE. Highlight each command and hit F8 to run it as required
# You can modify the commands as needed

#cmdlet to sign in to you azure subscription
Add-AzureRmAccount

#define a variable to point to your template file. You can run the command in either line 9 or line 12. If you placed it in 'mydocuments' folder, or some other system defined folder you can use the 'getfolderpath' method as below,
#otherwise you can just drop in the path as in line 12 below and define the variable that way, and not run this command in line 9.
$template = [environment]::getfolderpath(“mydocuments”) +"\EmptyTemplate.json" 

#if you didn't drop your template file in a system folder just point to the folder path as below, and do not run the command in line 9
$template = "C:\Labfiles\Devops200.2x-InfrastructureasCode\Mod03" +"\EmptyTemplate.json"


# Command to deploy the ARM template
New-AzureRmResourceGroupDeployment -ResourceGroupName 'DevOpsLab1RG' -Mode Complete -TemplateFile $template -Force
