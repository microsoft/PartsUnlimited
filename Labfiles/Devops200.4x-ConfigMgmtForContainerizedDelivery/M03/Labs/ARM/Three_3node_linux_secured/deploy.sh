#!/bin/bash

usage() { echo "Usage: $0 -t <subscriptionId> -p <resourceGroupName> -q <deploymentName> -l <resourceGroupLocation>" 1>&2; exit 1; }

# Initialize parameters specified from command line
while getopts ":t:p:q:l:" o; do
	case "${o}" in
		t)
			echo "in case t"
			subscriptionId=${OPTARG}
			;;
		p)
			resourceGroupName=${OPTARG}
			;;
		q)
			deploymentName=${OPTARG}
			;;
		l)
			resourceGroupLocation=${OPTARG}
			;;
		esac
done
shift $((OPTIND-1))

#Prompt for parameters is some required parameters are missing
if [ -z "$subscriptionId" ]; then
	echo "Subscription Id:"
	read subscriptionId
fi

if [ -z "$resourceGroupName" ]; then
	echo "ResourceGroupName:"
	read resourceGroupName
fi

if [ -z "$deploymentName" ]; then
	echo "DeploymentName:"
	read deploymentName
fi

if [ -z "$resourceGroupLocation" ]; then
	echo "Enter a location below to create a new resource group else skip this"
	echo "ResourceGroupLocation:"
	read resourceGroupLocation
fi

#templateFile Path - template file to be used
templateFilePath="template.json"

#parameter file path
parametersFilePath="parameters.json"

if [ -z "$subscriptionId" ] || [ -z "$resourceGroupName" ] || [ -z "$deploymentName" ]; then
	echo "Either one of subscriptionId, resourceGroupName, deploymentName is empty"
	usage
fi

#login to azure using your credentials
azure login

#set the default subscription id
azure account set $subscriptionId

#switch the mode to azure resource manager
azure config mode arm

#Check for existing resource group
if [ -z "$resourceGroupLocation" ] ; 
then
	echo "Using existing resource group..."
else 
	echo "Creating a new resource group..." 
	azure group create --name $resourceGroupName --location $resourceGroupLocation
fi

#Start deployment
echo "Starting deployment..."
azure group deployment create --name $deploymentName --resource-group $resourceGroupName --template-file $templateFilePath --parameters-file $parametersFilePath