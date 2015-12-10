#Summary#
As an Azure developer it's important to understand of key application code concepts in order to foster easier and more successful implementations for subsequent projects. The purpose of this document is to create technical, scenario-based collateral that represents real scenarios customers are building today. The intended audience for this collateral are senior developers and solution architects.
 
#Setup#

Before you going you will need to pull the solution and setup the required Azure dependencies which are listed below.

##Redis##

Reference
[Get stared with Redis](https://azure.microsoft.com/en-us/documentation/articles/cache-dotnet-how-to-use-azure-redis-cache/)


##Azure Storage / CDN ##
Reference
[How to use CDN](https://azure.microsoft.com/en-us/documentation/articles/cdn-how-to-use-cdn/)

##DocDB##
Reference
[Learning Path Doc DB](https://azure.microsoft.com/en-us/documentation/learning-paths/documentdb/)

##Web.config##
Edit web config, add the following keys here.

#Scenarios#

Each scenario will address common architectural and development issues typically faced within a project. Rather than creating a new site, we will make use of Parts Unlimited. Parts Unlimited is a fictional ecommerce website based on the The Phoenix Project by Gene Kim, Kevin Behr and George Spafford. 

The ecommerce website will adopt various new technologies which address these architectural and development issues as outlined below.

1. [Scaling Redis](1. Redis.md)

2. [User Generated Content](2. Content.md)

3. [Doc DB](3. DocDB.md)
 




