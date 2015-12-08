#Summary#
As an Azure developer it's important to understand of key application code concepts in order to foster easier and more successful implementations for subsequent projects. The purpose of this document is to create technical, scenario-based collateral that represents real scenarios customers are building today. The intended audience for this collateral are senior developers and solution architects.

Each scenario will address common architectural and development issues typically faced within a project. Rather than creating a new site, we will make use of Parts Unlimited. Parts Unlimited is a fictional ecommerce website based on the The Phoenix Project by Gene Kim, Kevin Behr and George Spafford. 

The ecommerce website will adopt various new technologies which address these architectural and development issues as outlined below.
 
##Scaling Redis##

How to scale redis

###Issue###

1. Basic Setup 
	1. Connection managment
1. Failover / Monitoring, https://azure.microsoft.com/en-us/documentation/videos/azure-redis-cache-103-failover-and-monitoring/
	1. Diagnostic logs p
	2. Alerts


**Write through**, change to DB, trigger update to redis. Patterns for caching scnarios.

TODO : Validate with Adam, make sure that when the failover happens that site does not fall over.
Connection to primary, make sure fail overs to secondary.
Transparent : Nuget package
Errors during failover
Transient failover

Request : Cache is in fail over, error straight back, Maybe good thing, maybe try again. ???
Gracful fail over.

(Callout OR Do it ??) Patterns : Cahce igniter keep it HOT, cache invalidation. Per loading of cache, load more frequently 
Reduce cache misses, refresh entire data set on a schedule.

Q : HOw much is this about connecting to Redis VS using properly.
KVP cache VS set Query, sorting, push notifications, pub sub. --> POWER.
Multiple keys and same data vs query mentality.

Code snipit on GB blog Static context.
http://gavinb.net/2015/07/11/dont-get-burned-by-redis-connectionmultiplexer-a-sample-wrapper/

3. Evicted Keys - Cache miss, portal, cache strategy, cache igniter. 
Discussion at bus tier VS output cache.
- ASP.Net cache entire response. OR Cache in repository layer.
When to think about both. Callout providers for both of these scnearios. Redis for session managment 
(MS have packages for redis session and output)

What does your application do when error. Dedicated hardware, great no throttling. Fall back to DB (Boom DB)
Buy, fail back to DB, readonly version redirect to error page. 

Implement : Call to cache, retry 3 times, scenario exists then fall back option.
Static Connection object......

1. Working with HA configurations
	1. 2 tiers, basic single node no replication, go down see ya No resiliancy / journaling.
	2. standard : update second tier, in memory replication (Think)
	Failing over connections, special be aware of, retry vs timed outage.
	
1. Sharding https://azure.microsoft.com/en-us/documentation/articles/cache-dotnet-how-to-use-azure-redis-cache/ 
1. Cache invalidation and refresh
1. Write Through caching https://groups.google.com/forum/#!msg/redis-db/zqjMDawO5qM/pQTvdgpZbbEJ 
1. Dealing with Cache failure
	1. Route to another Cache and Lazy load
	1. Route to underlying store
1. How to fail gracefully


Q???

User history : Order history.
Redis sets, load order history into redis, change sort order ask redis to do sorting.

User profile data.

KVP cache, focus on message. Name photo.
Security roles, pub sub......




###Parts Unlimited###

To Identify

###Issue###

Admin uploading images.
Removes Vision API, Admin should be trustworthy.

CDN traffic Manager, flushing CDN.
Check DOCDb mark container as public......
Share access keys (OVer kill), OR to go through CDN

- Secure blob storage, magic link, access for one file.
- Access via CDN
Fetch on bhalf.

Comapre to SQL, Store links, manually store someplace else.
Wiring together.... Support is not obvious.

CDN by default, not look at QString param, configure to ignore magic string. File name is the same serve it up.
Inaapropriate.

TODO : 1 hour drill to see blobs in DOC DB. 
TODO : Vision API, thumbnail generation.

Images in CDN, CDN charge through traffic $MB image, thumbnail.
Cost of hosting site down.

Document how to store image, complete loop.
Make available. Flush cache through QString param.

Getting load of website, user not longer hits site for content, indirect.

**Callout ; Consider webjobs processing off line. Block thread, scale, increase resiliance.


1. show how to handle blobs, how to deicded if to let DocDB manage the blob or do it yourself, So we might have to show how to build that ourselves ?? *Q* Here or in DOC DB section..
1. Backing images by a CDN
1. Traffic manager
1. Version switching
1. Vision API (REMOVED???) ------if user generated photo / review .
	1. smart cropping !!
	2. colour and categories
	3. 
2.  


Attachments VS 



###Parts Unlimited###

##DocDB; Storage and indexing of Arbitrary Data Structures##

pull everything into DocdB.
Item detail.


compatibility.
Nested, manufacturer, model, year number, country origin ?? TIME> 


Azure DocumentDB is a NoSQL document database service designed from the ground up to natively support JSON and JavaScript directly inside the database engine. It’s the right solution for applications that run in the cloud when predictable throughput, low latency, and flexible query are key.

###Issue###

1. Complex arbitraty JSON documents
1. Load JSON templates from DB to render
1. Allow users to see, a BoM type structure and to search for similar items deep within that structure
where search relies on indexed arbitrary structure
	1. In particular we need to show the power of being able to perform a fast, strongly typed query, on arbitraty data structures
	2. Something where it matters to be numeric, and it matters to be a string, To show that it's not just Full Text Indexing, and we want to be deeply nested
nested

	3. 

2. 

Patterns :
- Embrace loose typing, lights, define intersting stuff this way, all indexed over the top.
- Imported data structures with different teams, 
- Indexing - 
- 
-KEY ::  Make more powerfull out of the quick start guides. 



###Parts Unlimited###





TODO
----

Scenario
Fit into PU
Prioritised list
