#Summary#
As an Azure developer it's important to understand of key application code concepts in order to foster easier and more successful implementations for subsequent projects. The purpose of this document is to create technical, scenario-based collateral that represents real scenarios customers are building today. The intended audience for this collateral are senior developers and solution architects.

Each scenario will address common architectural and development issues typically faced within a project. Rather than creating a new site, we will make use of Parts Unlimited. Parts Unlimited is a fictional ecommerce website based on the The Phoenix Project by Gene Kim, Kevin Behr and George Spafford. 

The ecommerce website will adopt various new technologies which address these architectural and development issues as outlined below.
 
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


 




