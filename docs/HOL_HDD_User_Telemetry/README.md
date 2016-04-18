#User Telemetry with Application Insights

The marketing team has expressed interest in the behavior of users across the PartsUnlimited website for how to best market the products, especially how popular the web application is and where the users live. The development team would like to know which browsers and operating systems that most users browse to the site from to target better user experiences.  

Using the out-of-box telemetry for Application Insights, the teams will be able to find out how people use the application and gain insights into the goals that they will need to achieve. 


In this lab, you will learn about setting up Application Insights and inserting the snippet of code for usage tracking into a web application to gain further insight into how users are behaving towards your web application.

**Prerequisites**

- Visual Studio 2013 or higher

- PartsUnlimited website deployed to a Microsoft Azure Web App (see [link](https://github.com/Microsoft/PartsUnlimited/blob/master/docs/Deployment.md))

- Application Insights created in the same Azure Resource Group as the PartsUnlimited website

- Continuous Integration build with deployment to the Azure web app

**Tasks Overview**

1. Set up Application Insights for PartsUnlimited

2. View real-time results for user telemetry in the Azure portal

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

Now that the telemetry has been added to the web application, it may take a few minutes for Application Insights to refresh.

###Task 2: View real-time results for user telemetry in the Azure portal

Now that we've given Application Insights time to refresh, we can take a look at the usage data in the new Azure Portal. The Portal will show a variety of metrics out of the box, including number of sessions, users, and browser sessions. 

**Step 1.** In an Internet browser, navigate to <http://portal.azure.com> and
sign in with your credentials.

![](<media/prereq-step1.png>)

**Step 2.** Click on the “Browse All” tile on the left column, select
“Application Insights,” and click on the name of the telemetry that was created when you deployed the resource group using the Deployment template in the PartsUnlimited solution. The name of the telemetry should match the name of the PartsUnlimited name plus "Insights."

![](<media/prereq-step2.png>)

**Step 3.** In the overview panel of the Application Insights instance, overall application health will be shown in server response time, browser page load time, server requests, and failed requests. (Actual times may vary)

![](<media/task2step3.png>)

**Step 4.** Scroll down in the overview portal and click on the "Usage" tile. 

![](<media/task2step4.png>)

**Step 5.** Note the Usage timeline. By drilling into usage, we can gauge how popular our web application is based on the number of distinct users, active sessions, and number of calls to trackPageView() (usually called once). Click in the chart on "Users" to view more information about the users. 

![](<media/task2step5.png>)

**Step 6.** In the Users timeline, note the number of users, new users, and page views. Additionally, the unique count of users by country is recorded. Click the X in the upper-right corner to close the Users panel.  

![](<media/task2step6.png>)

![](<media/task2step6b.png>)

**Step 7.** Back in the overview panel of the Application Insights instance, click on the chart for Browser Page Load Time.

![](<media/task2step7.png>)

**Step 8.** In the Browsers timeline, note the receiving response time, client processing time, page load network time, send request time, server response time, and page views. Additionally, the average count of browser page load time is shown below the timeline. Click on the timeline chart. 

![](<media/task2step8.png>)

**Step 9.** In the Chart Details pane, scroll down to the Client area and uncheck all of the properties except for Receiving Response Time. Then, turn on the Grouping and select "Browser" as the "group by" property. The Browsers timeline will change and show the average receiving response time broken down into the various browsers that were used to log into the site. 

![](<media/task2step9.png>)

![](<media/task2step9b.png>)

**Step 10.** Back in the Overview panel, scroll down and click on the "Page views" tile. 

![](<media/task2step10.png>)

**Step 11.** The Page views panel will break down the total page views, number of users, and pages per session for the web application.

![](<media/task2step11.png>)

All of the out-of-box data from Application Insights provide different types of data to learn how users interact with your web application. 

In this lab, you learned about setting up Application Insights and inserting the snippet of code for usage tracking into a web application to gain further insight into how users are behaving towards your web application.

Try these labs out for next steps:

- [Testing in Production hands-on lab](https://github.com/Microsoft/PartsUnlimited/tree/aspnet45/docs/HOL_HDD_Testing_in_Production)

- [Application Performance Monitoring hands-on lab](https://github.com/Microsoft/PartsUnlimited/tree/aspnet45/docs/HOL_Application_Performance_Monitoring)

- [Continuous Integration hands-on lab](https://github.com/Microsoft/PartsUnlimited/tree/aspnet45/docs/HOL_Continuous_Integration)

**Further Resources**

[Usage analysis for web applications with Application Insights](https://azure.microsoft.com/en-us/documentation/articles/app-insights-web-track-usage/)