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
  




