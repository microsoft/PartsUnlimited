#Summary#
As an Azure developer it's important to understand of key application code concepts in order to foster easier and more successful implementations for subsequent projects. The purpose of this document is to create technical, scenario-based collateral that represents real scenarios customers are building today. The intended audience for this collateral are senior developers and solution architects.

Each scenario will address common architectural and development issues typically faced within a project. Rather than creating a new site, we will make use of Parts Unlimited. Parts Unlimited is a fictional ecommerce website based on the The Phoenix Project by Gene Kim, Kevin Behr and George Spafford. 

The ecommerce website will adopt various new technologies which address these architectural and development issues as outlined below.
   
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

 




