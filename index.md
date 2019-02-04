---
layout: default
title: "PartsUnlimited"
---

# Parts Unlimited

Parts Unlimited is an example eCommerce website site based for training purposes on the website described in chapters 31-35 of The Phoenix Project, by Gene Kim, Kevin Behr and George Spafford, © 2013 IT Revolution Press LLC, Portland, OR. Resemblance to “Project Unicorn” in the novel is intentional; resemblance to any real company is purely coincidental.

These labs are mostly based around the application PartsUnlimited, and they will walk you through implementing DevOps practices using Azure DevOps. The source files and labfiles used in these labs are available from the PartsUnlimited Git Repository <a href="http://github.com/microsoft/partsunlimited" target="_blank"><span style="color: #0066cc;" color="#0066cc">http://github.com/microsoft/partsunlimited</span></a>


There is also a shortened URL avalable for this site which you can use if you wish i.e. <a href="http://aka.ms/pulabs" target="_blank"><span style="color: #0066cc;" color="#0066cc">http://aka.ms/pulabs</span></a>



## Training and Certification usage

The labs on these GitHub pages are used as part of two training paths and certification programs:
- **Microsoft Professional Program (MPP) with DevOps**. 
    - The successful completion of the online courses and capstone project, that together make up the **MPP for DevOps**, results in the granting of the **Microsoft MPP for DevOps** credential. For more information on the **Microsoft Professional Program (MPP) for DevOps** program see the pages <a href="https://academy.microsoft.com/en-us/professional-program/tracks/devops/ " target="_blank"><span style="color: #0066cc;" color="#0066cc">https://academy.microsoft.com/en-us/professional-program/tracks/devops/ </span></a> and <a href="https://www.edx.org/microsoft-professional-program-devops " target="_blank"><span style="color: #0066cc;" color="#0066cc">https://www.edx.org/microsoft-professional-program-devops</span></a> 
- **Microsoft Exam AZ-400: Implementing Azure DevOps Solutions**
    - The <a href="https://www.microsoft.com/en-us/learning/exam-AZ-400.aspx " target="_blank"><span style="color: #0066cc;" color="#0066cc">AZ-400T05: Microsoft Azure DevOps Solutions </span></a> exam is currently in beta and some labs available on these pages map directly to sections of that exam. Individual labs that map to this exam will call that out on the lab page.


## Lab Structure
The labs available on this page are divided into sections that correspond to online courses or are direct mapping to objective domains sections of the AZ-400: Microsoft Azure DevOps Solutions exam. 
- The online courses are available  on <a href="http://www.edx.org" target="_blank"><span style="color: #0066cc;" color="#0066cc">http://www.edx.org</span></a>
- The courses corresponding objective domains sections of the *AZ-400: Microsoft Azure DevOps Solutions* exam are classroom based labs delivered by Microsoft Partners. Details of these courses are available from Microsoft partner sites.

### Pages lab sections
- DevOps Practices and Principles - <a href="https://www.edx.org/course/devops-practices-and-principles-0" target="_blank"><span style="color: #0066cc;" color="#0066cc">DevOps200.1x: DevOps Practices and Principles</span></a>
- Infrastructure as Code - 
    - <a href="https://www.edx.org/course/infrastructure-as-code-0" target="_blank"><span style="color: #0066cc;" color="#0066cc">DevOps200.2x: Infrastructure as Code</span></a>
    - *AZ-400T05: Implementing Application Infrastructure* classroom based course.
- Continuous Integration and Continuous Deployment - <a href="https://www.edx.org/course/continuous-integration-and-continuous-deployment-0" target="_blank"><span style="color: #0066cc;" color="#0066cc">DevOps200.3x: Continuous Integration and Continuous Deployment</span></a>
- Configuration Management for Containerized Delivery - <a href="https://www.edx.org/course/configuration-management-for-containerized-delivery-0" target="_blank"><span style="color: #0066cc;" color="#0066cc">DevOps200.4x: Configuration Management for Containerized Delivery </span></a>
- Testing - <a href="https://www.edx.org/course/devops-testing-1" target="_blank"><span style="color: #0066cc;" color="#0066cc">DevOps200.5x: DevOps Testing </span></a>
- DevOps for Databases - <a href="https://www.edx.org/course/devops-for-databases-0" target="_blank"><span style="color: #0066cc;" color="#0066cc">DevOps200.6x: DevOps for Databases</span></a>
- Application Monitoring and Feedback Loops - <a href="https://www.edx.org/course/application-monitoring-and-feedback-loops-0" target="_blank"><span style="color: #0066cc;" color="#0066cc">DevOps200.7x: Application Monitoring and Feedback Loops</span></a>
- DevOps for Mobile Apps - <a href="https://www.edx.org/course/devops-for-mobile-apps-0" target="_blank"><span style="color: #0066cc;" color="#0066cc">DevOps200.8x: DevOps for Mobile Apps</span></a>
- Architecting Distributed Cloud Applications - <a href="https://www.edx.org/course/architecting-distributed-cloud-applications-0" target="_blank"><span style="color: #0066cc;" color="#0066cc">DevOps200.9x: Architecting Distributed Cloud Applications </span></a>
- Advanced DevOps Practices


## PartsUnlimted Application - Key Features
- Works with Visual Studio 2017
- ASP.NET 5 support for Linux and Mono
- Updated to .NET Core 2.0 in Jan 2018
- Modern HTML5 responsive layout using bootstrap for mobile, tablet, and PC
- Includes a Dockerfile and sample publishing profile to publish to a Docker container
- Supports multiple authentication options including Azure Active Directory, Google, and Facebook
- Azure Machine Learning product recommendations based on Order History
- Designed for Azure Websites, including Testing in Production, Staging slots and environment variables for feature flags (to turn off recommendations)
- Includes Grunt tasks for publishing assets to Azure Storage for CDN ingestion for faster performance
- Entity Framework code-first using SQL Azure or an in-memory database (Mono)
- Basic administration pages to add or edit product information
- Includes Azure RM JSON templates and PowerShell automation scripts to easily build and provision your environment

For the labs based around the PartsUnlimitedMRP Java based application see the page <a href="http://microsoft.github.io/PartsUnlimitedMRP" target="_blank"><span style="color: #0066cc;" color="#0066cc">http://microsoft.github.io/PartsUnlimitedMRP</span></a>. The application and labs on this page use mostly open source software including Linux, Java, Apache, and MongoDB which creates a web front end, an order service, and an integration service.

## Issues and Updates

If you find any issues with the lab steps, you can open an issue in the github repo <a href="https://github.com/microsoft/PartsUnlimited" target="_blank"><span style="color: #0066cc;" color="#0066cc">https://github.com/microsoft/PartsUnlimited</span></a> and we will try to help resolve it, although response times can vary. You can also look through any previosuly logged issues in case it has been reported previously.

If you wish to submit fixes directly to a lab you can do so by opening Pull Request in the same GitHub repo, i.e. <a href="https://github.com/microsoft/PartsUnlimited" target="_blank"><span style="color: #0066cc;" color="#0066cc">https://github.com/microsoft/PartsUnlimited</span></a> against the file in question. See the bullets below for details.

- The lab steps on this page are sourced from files in the **gh-pages** branch in the **_posts** folder. 
- To identify the correct file against which to open a Pull Request, note the last part of the URL on the lab in question from your browser i.e. for the **PartsUnlimted Setup with Visual Studio** lab, the URL is <a href="http://microsoft.github.io/PartsUnlimited/pandp/200.1x-PandP-PUsetupwithVS2017.html" target="_blank"><span style="color: #0066cc;" color="#0066cc">http://microsoft.github.io/PartsUnlimited/pandp/200.1x-PandP-PUsetupwithVS2017.html</span></a>, so note the last part of that URL. 
- Then locate the corresponding file in the **gh-pages** branch in the **_posts** folder i.e. in this example it would be <a href="https://github.com/Microsoft/PartsUnlimited/blob/gh-pages/_posts/2018-01-08-200.1x-PandP-PUsetupwithVS2017.md" target="_blank"><span style="color: #0066cc;" color="#0066cc">https://github.com/Microsoft/PartsUnlimited/blob/gh-pages/_posts/2018-01-08-200.1x-PandP-PUsetupwithVS2017.md</span></a> 
- Then open the Pull request against the file in question.
- All lab step files are in markdown.
- Images in the lab steps are located in the **assets** folder in individual lab folders. If you are not sure of the folder, the image location paths are available from the individual lab markdown files located in the **_posts** folder. 
- Updates to help keep the labs current are welcome.



### Media Elements and Templates
 You may copy and use images, clip art, animations, sounds, music, shapes, video clips and templates provided with the sample application and identified for such use in documents and projects that you create using the sample application. These use rights only apply to your use of the sample application and you may not redistribute such media otherwise.

