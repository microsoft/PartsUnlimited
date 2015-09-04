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

**1. Setup a continuous deployment with Visual Studio Release Management Online:** In this step, we will use our CI build and the Web Deploy package to set up continuous deployment in our release pipeline. 

**2. Setup an Release Agent:** In this step, you will create a release agent for use in our release pipeline. 

**3. Test the CD Trigger in Visual Studio Online:** In this step, test the Continuous Integration build (CI) build we created by changing code in the Parts Unlimited project with Visual Studio Online. 
