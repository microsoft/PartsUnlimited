HOL - Experimentation and A/B Testing with Optimizely
=====================================================
 The marketing team suspects that users don't click on the "Shop Now" button as often as they would if it was more prominent on the page. Even though A/B Testing is comparing performance between two pages, in this lab we will experiment and create multiple variations of PartsUnlimited website and see which performs better using Optimizely.
 > **Note:** To get a reliable results in A/B testing, the experiment should run long enough to collect data to draw a conclusion. Optimizely lets you know this on their "Results" page.

 > **Note:** Both website variations created in this HOL are relatively simple and the main purpose of them is to show an idea of how useful A/B Testing can be. A more complicated website variations might require writing/modifying JavaScript and more complicated CSS modifications.  



### Pre-requisites: ###

- Completed [Continuous Integration](https://github.com/Microsoft/PartsUnlimited/tree/master/docs/HOL-Continuous_Integration) and [Continuous Deployment](https://github.com/Microsoft/PartsUnlimited/tree/master/docs/HOL-Continuous_Deployment) HOLs

- Visual Studio 2015 Update 3



### Tasks Overview: ###

1. Setup first variation in Optimizely

2. Integrating Optimizely into PartsUnlimited

3. Setup second variation in Optimizely

4. Updating settings of your experiment

5. Viewing results of your experiment



### Task 1: Setup first variation in Optimizely
**Step 1.** Create an account with [Optimizely](https://www.optimizely.com/). Login after confirming your email address and entering password.

**Step 2.** After being navigated to a "Welcome to Optimizely" page, click on "Create Web Project".

![](<media/1.png>)

**Step 3.** Enter your `production` site URL as a name for the new project and click on "Create Project".

![](<media/2.png>)


**Step 4.** Create a new experiment.

* Click on "Create New" then "Experiment".    

  ![](<media/34.png>)

* Give experiment a name, then enter your `production` site URL as the Pages" and Set "Audiences" to Everyone which is the default value.
click on "Create Experiment".
  ![](<media/35.png>)

* Then Set the Metrics. Audiences and Metrics will be changed later. Click on "Create Experiment".

  ![](<media/36.png>)

**Step 5.** Dismiss tutorial if it pop-ups. Let's define the first variation where we will change the styling of "Shop Now" button.
  * Click on `Champaigns > Variation #1` button in the right bottom corner.

    ![](<media/37.png>)
  * Click on `Create` Then `Element Change`
    ![](<media/38.png>)

  * Enter the selector as ".carousel-link". 
  * Enter the color `rgba(28,54,124,1)` or `#1c367c` 
    ![](<media/39.png>)

  * Enter the `Enable Event Tracking` 

  ```
  Track click on this element: check
  Name: Shop Button
  Category: Converted
  ```
    ![](<media/40.png>)
  
  * Click on "Save".

### Task 2: Integrating Optimizely into PartsUnlimited
**Step 1.** Add the snippet of code you took a note of from Optimizely's website to the head tag of `_Layout.cshtml` file in the PartsUnlimitedWebsite/Views/Shared/ directory.
![](<media/41.png>)

![](<media/10.png>)

**Step 2.** Commit changes using the following commands:

    git add .

    git commit -m "Added integration with Optimizely"

    git push

**Step 3.** Once the CI build and deployment to `dev` slot is completed, push your changes to the `staging` slot and finally `production` environment.



### Task 3: Setup second variation in Optimizely
Let's go back to Optimizely website. Navigate Champaigns", click on the experiment created in this lab.

  ![](<media/47.png>)

**Step 1.** Click on "+ Add Variation", ignore a warning about making changes while the experiment is running.

![](<media/42.png>)

**Step 2.** Let's remove "Fork me on Github" element in the top left corner from this variation. Click on it.

![](<media/43.png>)

Then chagne Layout > Visibility to "Removed".

![](<media/44.png>)

**Step 3.** In this variation background color of all "Shop Now" buttons will be changed.

  * Change background color of the "Shop Button" in the carousel the same way as in variation 1.
  * Enter the `Enable Event Tracking` the same way as in variation 1 as well.

  * If you scroll down you will see that all elements in "Arrivals" and "Top selling" groups also contain their own "Shop Now" buttons. Their color hasn't been changed because `.carousel-link` class is not used for these buttons.
  * We hope the color is like this.

    ![](<media/45.png>)

  * If you were to look at the DOM, you would see that all those "Shop Now" buttons are in div tags with class "shop-now", i.e. changing the background color of that class will change background color of all "Shop Now" buttons.

    ![](<media/15.png>)

  * To change the color of these buttons, select the selector `.shop-now` and change color the same way as in variation 1. 
    ![](<media/46.png>)

  * Optionally: Create another tracking goal for every of these buttons.



### Task 4: Updating settings of your experiment
Let's look at the main settings that could be adjusted for an experiment. Navigate "Home" and click on the experiment created in this lab.

  ![](<media/47.png>)

**Step 1.** Traffic Allocation:
  1. Click on "Edit" next to "Traffic Allocation: 100%".
  2. Adjust amount of traffic to participate in this experiment.
  3. Next, adjust the chance of seeing a particular variation of the website for the users included in this experiment.
  4. Click on "Save".

      ![](<media/48.png>)

  >  **Note:** If 0% are included then none of the users will see any changes.

**Step 2.** Metrics:
  1. Click on "Metrics".
  2. Set "Shop Button" goal as a primary goal for this experiment.
  3. Add any other goals you want to include in this experiment.
  4. Click on "Save".

      ![](<media/49.png>)

**Step 3.** Audiences:
  1. Click on "Audiences".
  2. Click on "Create a New Audience"

      ![](<media/50.png>)

  3. Enter Name for your audience. Drag "Device" condition to "Audience Conditions" or any other condition you want to use and set it up as you wish. Click on "Save"

      ![](<media/51.png>)
      >**Note:** It's possible to create logical expressions using these conditions by dragging them into "and" or "or" sections.

  4. Click on "Save".

**Step 4.** Pages:
  1. Click on "Pages".
  2. Let's add `dev` and `staging` slots to this experiment.
  3. Click on "Save"

      ![](<media/52.png>)

**Step 5.** Schedule:
  1. Click on "Schedule".
  2. Select time when experiment will start
  3. Select time when experiment will end
  4. Select time zone.
  3. Click on "Save"

      ![](<media/53.png>)



### Task 5: Viewing results of your experiment
Let's view some data collected by Optimizely.
>**Note:** Data can be generated by opening website in Incognito mode a few times and clicking on "Shop Now" button

**Step 1.** Click on "Results" button.
  ![](<media/54.png>)

**Step 2.** Once navigated to the "Results" page, you will be able to see and break down all of the collected information. This data can be shared with stakeholders by clicking on "Share" button near the top right corner.

  ![](<media/55.png>)

**Step 3.** This particular data indicates that users are more likely to click on the "Shop Now" button in "Variation #2" of the website than in any other variations.

  >**Note:** Keep in mind that if the sample size of this experiment is not large enough then this data would most likely be meaningless.


Summary
----------
In this lab, you have learned how to integrate with Optimizely, create different variations of your website, set audience and traffic allocation for an experiment, view and export results.
