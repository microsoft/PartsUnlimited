#Summary#
As an Azure developer it's important to understand of key application code concepts in order to foster easier and more successful implementations for subsequent projects. The purpose of this document is to create technical, scenario-based collateral that represents real scenarios customers are building today. The intended audience for this collateral are senior developers and solution architects.

Each scenario will address common architectural and development issues typically faced within a project. Rather than creating a new site, we will make use of Parts Unlimited. Parts Unlimited is a fictional ecommerce website based on the The Phoenix Project by Gene Kim, Kevin Behr and George Spafford. 

The ecommerce website will adopt various new technologies which address these architectural and development issues as outlined below.
 
##Scaling Redis##

Redis is a technology used to cache items from within your application, this making resource hungry requests less often and improving the read performance.

Azure Redis Cache is based on the open-source Redis cache. It gives you access to a secure, dedicated Redis cache, managed by Microsoft and accessible from any application within Azure. 

This section will see us reviewing common mistakes with connection management, identifying an approach to keeping the items in our cache up to date. We'll also look at what happens when the cache is failing over to a secondary or is offline how we detecting these failure conditions and ensure our application is resilient at failure time.

We will mention Sharding within the premium tier, other types of data retrieval within Redis (Sets, query, sort, pub sub) and Cache providers which can plug into your application with minimal code changes.


###Flow###

***Q*** : How much is this about connecting to Redis VS using it properly.

1. Connection management done right.
1. Failover / Monitoring, 
	1. (Call out to Azure diagnostic logs / alerts) - EvictedKey a metric to look at. 
	2. (Call out to service tiers) Load balancing / HA
		1. basic single node no replication, go down see No resiliancy.
		2. standard : two nodes, dedicated hardware, no throttling up to machine specs, update second tier, in memory replication, automated switching fail over bethe scenes.
		3. premium : sharding, percistance to disk
	3. During fail over what happens to your application as it's switching to secondary **(Decision as to Call out VS build)** ***TODO : Determine what is cached as this will mean we need to have items in the cache which need to be written to when an error occurs***
		1. Route to another Cache and Lazy load
		1. Route to underlying store, in Parts Unlimited potential to pass all queries off to source, if this is SQL call out potential to create bottle neck at the database.
			1. Read only data - re-direct to error page 
			2. Write data - in the case of parts unlimited becuase this means income write orders to the source database
	3. Fail over monitored with Transient fault handling with custom detection strategy
2.  Cache invalidation / refresh
	1.  Pre loading of the cache.
	2.  Worker task to keep cache items up to date,  
	2.  (Timed job to refresh cache items from source with a window less that the cache expiry)
2. Cached objects
		1.  Azure Redis ASP.Net Output cache provider 
		2.  Azure Redis Session State provider
		3.  Cache in your Business layer cache
	2.  Cache options
	3.  Can be a KVP, lesser known features sets, sorting and pub sub abilities. Call out to Signal R Back plane ?

###Parts Unlimited updates###

Update Parts unlimited to cache the order history, shows use of sets and ordering. 
TODO : Need to determine the fail over scenario if this is to be shown. Perhaps add customer balances updated at order time ? - E.g. Important for admin to carry out billing, that will go back to database but cached items will not. 

Pointers to cache connection management, 
Handling of failover using transient fault block.
  
##User generated content##

In a e-commerce scenario we want to provide the flexibility for the users of our application to be able to upload custom content and have the application handle it in a way which will not significantly increase hosting costs or cause any end user performance degradation.

We will look at ways in which we can upload and store images and have this content provided through an external source. We will capture thumbnails and generate image metadata using Microsoft's Computer Vision APIs and cover ways in which to handle updated content.

###Flow###

1. Creating content
	1. Demonstrate storing images outside of our application - stored in Azure blob storage
		* ***Q*** Quick demonstration, or call out quick start guide for blob storage?
		* ***Q*** Call out benefits of blob storage over traditional storage
			* Cheap data storage costs
			* Simplified integration with Queues and WebJobs for time consuming out-of-process image manipulation
			* Simplified integration with CDN
	2. Remove content serving responsibility from our application and onto the Azure Content Delivery Network (CDN)
		* Improve performance and user experience for end users located farther from content source
		* Removing potential high load from, and improving scalability of our application web server, in particular when multiple HTTP requests are required to serve content heavy pages
	3. Cache busting with deployments - version number query string
	4. Cache busting with user updated content.
1. Updating content
	1. Admin updating images
		1. Microsoft's Computer Vision APIs (Project Oxford)
			1. - Store image meta data for searching ? Tie into DOC DB colour search ?
			2. Thumbnail generation (CDN charged by MB, keeping costs down)
		2. Meta data storage in doc db for searching (colours) 
		3. Smart cropping of images.
	4. Call out : Production non admin site recommend process off line with queue / web jobs.
1. Call out load, regionalised sites, Traffic manager (Content and site)

###Parts Unlimited updates###

Extend Part's Unlimited's admin product section
	1. Remove existing product image url "hotlink" field
	2. Replace with file upload field
	3. Leverage Microsoft's Computer Vision APIs to generate:
		* "Smart cropped" thumbnail
		* Resized, web optimised version of source file for larger display
		* Extract image metadata
	5. Integrate with Azure blob storage for image hosting
	6. Store link to images (thumb, resized, etc) in DocDb with meta data along side it.
	7. Wrap CDN across the storage account.
	8. Modify image display of Part's Unlimited to link to CDN with query string appending version number.
	9. Deployment time, replace image with CDN reference.

##DocDB; Storage and indexing of Arbitrary Data Structures##

Microsoft Azure DocumentDB is the highly-scalable NoSQL document database-as-a-service that offers rich query and transactions over schema-free data, helps deliver reliable and predictable performance, and enables rapid development.

To highlight the capability of the schema free database we will show an architectural approach to address the schema free and embrace loose typings. This will be shown through storage of the product catalog with products which contain deeply nested attributes and show an approach how they can be shown and queried across.  


###Flow###

1. Complex arbitraty JSON documents
1. Load JSON templates from DB to render
1. Allow users to see, a BoM type structure and to search for similar items deep within that structure
where search relies on indexed arbitrary structure
	1. In particular we need to show the power of being able to perform a fast, strongly typed query, on arbitraty data structures
	2. Call out the indexing options
	2. Something where it matters to be numeric, and it matters to be a string, To show that it's not just Full Text Indexing, and we want to be deeply nested

###Parts Unlimited updates###

Re work product storage to utilize DOC DB
Re work product display to store template in doc db.
Template will contain how to load and search for similar products.

Product search, find other products which could apply to this model: Include Manufacturer, model, **year number**, country origin. Search for products with same and year +/- 3 years as they are similar.


 




