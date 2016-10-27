#User Telemetry and Performance Monitoring with Application Insights

The marketing team has expressed interest in the behavior of users across the PartsUnlimited website for how to best market the products, especially how popular the web application is and where the users live. The development team would like to know which browsers and operating systems that most users browse to the site from to target better user experiences. The development team has also noticed that the recommendations page is slow to load and shows performance spikes in Application Insights telemetry. By viewing the details of performance monitoring through Application Insights, we will be able to drill down to the code that has affected the slow performance of the web application and fix the code.

Using the out-of-box telemetry for Application Insights, the teams will be able to find out how people use the application and gain insights into the goals that they will need to achieve.

In this lab, you will learn about setting up Application Insights to gain further insight into how users are behaving towards your web application, and drill down into performance monitoring data through Application Insights in the new Azure Portal.

**Prerequisites**

- Continuous Integration with Visual Studio Team Services (see [link](https://github.com/Microsoft/PartsUnlimited/blob/master/docs/HOL-Continuous_Integration/README.md))

- Continuous Deployment with Release Management in Visual Studio Team Services (see [link](https://github.com/Microsoft/PartsUnlimited/blob/master/docs/HOL-Continuous_Deployment/README.md))

- Application Insights created in the same Azure Resource Group as the PartsUnlimited website

- Continuous Integration build with deployment to the Azure web app

**Tasks Overview**

**1. Set up Application Insights for PartsUnlimited** This will walk you through creating an Application Insights instance in Azure and connecting it to the PartsUnlimited solution.

**2. View real-time results for user telemetry in the Azure portal** In this step you will be shown where to find all of the information collected by Application Insights.

**3. Using Application Performance Monitoring to resolve performance issues** In this step you will investigate and resolve a performance issue with the help of Application Insights.



###Task 1: Set up Application Insights for PartsUnlimited
**Step 1.** To configure Application Insights with PartsUnlimited, please follow these steps: [Application Insights - Getting Started](https://github.com/Microsoft/ApplicationInsights-aspnetcore/wiki/Getting-Started)  


**Step 2.** Open command line that supports Git and navigate to the PartsUnlimited repository. Run the following commands to push your changes to the remote repository and trigger CI and CD:
```Bash
git add .

git commit -m "added Application Insights"

git push
```

Now that the telemetry has been added to the web application, it may take a few minutes for Application Insights to refresh.



###Task 2: View real-time results for user telemetry in the Azure portal

Now that we've given Application Insights time to refresh, we can take a look at the usage data in the new Azure Portal. The Portal will show a variety of metrics out of the box, including number of sessions, users, and browser sessions.

> **Note:** Before you proceed, you need to generate data for the Application Insights instance by browsing the website for a few minutes.

**Step 1.** In an Internet browser, navigate to <http://ms.portal.azure.com> and
sign in with your credentials.

![](<media/prereq-step1.png>)

**Step 2.** Click on the “More services” tile on the left column, and select “Application Insights”.

 ![](<media/prereq-step1.1.png>)

**Step 3.** Click on the name of the telemetry that was created when you deployed the resource group using the Deployment template in the PartsUnlimited solution.

![](<media/prereq-step2.png>)

**Step 4.** In the overview panel of the Application Insights instance, overall application health will be shown in server response time, page view load time, server requests, and failed requests. (Actual times may vary)

![](<media/task2step3.png>)

**Step 5.** In the Application Insights instance blade, scroll down and click on the "Usage" tile. By drilling into usage, we can gauge how popular our web application is based on the number of distinct users, active sessions, and number of calls to trackPageView() (usually called once). Click on "Users" to view more information about the users.

![](<media/task2step4.png>)

**Step 6.** In the Users timeline, note the number of users, new users, and page views. Additionally, the unique count of users by country is recorded. Click the X in the upper-right corner to close the Users panel.  

![](<media/task2step6.png>)

**Step 7.** Back in the overview panel of the Application Insights instance, click on the chart for Page View Load Time.

![](<media/task2step7.png>)

**Step 8.** In the Browsers timeline, note the receiving response time, client processing time, page load network time, send request time, server response time, and page views. Additionally, the average count of browser page load time is shown below the timeline. Click on the "Edit" button in the first timeline chart.

![](<media/task2step8.png>)

**Step 9.** In the Chart Details pane, scroll down to the Client area and uncheck all of the properties except for Receiving Response Time. Then, turn on the Grouping and select "Browser version" as the "group by" property. The Browsers timeline will change and show the average receiving response time broken down into the various browsers that were used to log into the site.

![](<media/task2step9.png>)

![](<media/task2step9b.png>)

**Step 10.** Back in the Usage panel, and click on "Page views".

![](<media/task2step10.png>)

**Step 11.** The Page Views panel will break down the total page views, number of users, and pages per session for the web application.

![](<media/task2step11.png>)

###Task 3: Using Application Performance Monitoring to resolve performance issues

**Step 1.** In an Internet browser, navigate to the PartsUnlimited website that you previously deployed and go to the Recommendations page, such as http://partsunlimited.azurewebsites.net/home/recommendations.

![](<media/task3-step6.png>) 

**Step 2.** In the Application Insights instance blade, scroll down and select the “Performance” tile to view performance monitoring
information.

![](<media/task3-step1.png>)

**Step 3.** In the Performance panel, note the timeline. The timeline data may not show up immediately, so you will want to wait for a few minutes for the telemetry to collect performance data.

![](<media/task3-step2.png>)

**Step 4.** Once data shows in the timeline, view the operations listed under the **Average
of server response time by operation name** section under the timeline. Click on the top operation in the list referring to the recommendations page to view details of that operation.

**Step 5.** Drill down into the method that is affecting the slow performance. We now know where the slow performance is being caused in our code.

**Step 6.** Using your preferred IDE or a text editor, open `HomeController.cs` and find the Recommendations method that is causing slow performance. At the top of the HomeController class, notice that the public int roco_count is set to 1000. Change that value to be 1.

![](<media/task3-step3.png>)

**Step 7.**  Open command line in PartsUnlimited repository and run the following commands:
```Bash
git add .

git commit -m "Changed roco_count from 1000 to 1 in HomeController.cs after being aware of slow perf in AI"

git push
```
>**Note** This will push the changes up to the remote repo and kick off a build automatically.

**Step 8.** Now that our changes have deployed to the website, open up a new incognito browser window (to prevent caching) and return to the recommendations page (http://partsunlimited.azurewebsites.net/home/recommendations).

![](<media/task3-step6.png>) 

**Step 9.** Return to the Application Insights performance monitoring view in the Azure Preview Portal and refresh the page.

![](<media/task3-step7.png>) 

In this lab, you learned how to set up Application Insights telemetry to gain further insight into how users are behaving towards your web application, and drill down into performance monitoring data through Application Insights in the new Azure Portal.


Try these labs out for next steps:

- [Testing in Production hands-on lab](https://github.com/Microsoft/PartsUnlimited/tree/master/docs/HOL_HDD_Testing_in_Production)

**Further Resources**

[Usage analysis for web applications with Application Insights](https://azure.microsoft.com/en-us/documentation/articles/app-insights-web-track-usage/)
