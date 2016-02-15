# Getting Started#

The following instruction is for Windows. For Linux or Mac OS X, please see
[Getting Started for Linux/OS X](GettingStartedLinuxOSX.md)

## Set up your machine ##
1. Install [Visual Studio 2015 RC](http://go.microsoft.com/fwlink/?LinkId=517106)
2. Install [Microsoft Web Platform Installer](http://www.microsoft.com/web/downloads/platform.aspx)
3. When Web Platform Installer opens, click the Add button for `Microsoft Azure SDK for .NET (VS 2015) - 2.7.1`
4. When Web Platform Installer opens, click the Add button for `Microsoft Azure PowerShell`
5. Click the Install button and follow the prompts to complete the installation

## Get the source code ##
1. Open Visual Studio 2015
2. Go to View -> Team Explorer
3. On the Connect page of the Team Explorer window, click the Clone dropdown located under the Local Git Repositories section
4. Enter the URL https://github.com/Microsoft/PartsUnlimited.git
5. If desired, change the local repository path
6. Click the Clone button

## Begin working with the Parts Unlimited solution ##
1. On the Home page of the Team Explorer window, open the solution by double-clicking PartsUnlimited.sln listed under the Solutions section.  If you do not see it listed under the Solutions section, click the Open... link and navigate the local repository folder to open PartsUnlimited.sln.
2. After opening the solution, wait for the Output window Package Manager pane to show "Restore complete" and "Total time" messages.
3. Go to Build -> Build Solution.
4. Go to Debug -> Start Debugging to launch the web application and attach the debugger.
