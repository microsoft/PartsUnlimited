#Summary#
As an Azure developer it's important to understand of key application code concepts in order to foster easier and more successful implementations for subsequent projects. The purpose of this document is to create technical, scenario-based collateral that represents real scenarios customers are building today. The intended audience for this collateral are senior developers and solution architects.
 
#Setup#

Before you get going you will need to pull the solution and setup the required dependancies.
The source for the current solution exists [here](https://github.com/Microsoft/PartsUnlimited/tree/accelerator).
The required Azure dependencies will also need to be created and configured, instructions are below.

##Redis##

To get started using the Redis Cache hosted within Azure you will need to setup a cache using the Azure portal.
Instructions to carry this out can be found [here](https://azure.microsoft.com/en-us/documentation/articles/cache-dotnet-how-to-use-azure-redis-cache/). 

Once you have a Redis cache configured you will need to edit the [`config.json`](../../src/PartsUnlimitedWebsite/config.json) file by filling in the `HostName` and `AccessKey` details, these can be found within Azure. If these details are not filled in PartsUnlimited will fall back to using an implementation of an in memory cache.
	
	"RedisCache": {
		"HostName": "<tennantname>.redis.cache.windows.net",
		"AccessKey": "<key>",
		...
	}

##DocDB##
To get started using the Azure DocumentDB first you will need to setup a database account using the Azure portal.
Instructions to carry this out can be found [here](https://azure.microsoft.com/en-us/documentation/articles/documentdb-create-account/). 

Once you have a Database account you will need to edit the [`config.json`](../../src/PartsUnlimitedWebsite/config.json) file by filling in the `URI` and `Key` details, these can be found within Azure. If these details are not filled in PartsUnlimited will fall back to using an implementation of a SQL Database.

	"DocDb": {
    	"URI": "https://<tennantname>.documents.azure.com:443/",
    	"Key": "<key>"
    }

Further information and learning resources can be found on the DocumentDB  [Learning Path way](https://azure.microsoft.com/en-us/documentation/learning-paths/documentdb/)

####Azure storage####

For a detailed guide on getting started, see: [here](https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/)

For modifications to Parts Unlimited, an Azure storage account, with a public blob container will need to be created.

####Computer Vision API####
In order to make use of Microsoft's Computer Vision APIs, we wil need to subscribe to Project Oxford, which can be done [here](https://www.projectoxford.ai/Account/Login?callbackUrl=/Subscription/Index?productId=/products/54d873dd5eefd00dc474a0f4)

Doing so will give us access to a free offer that currently provides all Vision API operations that include analysing an image and generating thumbnails. The free plan limits calling to the Vision APIs limits to 20 transactions per minute and 5000 transactions per month. Once this has been done, we will be provided with a unique access key required as part of every call the the API.

Once you have a VisionAPI account you will need to edit the [`config.json`](../../src/PartsUnlimitedWebsite/config.json) file by filling in the `URI` and `Key` details.

	"VisionApi": {
        "Key": "<key>",
        "Uri": "https://api.projectoxford.ai/vision/v1/analyses"
    },

####Azure Content Delivery Network (CDN)####

For an overview and guide on getting started see: [here](https://azure.microsoft.com/en-us/documentation/articles/cdn-serve-content-from-cdn-in-your-web-application/)

Your unique CDN URI needs to be configured in the [`config.json`](../../src/PartsUnlimitedWebsite/config.json) file by replacing the `tennantname` below with your newly created CDN tennant.

	"CDN": {
    	"ProductImages": "<tennantname>.azureedge.net"
	}

#Scenarios#

Each scenario will address common architectural and development issues typically faced within a project. Rather than creating a new site, we will make use of Parts Unlimited. Parts Unlimited is a fictional ecommerce website based on the The Phoenix Project by Gene Kim, Kevin Behr and George Spafford. 

The ecommerce website will adopt various new technologies which address these architectural and development issues as outlined below.

1. [Scaling Redis](1. Redis.md)

2. [User Generated Content](2. Content.md)

3. [DocumentDB; Storage and indexing of Arbitrary Data Structures](3. DocDB.md)
 




