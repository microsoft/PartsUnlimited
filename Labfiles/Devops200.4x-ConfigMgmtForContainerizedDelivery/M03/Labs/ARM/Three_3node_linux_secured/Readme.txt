sample command;

.\deploy.ps1 -subscriptionId 8791549a-6c99-40d9-8390-d316ee32d284 -resourceGroupName fabricclusterlinux -resourceGroupLocation westeurope -deploymentName d1 -templateFolder "One Cert"


----
Run .\deploy.ps1 with -templateFolder argument set to either 'One Cert' or 'Two Cert' 
to deploy a cluster with 1 or 2 certificates.

Story:
- Start with 1 cert.
- Wait until it's running completely.
- Upgrade to 2 certs, to enable rollover. (fallback to secondary)
- Rollover primary.


Use Chrome browser to see SF explorer. Edge doesn't support multiple certs with the same issuername.

