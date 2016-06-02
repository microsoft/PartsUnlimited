HOL - Parts Unlimited WebSite Continuous Integration with Visual Studio Team Services
====================================================================================
In this lab we have an application called PartsUnlimited. We want to set up
Visual Studio Team Services to be able continuously integrate code into the master
branch of code. This means that whenever code is committed and pushed to the
master branch, we want to ensure that it integrates into our code correctly to
get fast feedback. To do so, we are going to be setting up a Continuous Integration build (CI) that
will allow us to compile and run unit tests on our code every time a commit is
pushed to Visual Studio Team Services.

###Pre-requisites:###

-   An active Visual Studio Team Services account

-   An Visual Studio 2015 Update 2 or Visual Studio 2013 Update 5 client

-   Project Admin rights to the Visual Studio Team Services account

### Tasks Overview: ###

**1. Import Source Code into your VSTS Account:** In this step, you will connect your own Visual Studio Team Services account, download the PartsUnlimited source code, and then push it to your own Visual Studio Team Services account. There are two approaches to doing this: a) Use the Git command line, or b) Use Visual Studio. The Git command line is the cleanest and easiest approach, but it does require some familiarity with the Git command line. 

> Note: VSTS does support GitHub source code integration for use with VSTS builds, but is outside of the scope of this HOL

**2. Create Continuous Integration Build:** In this step, you will create a build definition that will be triggered every time a commit is pushed to your repository in Visual Studio Team Services. 

**3. Test the CI Trigger in Visual Studio Team Services:** In this step, test the Continuous Integration build (CI) build we created by changing code in the Parts Unlimited project with Visual Studio Team Services. 

### 1a: Import Source Code into your VSTS Account with Git

> Note: Use this to approach to use the Git command line to migrate code from GitHub to VSTS. If you use this approach, skip section 1b.

We want to push the application code to your Visual Studio Team Services account in
order to use VSTS Build.

> **Talking Point:** For this lab we are using the VSTS Git project. The next couple of steps will allow you to add the PartUnlimited source to the Git master repository.

**1.** Clone the repository to a local directory.

Create a parent **Working Directory** on your local file system. For instance, on a Windows OS you can create the following directory:

`C:\Source\Repos`

Open a command line (one that supports Git) and change to the directory you created above.

Clone the repository with the following command. You can paste in the URL if you copied it in Step 1.  In the example below, the clone will be copied into a directory named HOL. Feel free to use whatever directory name you like, or leave it blank to use the default directory name:

	git clone https://github.com/Microsoft/PartsUnlimited.git HOL

After a few seconds of downloading, all of the code should now be on your local machine.

Move into the directory that was just created.  In a Windows OS (and assuming you used HOL as the directory name), you can use this command:

	cd HOL

**2.** Remove the link to GitHub. 

The Git repo you just downloaded currently has a remote called _origin_ that points to the GitHub repo.  Since we won't be using it any longer, we can delete the reference. 

To delete the GitHub remote, use:

	git remote remove origin

**3.** Find the URL to access the VSTS Git repo

First, we need to find the URL to empty Git repository in VSTS.  If you remember your account name, and the Team Project name you created, the URL to the default Git repo is easily assembled:

	https://<account>.visualstudio.com\<project>\_git\<project>

Alternatively, you can use a web browser to browse to your account, click into your project, and click the Code tab to get to your default Git repository:

	https://<account>.visualstudio.com

Additionally, at the bottom of the web page, you will see the two commands that we will use to push the existing code to VSTS.

![](<media/findVstsRepoUrl.png>)

**4.** Add the link to VSTS and push your local Git repo

In the local directory from Step 3, use the following command to add VSTS as the Git remote named _origin_. You can either type the URL you found in Step 4, or simply copy the first command from the VSTS web page.

	git remote add origin https://<account>.visualstudio.com\<project>\_git\<project>
Now you can push the code, including history, to VSTS:

	git push -u origin --all	
Congratulations, your code should now be in VSTS!

### 1b: Import Source Code into your VSTS Account with Visual Studio

> Note: If you already migrated using 1a (above), skip this step. 

Use this approach to use the Visual Studio to migrate code from GitHub to VSTS. We want to push the application code to your Visual Studio Team Services account in order to use Build.

**1.** First, we need to open Team Explorer. Go to your **account home
page**:

	https://<account>.visualstudio.com

**2.** Connect to the VSTS account project using Visual Studio.

![](<media/25.jpg>)

> **Talking Point:** For this lab we are using the VSTS Git project. The next couple of steps will allow you to add the PartUnlimited source to the Git master repository.

**3.** Navigate to [https://github.com/Microsoft/PartsUnlimited/tree/aspnet45](https://github.com/Microsoft/PartsUnlimited/tree/aspnet45) and download the sample as a zip

> **Note:** For this lab is vitally IMPORTANT THAT YOU GET THE 4.5 BRANCH!

**4.** Create folder and save the download to this folder.

Create **Working Directory** to the following location:

`C:\Source\Repos\HOL`

**5.** Unzip the PartsUnlimited project, when unzipping be sure and “Unblock” the content or the deployment scripts won’t run

![](<media/21.jpg>)

**6.** Clone the repo of your team project to the location where you extracted the sample

Set the **Working Directory** to the following location:

`C:\Source\Repos\HOL`

![](<media/26.jpg>)

**7.** Click Open and navigate to the Parts Unlimited Project Solution in Solution Explorer

![](<media/27.jpg>)

**8.** Now we will add the source to the Git repo. Right click on the solution and click **Add to Source Control**.

![](<media/29.jpg>)

**7.** The Changes windows will appear, add in checkin text and verify the source is ready to be committed. Click on **Commit and Push**.

![](<media/30.jpg>)

**9.** Once the changes have been committed, click on the **Code** hub at the top of
the page. Verify the source is in the repo.

![](<media/31.jpg>)

**10.** Now it is time to create a local repo to work from, in the Team Explorer, click **Branches** -> Right click on **Master** -> **New Local Branch from**... 

![](<media/32.jpg>)

**11.** Add in the repo name (i.e. *HOLRepo*) and click **Create Branch**

![](<media/33.jpg>)

> **Note:** Publishing back to VSO when cloning a repo allow the build definition to see the new repo for building out the projects.

**12.** Now we need to make sure the branch is discoverable from build, click on **Publish Branch**.

![](<media/34.jpg>)

### 2. Create Continuous Integration Build

A continuous integration build will give us the ability check whether the code
we checked in can compile and will successfully pass any automated tests that we
have created against it.

**1.** Go to your **account’s homepage**: 

	https://<account>.visualstudio.com


**2.** Click **Browse** and then select your team project and click
**Navigate**.

![](<media/22.jpg>)

**3.** Once on the project’s home page, click on the **Build** hub at the top of
the page.

![](<media/23.jpg>)

**4.** Click the **green “plus” sign**, select **Visual Studio** build definition, and then click **Next**.

![](<media/24.1.jpg>)

>**Note:** As you can see, you can now do Universal Windows Apps & Xamarin Android/IOS Builds as well as Xcode builds.

**5.** After clicking the **Next** button, select **HOL Team Project**, select **HOL** Repository, select **HOLRepo** as the default branch and check **Continuous Integration** then click **Create**.

> **Note:** For this HOL, we created another agent queue (**HOL Pool**) but using the **Default or Hosted** queue in VSTS will work as well.

![](<media/24.2.jpg>)

> **Note:** We may have multiple repos and branches, so we need to select the correct Repo and Branch before we can select which Solution to build.

**6.** After clicking the **Create** button, On the **Build** tab, and click the **ellipsis** in the Build Solution pane. Select the PartsUnlimited solution file.

![](<media/24.3.jpg>)

**7.** On the **Visual Studio Build** task, now enter the following information to **MSBuild** Arguments:
    
    /p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:SkipInvalidConfigurations=true /p:PackageLocation=$(build.artifactstagingdirectory)
      
> **Note:** Add all the **MSBuild** parameters will be one after the other with spaces between them.

![](<media/24.4.jpg>)

**8.** On the **Visual Studio Build** task, we want to restore the nuget packages for the PartsUnlimited solution . Check **Restore Nuget Packages**.

![](<media/24.9.jpg>)

**9.** Since the PartUnlimited project has passing and failing tests, click **Continue on Error** checkbox. 

> **Talking Points:** If you do not want the build to fail, clicking ”**Continue On Error**” checkbox will allow the build will partially succeed which can be used in a **Release** that is part of **Continuous Delivery**, if necessary.

![](<media/24.6.jpg>)

**10.** Click on the **Trigger** tab, to make sure the **Build** fires off every time there’s a check in, check the **Continuous integration (CI)** checkbox. Also make sure the filter to include **HOLRepo** and **Batch Changes** checkbox is unchecked

![](<media/24.7.jpg>)

> **Note:** To enable Continuous integration in your project, check the **Continuous integration (CI)** checkbox. You can select which branch you wish to monitor, as well.

**11.** Select the **Copy Files** task, and input the **Contents** value
with the following:

	 *.zip
	
![](<media/24.8.jpg>)

**12.** Click **Save** and give the build definition a name (i.e.
*“HOL Build”*).

![](<media/41.jpg>)

### 3. Test the CI Trigger in Visual Studio Team Services

We will now test the **Continuous Integration build (CI)** build we created in *Task 2* by changing code in the Parts Unlimited project with Visual Studio Team Services.

**1.** Click **Code** hub and then select your your repo, **HOLRepo**. Navigate to **Controllers/HomeController.cs** in the PartsUnlimited project, then click **Edit**.

![](<media/45.jpg>)

**2.** After clicking **Edit**, add in text (i.e. *This is a test*) to the top of the constuctor of the **HomeController.cs** file. Once complete, click **Save**.

![](<media/46.jpg>)

**3.** Click **Build** hub, then click the **Queue** link. This should have triggered the build definition we previously created.

![](<media/47.jpg>)

**4.** Click on the **Build Number**, and you should get a build summary similar to this, which includes test results.

![](<media/49.jpg>)

Next steps
----------

In this lab, you learned how to push new code to Visual Studio Team Services, setup a Git repo and create a Continuous
Integration build that runs when new commits are pushed to the master branch.
This allows you to get feedback as to whether your changes made breaking syntax
changes, or if they broke one or more automated tests, or if your changes are a
okay. 

Try these labs out for next steps:

-   <a href="https://github.com/Microsoft/PartsUnlimited/blob/aspnet45/docs/HOL_Continuous_Deployment_Release_Management/">Continuous Deployment with Release Management in Visual Studio Team Services</a>
