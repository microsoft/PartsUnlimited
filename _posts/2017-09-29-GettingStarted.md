---
layout: page
title:  Getting Started
category: basic
order: 0
---



The following instruction is for Windows. For Linux or Mac OS X, please see
[Getting Started for Linux/OS X](GettingStartedLinuxOSX)


## Set up your machine ##
1. Install [Visual Studio 2017](http://go.microsoft.com/fwlink/?LinkId=517106). Visual Studio Community 2017 version is available to use for free if you need.
2. During the VS2017 installation select the following components
- **ASP.NET and web development**
- **Azure development**

   If VS2017  is already installed you can add the required components by accessing ***Tools > Get Tools and Features*** from within Visual Studio.
3. Install [Azure Power Shell](https://docs.microsoft.com/en-us/powershell/azure/install-azurerm-ps?view=azurermps-4.4.0)
4. Install [NodeJS](https://nodejs.org/en/) (we recommend the 6.11.3 LTS version)
5. Then run the following commands on a command line as administrator:
```
    npm install bower -g
    npm install grunt-cli -g
```



## Get the source code ##
1. Open Visual Studio 2017
2. Go to View -> Team Explorer
3. Click the Manage Connections icon on the Team Explorer window, then under Local Git Repositories click the Clone drop-down
4. Enter the URL https://github.com/Microsoft/PartsUnlimited.git
5. If desired, change the local repository path
6. Click the Clone button

## Begin working with the Parts Unlimited solution ##
1. On the Home page of the Team Explorer window, open the solution by double-clicking PartsUnlimited.sln listed under the Solutions section.  If you do not see it listed under the Solutions section, click the Open... link and navigate the local repository folder to open PartsUnlimited.sln.
2. After opening the solution, wait for the Output window Package Manager pane to show "Restore complete" and "Total time" messages.
3. Go to Build -> Build Solution.

    > If the build fails try to clean your repository and build again. This usually happens because VS2017 downloaded an incorrect reference package before.
    > - Close Visual Studio.
    > - Open PowerShell and navigate to the repository folder, usually `C:\Users\<user>\Source\Repos\PartsUnlimited`.
    > - Execute the command `git clean -fdx`. This will clean your working directory and make sure your code is in sync with the repository.
    > - Open the solution on Visual Studio and rebuild the application again.

4. Go to Debug -> Start Debugging to launch the web application and attach the debugger.

## Next steps

- [Continuous Integration](ci)


