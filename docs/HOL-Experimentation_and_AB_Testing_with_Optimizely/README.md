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

* Click on "New Experiment".    

  ![](<media/32.png>)

* Give experiment a name, then enter your `production` site URL as the "Experiment URL" and click on "Create Experiment".

  ![](<media/33.png>)

**Step 5.** Dismiss tutorial if it pop-ups. Let's define the first variation where we will change the styling of "Shop Now" button.
  * Click on `<edit code>` button in the right bottom corner.

    ![](<media/27.png>)

  * Enter the following code and click on "Apply" afterwards:

        $(".carousel-link").css({"background-color":"#1c367c"});

    ![](<media/3.png>)

**Step 6.** Let's add a goal to this experiment. A goal is an information we are trying to find out by doing an experiment. In this case, our goal is to know whether changing the background of the "Shop Now" button will make it more likely for users to click on it.

  * Right click on the "Shop Now" button, select "Track Clicks..." and click on "Create New Click Goal".

    ![](<media/8.png>)

  * Enter "Shop Button" as a "Goal Name", make sure that this goal will be tracking clicks and click on "Save". On the next page click on "Close" button.

    ![](<media/9.png>)

**Step 7.** Save this variation by clicking on "Save now". Let's click on "Start Experiment" to activate this experiment. A few pop-ups might show up, we are only interested in "Diagnostic Report" one. It will indicate that a particular snippet of code has to be added to the project so that Optimizely can redirect users to the correct variation of the website. Take a note of this line of code.
![](<media/5.png>)
![](<media/6.png>)
>**Note:** If "Diagnostic Report" didn't pop-up then the same snippet of code can be found in settings for this project on Optimizely. Navigate "Home", select the project created in this lab, navigate to "Settings" tab and you should see it under "Implementation" sub-tab.
![](<media/7.png>)



### Task 2: Integrating Optimizely into PartsUnlimited
**Step 1.** Add the snippet of code you took a note of from Optimizely's website to the head tag of `_Layout.cshtml` file in the PartsUnlimitedWebsite/Views/Shared/ directory.

![](<media/10.png>)

**Step 2.** Commit changes using the following commands:

    git add .

    git commit -m "Added integration with Optimizely"

    git push

**Step 3.** Once the CI build and deployment to `dev` slot is completed, push your changes to the `staging` slot and finally `production` environment.



### Task 3: Setup second variation in Optimizely
Let's go back to Optimizely website. Navigate "Home", click on the experiment created in this lab then click on "Editor" button.

  ![](<media/28.png>)

**Step 1.** Click on "+ Add Variation", ignore a warning about making changes while the experiment is running.

![](<media/4.png>)

**Step 2.** Let's remove "Fork me on Github" element in the top left corner from this variation. Right click on it, select "Remove, check "Remove element from page." and click on "Done" button.

![](<media/11.png>)

**Step 3.** In this variation background color of all "Shop Now" buttons will be changed.

  * Change background color of the "Shop Button" in the carousel the same way as in variation 1.

  * If you scroll down you will see that all elements in "Arrivals" and "Top selling" groups also contain their own "Shop Now" buttons. Their color hasn't been changed because `.carousel-link` class is not used for these buttons.

    ![](<media/12.png>)

  * To change the color of these buttons allow animation to show you "Shop Now" button, right click on it, select "Edit Element..." and click on "Edit Style".

    ![](<media/13.png>)

  * Navigate to "Color & Background" tab and enter `#1c367c` as your "Background Color", then click "Done".

    ![](<media/14.png>)

  * This will generate a long CSS Selector.

        $("#home-page > section:eq(0) > div:eq(0) > div:eq(0) > div:eq(0) > a:eq(0) > div:eq(0) > div:eq(2) > div:eq(1)").css({"background-color":"#1c367c"});

    You may however be able to use a more generic selector if you know that it will only select the "Shop Now" buttons on the page, such as $(".shop-now"), this is preferred as it is more resilient to changes in the structure of the DOM.

  * If you were to look at the DOM, you would see that all those "Shop Now" buttons are in div tags with class "shop-now", i.e. changing the background color of that class will change background color of all "Shop Now" buttons.

      ![](<media/15.png>)

  * Replace previously added line of code with this one:

          $(".shop-now").css({"background-color":"#1c367c"});
    and click on "Apply". Save this variation by clicking on "Save now" in the top right corner.

    ![](<media/16.png>)

  * Optionally: Create another tracking goal for every of these buttons.



### Task 4: Updating settings of your experiment
Let's look at the main settings that could be adjusted for an experiment. Navigate "Home" and click on the experiment created in this lab.

  ![](<media/17.png>)

**Step 1.** Traffic Allocation:
  1. Click on "Edit" next to "Traffic Allocation: 100%".
  2. Adjust amount of traffic to participate in this experiment.
  3. Next, adjust the chance of seeing a particular variation of the website for the users included in this experiment.
  4. Click on "Apply".

      ![](<media/18.png>)

  >  **Note:** If 0% are included then none of the users will see any changes.

**Step 2.** Experiment's goals:
  1. Click on "Edit" next to "Goals".
  2. Set "Shop Button" goal as a primary goal for this experiment.
  3. Add any other goals you want to include in this experiment.
  4. Click on "Done".

      ![](<media/19.png>)

**Step 3.** Audiences:
  1. Click on "Edit" next to "Audiences".
  2. Click on "Create a New Audience"

      ![](<media/20.png>)

  3. Enter Name for your audience. Drag "Device" condition to "Audience Conditions" or any other condition you want to use and set it up as you wish. Click on "Save"

      ![](<media/21.png>)
      >**Note:** It's possible to create logical expressions using these conditions by dragging them into "and" or "or" sections.

  4. Click on "Done".

      ![](<media/29.png>)

**Step 4.** URL Targeting:
  1. Click on "Edit" next to "URL Targeting".
  2. Let's add `dev` and `staging` slots to this experiment.
  3. Click on "Save"

      ![](<media/22.png>)

**Step 5.** Scheduler:
  1. Click on "Edit" next to "Scheduler".
  2. Select time when experiment will start
  3. Select time when experiment will end
  4. Select time zone.
  3. Click on "Save"

      ![](<media/23.png>)



### Task 5: Viewing results of your experiment
Let's view some data collected by Optimizely.
>**Note:** Data can be generated by opening website in Incognito mode a few times and clicking on "Shop Now" button

**Step 1.** Click on "Results" button.
  ![](<media/24.png>)

**Step 2.** Once navigated to the "Results" page, you will be able to see and break down all of the collected information. This data can be exported for further analysis by clicking on "Share" button and selecting "Export CSV" near the top right corner.

  ![](<media/25.png>)

**Step 3.** This particular data indicates that users are more likely to click on the "Shop Now" button in "Variation #1" of the website than in any other variations.

  ![](<media/26.png>)
  >**Note:** Keep in mind that if the sample size of this experiment is not large enough then this data would most likely be meaningless.


Summary
----------
In this lab, you have learned how to integrate with Optimizely, create different variations of your website, set audience and traffic allocation for an experiment, view and export results.
