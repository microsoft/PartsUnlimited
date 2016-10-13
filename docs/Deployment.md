# Deployment
For full deployments, you can deploy the PartsUnlimited app to slots as needed. The website is setup to keep the connection strings with the slots. This means that if staging and main website slots are switched, the main website will still use the production database, while the staging slot will keep pointing to the staging database.

## How to Manually Deploy Full Environment And Publish Website  
1.	Open PartsUnlimited.sln at the root of the directory
2.	If you haven’t already done so, install PowerShell Tools for Visual Studio by following these steps:
    * Go to Tools - Extensions and Updates and search for "PowerShell"
    ![PowerShell Tools for Visual Studio](./img/PowerShellToolsVS.jpg)
    * If it is not present, then click the Online - Visual Studio Gallery section and search for "PowerShell" there and then install.
    * Restart Visual Studio to complete the installation
    * Re-open the Visual Studio project PartsUnlimited.sln
3.	Right-click on the `PartsUnlimitedEnv` folder (the deployment project root) and choose Deploy -> New…
4.	Choose subscription to deploy to.
5.	Under Resource Group choose New…
    * Enter Resource Group name
       * Name should not include periods.
    * Choose Resource group location.
6.	For Deployment template, choose `fullenvironmentsetupmerged.json`.
7.	For Deployment template parameters, choose `fullenvironmentsetupmerged.param.json`.
8.	Click OK.
9.	If prompted, edit the parameter to add any required value for the deployment template and click Save.
    * Do not fill in values for anything that has ‘auto-generated’ for its value.
    * `PartsUnlimitedHostingPlanSKU` needs to be value that supports slots or else the deployment will fail.  (I took out the Free hosting plan from available hosting plans for the full setup template, but still worth noting for
    * The SQL Server name (specified by `PartsUnlimitedServerName`) should be all lowercase
    * The Storage Account names (specified by `CdnStorageAccountName`, `CdnStorageAccountNameForDev` and `CdnStorageAccountNameForStaging`) should be all lowercase and be 3 to 24 characters in length
    * The Storage Container names (specified by `CdnStorageContainerName`, `CdnStorageContainerNameForDev` and `CdnStorageContainerNameForStaging`) should be all lowercase and be 3 to 63 characters in length

10.	If prompted fill in any values the scripts ask for.  This will only happen if you leave a non-auto-generated parameter blank.
11.	Now that the environment is setup, it is time to publish the website bits.
12.	Open PartsUnlimited.sln.
13.	Wait for package loads to complete.
14.	Build the project
15.	Right-click on `src\PartsUnlimitedWebsite` project and choose Publish…
16.	For Publish Web Wizard
    * On Profile Page choose ‘Microsoft Azure App Service’ under ‘Select a publish target’.
    * On the App Service page select the right subscription and view resource group. Open the resource group and the website created above (Website name specified by `WebsiteName` parameter name in previous steps.) This time choose a slot to publish the website to. Click OK.
    * On Connection Page, just click Next.  This should be automatically filled out when the website was chosen on the previous page.
    * On Settings Page, verify the configuration is what you want (i.e. Release – Any CPU) and click Next.
    * On Preview Page, click Publish
        * Users can click ‘Start Preview’ to verify the deployment will work, if they wish.  
17.	Wait for publish to complete and load the website.
    * The website is set up to automatically update the database shape, only if the database is empty.  No extra EF migrations commands needed for the first deployment.
18.	Website is all setup to use slot switching.  Users can publish to individual slots as they need.  Website is setup to keep the connection strings with the slots.  This means that if staging and main website slots are switched the main website still uses the production database.
