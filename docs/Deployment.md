#Deployment#
For full deployments, you can deploy the PartsUnlimited app to slots as needed. The website is setup to keep the connection strings with the slots. This means that if staging and main website slots are switched, the main website will still use the production database, while the staging slot will keep pointing to the staging database.

##How to Deploy Demo/Simple Environment and Publish Website##

1.	Create a storage account (or use an existing one) where deployment artifacts can be uploaded. The Storage account must be on the same subscription as the one the website will be deployed to.
1.	Open PartsUnlimited.sln at the root of the directory
1.	If you haven’t already done so, install PowerShell Tools for Visual Studio by following these steps:
    * Go to Tools - Extensions and Updates and search for "PowerShell"
    ![PowerShell Tools for Visual Studio](./img/PowerShellToolsVS.jpg)
    * If it is not present, then click the Online - Visual Studio Gallery section and search for "PowerShell" there and then install.
    * Restart Visual Studio to complete the installation
    * Re-open the Visual Studio project PartsUnlimited.sln
1.	Right-click on the PartsUnlimitedEnv folder (the deployment project root) and choose Deploy -> New Deployment…
1.	Choose the subscription to which you want to deploy
1.	Under Resource Group select New…
    * Enter a Resource Group name 
        * Name should not include periods
        * Choose Resource group location.
1.	For Deployment template, choose demoenvironmentsetup.json.
1.	For Deployment template parameters, choose demoenvironmentsetup.param.json.
1.	Click Deploy.
1.	If prompted, edit the parameter to add any required value for the deployment template.
    * Do not fill in values for anything that has ‘auto-generated’ for its value.
1.	If prompted fill in any values the scripts ask for.  This will only happen if you leave a non-auto-generated parameter blank.
    * The WebsiteName must be globally unique and have only letters and numbers
    * The SQL Server name (specified by PartsUnlimitedServerName) should be all lowercase
    * The Storage Account name (specified by CdnStorageAccountName) should be all lowercase and be 3 to 24 characters in length
    * The Storage Container name (specified by CdnStorageContainerName) should be all lowercase and be 3 to 63 characters in length
1.	Now that the environment is setup, it is time to publish the website bits.
1.	Open PartsUnlimited.sln.
1.	Wait for package loads to complete. 
1.	Build the project
1.	Right-click on src\PartsUnlimited project and choose Publish…
1.	For Publish Web Wizard
    * On Profile Page
        * Choose ‘Microsoft Azure Web Apps’ under ‘Select a publish target’.  Select the website created above.  (Website name specified by WebsiteName parameter name in picture above.)
        * Click Next.
    * On Connection Page, just click Next.  This should be automatically filled out when the website was chosen on the previous page.
    * On Settings Page, verify the configuration is what you want (i.e. Release – Any CPU).
    * On Preview Page.  Click Publish
        * Users can click ‘Start Preview’ to verify the deployment will work, if they wish.  
1.	Wait for publish to complete and load the website.
    * The website is set up to automatically update the database shape, only if the database is empty.  No extra EF migrations commands needed for the first deployment.

##How to Deploy Demo/Full Environment And Publish Website##
1.	Create a storage account (or use an existing one) where deployment artifacts can be uploaded.
    * The Storage account must be on the same subscription as the one the website will be deployed to.
1.	Open PartsUnlimited.sln at the root of the directory
1.	If you haven’t already done so, install PowerShell Tools for Visual Studio by following these steps:
    * Go to Tools - Extensions and Updates and search for "PowerShell"
    ![PowerShell Tools for Visual Studio](./img/PowerShellToolsVS.jpg)
    * If it is not present, then click the Online - Visual Studio Gallery section and search for "PowerShell" there and then install.
    * Restart Visual Studio to complete the installation
    * Re-open the Visual Studio project PartsUnlimited.sln
1.	Right-click on the PartsUnlimitedEnv folder (the deployment project root) and choose Deploy -> New Deployment…
1.	Choose subscription to deploy to.
1.	Under Resource Group choose New…
    * Enter Resource Group name 
       * Name should not include periods.
        * Choose Resource group location.
1.	For Deployment template, choose fullenvironmentsetup.json.
1.	For Deployment template parameters, choose fullenvironmentsetup.param.json.
1.	Click Deploy.
1.	If prompted, edit the parameter to add any required value for the deployment template.
a.	Do not fill in values for anything that has ‘auto-generated’ for its value.
b.	PartsUnlimitedHostingPlanSKU needs to be value that supports slots or else the deployment will fail.  (I took out the Free hosting plan from available hosting plans for the full setup template, but still worth noting for
    * The SQL Server name (specified by PartsUnlimitedServerName) should be all lowercase
    * The Storage Account names (specified by CdnStorageAccountName, CdnStorageAccountNameForDev and CdnStorageAccountNameForStaging) should be all lowercase and be 3 to 24 characters in length
    * The Storage Container names (specified by CdnStorageContainerName, CdnStorageContainerNameForDev and CdnStorageContainerNameForStaging) should be all lowercase and be 3 to 63 characters in length
1.	If prompted fill in any values the scripts ask for.  This will only happen if you leave a non-auto-generated parameter blank.
1.	Now that the environment is setup, it is time to publish the website bits.
1.	Open PartsUnlimited.sln.
1.	Wait for package loads to complete. 
1.	Build the project
1.	Right-click on src\PartsUnlimited project and choose Publish…
1.	For Publish Web Wizard
    * On Profile Page
        * Choose ‘Microsoft Azure Web Apps’ under ‘Select a publish target’.  Select the website created above.  (Website name specified by WebsiteName parameter name in picture above.)  This time choose the ‘WebsiteName(slotname)’ website to publish to a slot for the website.
        * Click Next.
    * On Connection Page, just click Next.  This should be automatically filled out when the website was chosen on the previous page.
    * On Settings Page, verify the configuration is what you want (i.e. Release – Any CPU).
    * On Preview Page.  Click Publish
        * Users can click ‘Start Preview’ to verify the deployment will work, if they wish.  
1.	Wait for publish to complete and load the website.
    * The website is set up to automatically update the database shape, only if the database is empty.  No extra EF migrations commands needed for the first deployment.
1.	Website is all setup to use slot switching.  Users can publish to individual slots as they need.  Website is setup to keep the connection strings with the slots.  This means that if staging and main website slots are switched the main website still uses the production database.
