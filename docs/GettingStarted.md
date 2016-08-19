# Getting Started#

If you have a non-windows machine, you can still open the project in VS Code or other editor and go through the [CI HOL](https://github.com/Microsoft/PartsUnlimited/tree/aspnet45/docs/HOL_Continuous_Integration), but will not be able to run and debug the application locally.

## Set up your Windows machine
1. Install [Visual Studio 2013 Update 5 or 2015 Update 3](http://www.visualstudio.com)
 > Note: The remaining steps are only required if you want to do local development and debugging of the PartsUnlimited solution.  Otherwise, you can skip straight to the [Continuous Integration with Visual Studio Team Services HOL](https://github.com/Microsoft/PartsUnlimited/tree/aspnet45/docs/HOL_Continuous_Integration).
2. Install [Microsoft Web Platform Installer](http://www.microsoft.com/web/downloads/platform.aspx)
3. When Web Platform Installer opens, click the add button for:
    - Microsoft Azure SDK 2.9.1 (VS 2013 or VS 2015)
    - Microsoft Azure PowerShell (latest release)
4. Click the Install button and follow the prompts to complete the installation
5. Install [SQL Server 2012 Express](http://go.microsoft.com/fwlink/?LinkID=627336)

## Get the source code
1. Open Visual Studio 2013 or 2015
2. Go to View -> Team Explorer
3. On the Connect page of the Team Explorer window, click the Clone dropdown located under the Local Git Repositories section
4. Enter the URL `https://github.com/Microsoft/PartsUnlimited.git`
5. If desired, change the local repository path
6. Click the Clone button
7. Expand the repository and double click on the aspnet45 branch

## Begin working with the Parts Unlimited solution
1. On the Home page of the Team Explorer window, open the solution by double-clicking PartsUnlimited.sln listed under the Solutions section.  If you do not see it listed under the Solutions section, click the Open... link and navigate the local repository folder to open PartsUnlimited.sln.
2. After opening the solution, wait for the Output window Package Manager pane to show "Restore complete" and "Total time" messages.
3. Go to Build -> Build Solution.
4. Go to Debug -> Start Debugging to launch the web application and attach the debugger.