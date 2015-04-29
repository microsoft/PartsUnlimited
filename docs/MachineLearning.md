#Machine Learning#

PartsUnlimited uses a “Frequently Bought Together” API service to populate the “Recommendations” section that appears in every product page. This service is built on Azure Machine Learning and is available for free on the Azure Marketplace.

In the config.json file of PartsUnlimited we provide a “WebsiteOptions” key named “ShowRecommendations” that you can use to turn “Recommendations” section on or off. Optionally, you can add this key to your website app settings using the Azure portal. Once you’ve added it, you’ll be able to turn this section on and off from the Portal instead of having to redeploy your application for the changes to take effect.

The service takes a few seconds to load for the first time. In order to avoid delaying the rest of the page, we display the recommendations in a separate div and use ajax to asynchronously load the content.

The project has sample data included (under Models/SampleData.cs). This populates the purchase history. You can then create a .csv file based on these transactions that the “Frequently Bought Together” service uses to generate the recommendations. The instructions below also have data on how to train your service separately using your own data from a .csv file.

##How to sign up for the "Frequently Bought Together" Service ##
1.	Go to the [“Frequently Bought Together” API service page](https://datamarket.azure.com/dataset/amla/mba)
1.	Sign up for the service (may require you to sign up for a Marketplace account if you don’t have one)
1.	After you successfully sign up for the service, go to “Mange your models and upload data”
1.	You need to get your credentials by clicking on the “Account Information” link. Copy and paste the string back in to the Account Key field to validate and initiate the connection
1.	Create a new model and train it with a proper set of data. You can use [Transactions.csv](https://github.com/Microsoft/PartsUnlimited/blob/master/docs/Transactions.csv) to train your model. It takes two columns of data: one is transaction ID and the other one is item ID
1.	After you have created and trained the model, you can test it by selecting the model you just created and entering an item ID to see the output. This output represents item IDs that are usually bought together with the input item ID (Tip: number of occurrences you specify to train your model must be greater than the smallest multiplier in your sample data)

##How to add the "Frequently Bought Together" Service to your site##
1.	In the PartsUnlimited project, open the config.json file
1.	Find the “Keys” section
1.	Under “AzureMLFrequentlyBoughTogether”, enter the “AccountKey” and “ModelName” of your “Frequently Bought Together” service. You can find these under the "Account Information" link in the datamarket site
1.	Under the “WebsiteOptions” section, make sure that the “ShowRecommendations” key is set to “true”
1.	Run your app and test that this section works
