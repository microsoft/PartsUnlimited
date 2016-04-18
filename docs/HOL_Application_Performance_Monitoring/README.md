#Application Performance Monitoring - PartsUnlimited

The DevOps team has noticed that the recommendations page is slow to load and shows performance spikes in Application Insights telemetry. By viewing the details of performance monitoring through Application Insights, we will be able to drill down to the code that has affected the slow performance of the web application and fix the code.

In this lab, you will learn how to set up Application Insights telemetry, and drill down into performance monitoring data through Application Insights in the new Azure Portal.

*Note: This hands-on lab is designed to use code from the aspnet45 branch.*

**Prerequisites**

- Visual Studio 2013 or higher

- PartsUnlimited website deployed to a Microsoft Azure Web App (see [link](https://github.com/Microsoft/PartsUnlimited/blob/aspnet45/docs/Deployment.md))

- Application Insights created in the same Azure Resource Group as the PartsUnlimited website

- Continuous Integration build with deployment to the Azure web app

**Tasks Overview**

1. Set up Application Insights for PartsUnlimited

2. Using Application Performance Monitoring to resolve performance issues

###Task 1: Set up Application Insights for PartsUnlimited
**Step 1.** In an Internet browser, navigate to <http://portal.azure.com> and
sign in with your credentials.

![](<media/prereq-step1.png>)

**Step 2.** Click on the “Browse All” tile on the left column, select
“Application Insights,” and click on the name of the telemetry that was created when you deployed the resource group using the Deployment template in the PartsUnlimited solution. The name of the telemetry should match the name of the PartsUnlimited name plus "Insights."

![](<media/prereq-step2.png>)

**Step 3.** In the PartsUnlimited Application Insights telemetry, click on the "Settings" tile and then "Properties." You will want to keep this browser window open with the details of the telemetry name, location, instrumentation key, and subscription information when we attach it to the PartsUnlimited website. 

![](<media/prereq-step3.png>)

**Step 4.** To receive data, we will need to attach the PartsUnlimited Application Insights telemetry to the PartsUnlimited project in Visual Studio. In Visual Studio, open the PartsUnlimited solution. 

![](<media/prereq-step4.png>)

**Step 5.** In the PartsUnlimited solution, right-click on the PartsUnlimitedWebsite project and select "Add Application Insights telemetry."

![](<media/prereq-step5.png>)

**Step 6.** In the "Add Application Insights to Project" window, click on the "Use different account" hyperlink.

![](<media/prereq-step6.png>)

**Step 7.** Since we have the subscription and resource information for our PartsUnlimited Application Insights telemetry, click on the button, "Use advanced mode." 

![](<media/prereq-step7.png>)

**Step 8.** Fill in the subscription ID, resource group, Application Insights resource name, and Instrumentation key found in the new portal from step 3. Then press the "Add Application Insights to Project" button to add the telemetry in the project. 

![](<media/prereq-step8.png>)


**Step 9.** Back in the Azure Portal, in the PartsUnlimited Application Insights telemetry, in the Application Health panel, click on the "Learn how to collect server response time data" overlay. Before Application Insights receives any information, we need to add script code to monitor each web page in the PartsUnlimited project. 

![](<media/prereq-step9.png>)

**Step 10.** In the Quickstart panel to add Application Insights to your project, click on "Get code to monitor my web pages" and copy the end-user usage analytics code. 

![](<media/prereq-step10.png>)


**Step 11.** Back in Visual Studio, in the PartsUnlimitedWebsite project, in Views/Shared/_Layout.cshtml, paste the snippet of code you copied for the end-user usage analytics code from the Azure portal before the rest of the scripts between the head tag. 

![](<media/prereq-step11.png>)

**Step 12.** In the "Changes" tile in Team Explorer, commit the changes of adding Application Insights (adding ApplicationInsights.config, editing the publish profile, _Layout.cshtml, packages.config and web.config) and sync to the repo which should kick off an automated build and deploy to the website. 

![](<media/prereq-step12a.png>)

![](<media/prereq-step12b.png>)

**Step 13.** Lastly, in the Azure portal, navigate to the PartsUnlimited Azure web app and click the "Tools" tile. We need to add in application performance monitoring for the web app now that it is connected to Application Insights. 

![](<media/prereq-step13.png>)

**Step 14.** In the tools panel, click on "Performance Monitoring," then the "Click here to collect insights into the performance of your .NET applications" overlay. Select the Application Insights telemetry used in the previous steps for the Application Insights provider. 

![](<media/prereq-step14.png>)

**Step 15.** Once you have selected the Application Insights provider, you do not need to add a New Relic provider, so you may just click on the OK button to add the Application Insights telemetry as a provider for peformance monitoring. 

![](<media/prereq-step15.png>)

Now that you have connected the PartsUnlimited Application Insights telemetry to your PartsUnlimited web app and enabled performance monitoring, you are ready to continue the lab. 

###Task 2: Using Application Performance Monitoring to resolve performance issues

**Step 1.** In an Internet browser, navigate to the PartsUnlimited website that you previously deployed and go to the Recommendations page, such as [http://partsunlimited.azurewebsites.net/home/recommendations](http://partsunlimited.azurewebsites.net/home/recommendations). 

![](<media/step1.png>)

**Step 2.** Click on the “Browse All” tile on the left column, select
“Application Insights,” and click on the name of your Application Insights
telemetry for your web app.

![](<media/step2.png>)

**Step 3.** After selecting the Application Insights telemetry for your web app,
scroll down and select the “Performance” tile to view performance monitoring
information.

![](<media/step3.png>)

**Step 4.** In the performance tile of the Application Insights telemetry, note
the timeline. The timeline data may not show up immediately, so you will want to wait for a few minutes for the telemetry to collect performance data. 

![](<media/step4.png>)

**Step 5.** Once data shows in the timeline, view the operations listed under the **Average
of server response time by operation name** section under the timeline. Click on the top operation in the list referring to the recommendations page to view details of that operation.

**Step 6.** Drill down into the method that is affecting the slow performance. We now know where the slow performance is being caused in our code. 

**Step 7.** In Visual Studio, open the **PartsUnlimited.sln**.

![](<media/step7.png>)

**Step 8.** Find the Recomendations method in **HomeController.cs** that is causing slow performance. At the top of the HomeController class, notice that the public int roco_count is set to 1000. Change that value to be 1. 

![](<media/step8.png>)

**Step 9.** Save the changes and commit the changes on the master branch.

![](<media/step9.png>)
 

**Step 10.** Press the "Sync" button to push the changes up to the repo and
kick off a build automatically.

![](<media/step10.png>)

**Step 11.** Now that our changes have deployed to the website, open up a new incognito browser window (to prevent caching) and return to the recommendations page (http://partsunlimited.azurewebsites.net/home/recommendations). 

![](<media/step1.png>) 

**Step 12.** Return to the Application Insights performance monitoring view in the Azure Preview Portal and refresh the page. 

![](<media/step12.png>) 

In this lab, you learned how to set up Application Insights telemetry, and drill down into performance
monitoring data through Application Insights in the new Azure Portal.

Try these labs out for next steps:

- Automated Recovery hands-on lab <!---Coming soon!-->
- Testing in Production hands-on lab <!---Coming soon!-->
- Load Tests and Auto-Scaling hands-on lab <!---Coming soon!-->
- Using Azure Automation Service to Provision and De-Provision Environments hands-on lab <!---Coming soon!-->