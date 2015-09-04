HOL - Parts Unlimited WebSite Continuous Delivery with Visual Studio Online & Release Management Online
====================================================================================
In this lab we have an application called PartsUnlimited. We want to set up
Visual Studio Online to be able continuously integrate code into the master
branch of code. This means that whenever code is committed and pushed to the
master branch, we want to ensure that it integrates into our code correctly to
get fast feedback. To do so, we are going to be setting up a Continuous Integration build (CI) that
will allow us to compile and run unit tests on our code every time a commit is
pushed to Visual Studio Online.

###Pre-requisites:###

-   An active Visual Studio Online account

-   An Visual Studio 2015 or Visual Studio 2013 Update 5 client

-   Project Admin rights to the Visual Studio Online account

### Tasks Overview: ###

**1. Setup an Release Agent:** In this step, you will create a release agent for use in our release pipeline. 

**2. Setup a continuous deployment with Visual Studio Release Management Online:** In this step, we will use our CI build and the Web Deploy package to set up continuous deployment in our release pipeline. 

**3. Test the CD Trigger in Visual Studio Online:** In this step, test the Continuous Integration build (CI) build we created by changing code in the Parts Unlimited project with Visual Studio Online. 

### 1: Setup an Release Agent 

We want setup a release agent for our Visual Studio Online account in
order to use Release Management Online.

> **Talking Point:** Currently Release Mgt does not support hosted agents. This task helps you to manually deploy and configure an agent to your test machine (this could be your personal Windows machine or a VM hosted on Azure).

**1.** First, Go to your **account home page**:

	https://<account>.visualstudio.com

**2.** Go to the control panel

**3.** Create a new pool named “HOL_Pool”

### 1: Setup a continuous deployment with Visual Studio Release Management Online 

We want setup a release agent for our Visual Studio Online account in
order to use Release Management Online.

**2.** Once on the project’s home page, click on the **Release** hub at the top of
the page.

**3.** Click the **green “plus” sign**, select **Visual Studio Build**, and then click **OK**.
