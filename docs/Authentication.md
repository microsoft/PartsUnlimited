#OAuth#
The PartsUnlimited app supports 5 authentication models: ASP.NET, Azure Active Directory, Google, Facebook, and Twitter. For the non-ASP.NET auth models, you must use create an auth app for the auth model you want to use and provide it with the URL for the site, and then modify the code in your project’s config file. In ASP.NET 5, all auth is configured in a config.json file.The instructions below explain how to set this up for each model:


##Azure Active Directory##
1.	Sign in to the [Azure Portal](https://manage.windowsazure.com)
1.	Create a new Active Directory (or use an existing one if available)
1.	Select the directory and add a new application
1.	Enter a name and select “web application”
1.	Under “App Properties” provide the URL for your site
1.	Once your app is created, select it and go to the Configure tab
1.	Under the “Keys” section, select a duration
1.	Select “Save” in the bottom toolbar to generate the key
1.	In the PartsUnlimited project, open the config.json file
1.	Find the “Authentication” section
1.	Under the “Microsoft” section, add the Client ID as the “Key” and the Client Secret as the “Secret”
1.	Run your app and test that the Active Directory login works
1.	For more information, see the [Active Directory OAuth documentation](https://msdn.microsoft.com/en-us/library/azure/dn645542.aspx )


##Google##
1.	Create or log in to your Google Account from the Google Developers Console
1.	Create a new project
1.	After the project is created, expand the “APIs & Auth” menu on the left-hand navigation bar
1.	Select “Credentials” from the drop down
1.	Under the OAuth section, select “Create new Client ID”
1.	In the Create Client ID dialog, select “Web application”
1.	Enter a Product Name, and under “Homepage URL” enter the URL for your site
1.	Save your changes to generate the Client ID and Client Secret
1.	In the PartsUnlimited project, open the config.json file
1.	Find the “Authentication” section
1.	Under the “Google” section, add the ClientID as the “Key” and the Client Secret as the “Secret”
1.	Run your app and test that the Google login works
1.	For more information, see the Google OAuth documentation: https://developers.google.com/api-client-library/dotnet/get_started#auth


##Facebook##
1.	Create a new account or log in to your Facebook developer account
1.	Add a new Facebook app for Websites
1.	Once the app is created, enter the URL of your web project as the “Site URL” of the Facebook app
1.	In the PartsUnlimited project, open the config.json file
1.	Find the “Authentication” section
1.	Under the “Facebook” section, add the Facebook app AppID as the “Key” and the AppSecret as the “Secret”
1.	Run your app and test that the Facebook login button works
1.	For more information, see the Facebook OAuth documentation: https://developers.facebook.com/docs/facebook-login/overview/v2.2 

##Twitter##
1.	Create a new account or log in to your Twitter developer account
1.	Create a new application
1.	Enter the URL of your website during the creation process
1.	In the PartsUnlimited project, open the config.json file
1.	Find the “Authentication” section
1.	Under the “Twitter” section, enter the consumer key as the “Key” and consumer secret as the “Secret”
1.	Run your app and test that the Google login works
1.	For more information, see the Twitter OAuth documentation: https://dev.twitter.com/oauth/overview/introduction
