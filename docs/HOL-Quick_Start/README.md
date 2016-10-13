HOL - Quick Start
====================================================================================
In this lab we have an application called PartsUnlimited. We want to set up Continuous Integration (CI) and Continuous Deployment (CD) with Visual Studio Team Services (VSTS). Continuous Integration builds the app and runs unit tests whenever code is pushed to the master branch. After CI step succeeds it will trigger a deployment to a `dev` deployment slot. The `staging` slot and `production` will require an approver before the app is deployed into them. Once the approver confirms that `staging` slot is stable, the app will be deployed to the production site.
>**Note:** For more in-depth information on CI and CD please visit the following links:
* [HOL - Parts Unlimited WebSite Continuous Integration with Visual Studio Team Services](https://github.com/Microsoft/PartsUnlimited/tree/master/docs/HOL-Continuous_Integration)
* [HOL - Continuous Deployment with Release Management in Visual Studio Team Services](https://github.com/Microsoft/PartsUnlimited/tree/master/docs/HOL-Continuous_Deployment)



## Pre-requisites: ##

- An active Visual Studio Team Services account

- Project Admin rights to the Visual Studio Team Services account

- An account in Azure

- [Export/Import Build Definition](https://marketplace.visualstudio.com/items?itemName=onlyutkarsh.ExportImportBuildDefinition) extension for VSTS



## Tasks Overview: ##
**1. Import Source Code into your VSTS Account with Git:** In this step you will download the PartsUnlimited source code, and then push it to your own Visual Studio Team Services account.

**2. Setting up Service Endpoint in VSTS:** In this step you will set up Service Endpoint in VSTS for Continues Deployment be able to deploy to Azure.

**3. Continuous Integration:** In this step you will import Build definition template and configure it for your repository.

**4. Continuous Deployment:** In this step you will import Release definition template and configure it for your build definition.

**5. Export Build and Release Definitions from VSTS:** In this step you will export Build and Release Definitions into your local repository.

**6. Confirming successful deployment to Azure:** In this step you will confirm the status of your deployment in VSTS and Azure.


## Tasks
### Task 1: Import Source Code into your VSTS Account with Git ###
In order to use VSTS Build, your VSTS must contain source code for the application. For this lab we are using the VSTS Git project. The next couple of steps will allow you to add the PartUnlimited source to the Git master repository.

**Step 1.** If you haven't already, create a new team project in your Visual Studio Team Services account that uses Git for source control. Click on "New", enter project name, select "git" for "Version control" and click on "Create project" button.

![](<media/28.png>)

**Step 2.** Clone the repository to a local directory.

* Open a command line (one that supports Git) and navigate to the directory where you want to store your local repositories. For example in a Windows OS, you can create and navigate to `C:\Source\Repos`.

* Clone the repository with the following command:

		git clone https://github.com/Microsoft/PartsUnlimited.git
	> After a few seconds of downloading, all of the code should now be on your local machine.

* Move into the repository directory that was just created. In a Windows OS, you can use this command:

		cd PartsUnlimited

**Step 3.** Remove the link to GitHub.

* The Git repo you just downloaded currently has a remote called _origin_ that points to the GitHub repo.  Since we won't be using it any longer, we can delete the reference. To delete the GitHub remote, use:

		git remote remove origin

**Step 4.** Add the link to VSTS and push to your local Git repo.

* Navigate to your VSTS and click on "Code" tab. Copy and execute commands marked with `2` and `3` in your command line.

	![](<media/1.png>)

Congratulations, your code should now be in VSTS.



### Task 2: Setting up Service Endpoint in VSTS
 Deploying [ARM Templates](https://azure.microsoft.com/en-us/documentation/articles/resource-group-authoring-templates/)
to Azure from Release Management requires an organizational account or a Service Principal. MSA Accounts and certificate-based connections are not supported. 
For this HOL, you will use a Service Principal. Follow these instructions to quickly set it up: [Create Azure Service Principal for VSTS](http://blog.jstroheker.com/2016/10/11/SPNAzure/)
>**IMPORTANT:** For the following CD template to work, `Connection Name` must be `Azure For PartsUnlimited`.



### Task 3: Continuous Integration
**Step 1.** Navigate to `Export/Import Build Definition` extension's [link ](https://marketplace.visualstudio.com/items?itemName=onlyutkarsh.ExportImportBuildDefinition). Click on "Install", select VSTS instance you want to add this extension to and click "Confirm".

**Step 2.** Navigate to the "Build & Release" tab and click on "Builds". If you already have at least one build definition then skip to the next step, otherwise we are going to create one very quickly to get an import build option.

* Click on the "+ New" button, select "Empty" definition and click "Next". On the next page click "Create". This will create an empty build definition with default settings. Click on "Save" and then "OK".

	![](<media/4.png>)

* Click on "Build Definitions".
	![](<media/27.png>)

**Step 3.** Click on the ellipsis (...) button next to any build and click on "Import" option.

![](<media/5.png>)

**Step 4.** Click on "Browse" and select `dotnetcore CI HOL.json` file in `templates\build definitions` of your repository. Then click on "Import" button.

![](<media/6.png>)
>**Note:** Since the template doesn't contain any information related to this project or this repository, it will take settings from the first build definition the extension finds.  That's why this extension requires at least one build definition to be there already.

**Step 5.** If you had to define an empty build definition before, it can now be deleted by clicking on the ellipsis (...) button and selecting "Delete definition".

![](<media/7.png>)

**Step 6.** Now, we need to define triggers for this imported build definition.

1.  Click on the build definition and then click on "Edit"

	![](<media/18.png>)

2. Navigate to "Triggers" tab, check "Continuous integration (CI)" and make sure that your master branch is specified as a trigger. Click on "Save", then "OK".

	![](<media/19.png>)

**Step 7.**  At this stage it's a good idea to check that the imported build successfully builds your project. Trigger the new build with default settings by clicking on the "Queue new build..." button and then clicking on "OK".

![](<media/8.png>)
> **Note:** The build process may take a while, but there is no need to await its completion before proceeding.

Congratulations, you have imported a build definition successfully.



### Task 4: Continuous Deployment:
**Step 1.** Navigate to the "Build & Release" tab and click on "Releases". If you already have at least one release definition then skip to the next step, otherwise we are going to create one very quickly to get an import release option.

* Click on the "+ New definition" button and select "Empty" definition and click "Next". On the next page click "Create". This will create an empty release definition with default settings. Click on "Save" and "OK".  

	![](<media/9.png>)

**Step 2.** To import a release template click on "+" button and select "Import release definition".

![](<media/10.png>)

**Step 3.** Click on "Browse" and select `PartsUnlimited.json` file in `templates\release definitions` of your repository. Then click on "Import" button.

![](<media/11.png>)

**Step 4.** Let's specify the artifacts(output from VSTS Build) for our release definition. Navigate to the " Artifacts" tab and click on "Link to an artifact source". Set it up as shown in the screenshot below and click "Link".

![](<media/20.png>)

**Step 5.** Right after the import, you should see your tasks highlighted in red. For each tasks you may have to select your Azure subscription for the yellow field called "Azure RM Subscription".
> The name of your Azure subscription should be 'Azure For PartsUnlimited' like descripted at the beginning of this HOL.

![](<media/211.png>)

**Step 6.** The release definition's trigger is the key to the Continuous Deployment. To deploy on every successful build there has to be a trigger referencing the CI step completed above. Navigate to "Triggers" tab, tick "Continuous Deployment" and select the previously imported build definition.

![](<media/21.png>)

**Step 7.** Now we need to specify a deployment queue.

1. To do that click on the ellipsis (...) next to the "Dev" environment definition and select "Agent queue...".

	![](<media/12.png>)

2. Select a "Hosted" deployment queue and click "OK".

	![](<media/13.png>)

3. Repeat these steps for the other two environments.

**Step 8.** Our template cannot define approvers for your environments but it's a very good idea to have them for staging and production deployments.

1. Click on the ellipsis (...) next to the "Staging" environment definition and select "Assign approvers...".

	![](<media/14.png>)

2. Set pre-deployment and post-deployment approvers. Also tick "Send an email notification to the approver whom the approval is pending on" and click "OK".

	![](<media/15.png>)

3. Similarly, specify a pre-deployment approver for the "Production" environment. A post-deployment approver will not be required here because production is the last deployment environment in this template.

	![](<media/16.png>)

4. The "Dev" environment creates/updates the architecture in Azure for all three environments before deploying to the "dev" slot. For this to work you need to define passwords for test and production databases.

	* Click on the ellipsis (...) button next to the "Dev" environment and click on "Configure variables...".

		![](<media/22.png>)

	* Enter passwords for AdminPassword and AdminTestPassword variables.

		![](<media/23.png>)
		> **Note:** `AdminPassword` is the password for the production database. `AdminTestPassword` is the password for the test database.

	* Change the values for the first fourth parameters by adding something unique like your initials at the end of the current one, for example in my case 'jstr', then click "OK".

		![](<media/231.png>)
		> **Note:** You have to use unique values on Azure, if not you may have an deployment error because someone is already using the same values.

5. Like the previous step, you have to modify the global variales of this definition. Click on the 'Variables' section and add something unique like your initials at the end of the current one, for example in my case 'jstr', then click "OK". 
Save the release definition by clicking on "Save" and "OK".

	![](<media/232.png>)

Congratulations, you have imported a release definition successfully.


6. If you had to define an empty release definition before, then it can be now deleted by clicking on the dropdown arrow next to the empty definition and selecting "Delete".

	![](<media/17.png>)

Congratulations, you have imported a release definition successfully.

### Task 5: Export Build and Release Definitions from VSTS
Now that you have configured build and release definitions specifically for your repository in VSTS, it's a good idea to replace the given templates with your own.

**Step 1.** Navigate to the "Build" tab. Click on the ellipsis (...) button next to the build definition you would like to export and select "Export". This will trigger a download of the build definition in JSON format. Place this file in the `templates\build definitions` directory of your local repository.
![](<media/2.png>)

**Step 2.** Navigate to the "Release" tab. Click on the dropdown arrow next to the release definition you would like to export and select "Export". This will trigger a download of the release definition in JSON format. Place this file in the `templates\release definitions` directory of your local repository.
![](<media/3.png>)

**Step 3.** Commit your changes using the following commands:

			git add .

> Stages all changes for the next commit

			git commit -m "added build and release definitions"

> Creates a commit from all current staged changes.

			git push

  > Uploads commits to the remote repository.

Congratulations, now you can reuse your templates with other projects.



### Task 6: Confirming successful deployment to Azure
The changes you have just committed will trigger a CI build and a deployment to Azure.
Once the deployment to the "dev" slot is completed, the pre-approver for the "Staging" environment will receive an email notification about the pending deployment to the "staging" slot.

**Step 1.** To approve/reject the request navigate to the "Release" tab, click on ![](<media/26.png>) , optionally leave a comment and click on the "Approve" or "Reject" button.  

![](<media/24.png>)

> **Note:** The "Staging" environment also has a post-approver who must confirm that the app is stable and ready for the production environment. The pre-approver for the "Production" environment must also confirm before this final deployment.

**Step 2.** In Azure, find your App Service with the name `pudncore` and open its url (this is the production site).

![](<media/25.png>)
> **Note:** In the "Deployment slots" section you can find the `dev` and `staging` slots with their respective urls.

Congratulations on successfully setting up Continuous Integration and Continuous Deployment with VSTS.



Next steps
----------
In this lab, you have learned how to push new code to Visual Studio Team Services, setup a Git repo, create a Continuous Integration build that runs when new commits are pushed to the master branch and create a Continuous Deployment that deploys an application to environments automatically.
This allows you to receive feedback as to whether your changes contain syntactical errors or break existing tests, as well as saving time by automating the deployment process.

To learn more in-depth information about CI and CD try out these labs:

- [HOL - Parts Unlimited WebSite Continuous Integration with Visual Studio Team Services](https://github.com/Microsoft/PartsUnlimited/tree/master/docs/HOL-Continuous_Integration)
- [HOL - Continuous Deployment with Release Management in Visual Studio Team Services](https://github.com/Microsoft/PartsUnlimited/tree/master/docs/HOL-Continuous_Deployment)
