
#Fault Injection for Service Fabric

Customers of the PartsUnlimited website have complained of the site becoming unresponsive when there's a big sale on. This loss in availability has a direct impact on sales. You have been tasked with ensuring high availability and scalability for the site. 

**Prerequisites**

- Visual Studio 2015 Update 3

- An active Azure subscription

- [Install Service Fabric SDK](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-get-started/) and [Create a local cluster](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-get-started-with-a-local-cluster/)

**Tasks Overview**

1. Prepare the PartsUnlimited solution to work with Service Fabric

2. Deploy the Service Fabric package to an Azure Service Fabric Cluster.

3. Run some tests against the cluster.

###Task 1: Prepare the PartsUnlimited solution to work with Service Fabric

Service Fabric requires an executable. Our first step involves ensuring the PartsUnlimited can publish the exe we require.

**Step 1.** Open up PartsUnlimited.sln in Visual Studio.

**Step 2.** Locate the project.json file under the PartsUnlimitedWebsite project.

**Step 3.**  We need to remove the 'platform' property of Microsoft.NETCore.App. Check out the sample below.

Note: The platform property is used when you want to build **portable apps** that can be executed by the .NET Core shared runtime (via 'dotnet run myapp'). Service Fabric requires a **standalone app**. Standalone apps have an executable file with the runtimes embedded in the application itself. This should be added at the begining of the .json file.

```json
{
    "dependencies": {
        "Microsoft.NETCore.App": {
            "version": "1.0.0"
        }
        ...
    }
}
```

**Step 4.** Now we want add the desired runtime so it will build an executable. In the same project.json file add the following to the root of the json file. This should be located at the **end** of the file, after the **scripts** section.

Note: The runtime you use here is important. If you want to solution to work locally and you're not using Windows 10 - use one of the following:

*Windows 7*: win7-x64

*Windows 8*: win8-x64

*Windows 8.1*: win81-x64

*Windows 10*: win10-x64

```json
{
    ...

    "runtimes": {
        "win10-x64": {}
    }

    ...
}
```

**Step 5.** Right click on the PartsUnlimitedWebsite and select 'Publish'

**Step 6.** Create a new profile called 'FileSystem' then select 'Next'

![](<media/publish.png>)

**Step 7.** Select the file system publish method.

**Step 8.** Select a location you will remember for when we need to package the application.

![](<media/publish-location.png>)

**Step 9.** Set the target runtime to the one you added to the project.json file earlier. Also, select the option to delete all existing files prior to push. This will ensure that future publish actions will provide a clean code set when we come to deploy the application. Then click on publish.

![](<media/settings.png>)


Service fabric expects a specific folder structure to work correctly. This is **not** generated automatically from the output of the Visual Studio publish profile. We will therefore have to create this manually.

    |-- WebSite
        |-- WebApp
            |-- Code
                ...
                |-- PartsUnlimitedWebsite.exe
                ...
            |-- Config
                |-- Settings.xml
            |-- Data
            |-- ServiceManifest.xml
        |-- ApplicationManifest.xml


Now lets grab all the published files we created before and put them in the **Code** folder as seen in the directory structure above.

The other two important files are ServiceManifest.xml and ApplicationManifest.xml.

**ServiceManifest.xml** should look something like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<ServiceManifest xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Name="WebApp" Version="1.0.0.0" xmlns="http://schemas.microsoft.com/2011/01/fabric">
<ServiceTypes>
    <StatelessServiceType ServiceTypeName="WebApp" UseImplicitHost="true">
<Extensions>
        <Extension Name="__GeneratedServiceType__">
            <GeneratedNames xmlns="http://schemas.microsoft.com/2015/03/fabact-no-schema">
                <DefaultService Name="WebAppService" />
                <ServiceEndpoint Name="WebAppTypeEndpoint" />
            </GeneratedNames>
        </Extension>
        </Extensions>
</StatelessServiceType>	 
</ServiceTypes>
<CodePackage Name="code" Version="1.0.0.0">
    <EntryPoint>
        <ExeHost>
        <Program>PartsUnlimitedWebsite.exe</Program>
        <WorkingFolder>CodePackage</WorkingFolder>
    <ConsoleRedirection FileRetentionCount="5" FileMaxSizeInKb="2048"/>
        </ExeHost>
    </EntryPoint>
</CodePackage>
<Resources>
    <Endpoints>
        <Endpoint Name="WebAppTypeEndpoint" Protocol="http" Port="5000" UriScheme="http" Type="Input" />
    </Endpoints>
</Resources>
</ServiceManifest>
```
**ApplicationManifest.xml** should look something like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<ApplicationManifest xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ApplicationTypeName="WebAppType" ApplicationTypeVersion="1.0" xmlns="http://schemas.microsoft.com/2011/01/fabric">
    <ServiceManifestImport>
        <ServiceManifestRef ServiceManifestName="WebApp" ServiceManifestVersion="1.0.0.0" />
    </ServiceManifestImport>
    <DefaultServices>
        <Service Name="WebAppService">
            <StatelessService ServiceTypeName="WebApp" InstanceCount="1">
            <SingletonPartition />
            </StatelessService>
        </Service>
    </DefaultServices>
</ApplicationManifest>
```

**Settings.xml needs to exist** but for now it can just be an **empty file** as we don't require any configuration values at the moment.

Something to note - if you run this locally you want to make sure that the instance count is 1. This is because it's not possible to host multiple web applications on the same port. When we deploy this to a cluster we will not have this problem.

Now we're ready to deploy locally. Fire up PowerShell as an administrator and type the following:

We want to connect to the service fabric cluster.

```powershell
Connect-ServiceFabricCluster localhost:19000
```

Now we're going to point to the folder structure we created (with the app inside the code section).

```powershell
Copy-ServiceFabricApplicationPackage -ApplicationPackagePath 'C:\Users\user\Desktop\WebSite' -ImageStoreConnectionString 'file:C:\SfDevCluster\Data\ImageStoreShare' -ApplicationPackagePathInImageStore 'Apps\WebApp'
```

Now we want to register the application type.

```powershell
Register-ServiceFabricApplicationType -ApplicationPathInImageStore 'Apps\WebApp'
```

Now we're going to create the new application.

```powershell
New-ServiceFabricApplication -ApplicationName 'fabric:/WebApp' -ApplicationTypeName 'WebAppType' -ApplicationTypeVersion 1.0
```

Open a web browser and navigate to http://localhost:19080/

![](<media/service-fabric.png>)

We can drill down on the cluster and see our active node.

![](<media/node-cluster.png>)

Now navigate to http://localhost:5000 to check to see if the PartsUnlimited website is active.

![](<media/localhost-parts-unlimited.png>)

###Task 2: Deploy the Service Fabric package to an Azure Service Fabric Cluster.
Now that we have the application working locally, let's deploy to an Azure Service Fabric Cluster.

**Step 1.** First things first, let's create a Service Fabric container for our PartsUnlimited site. 

Navigate to the Azure portal and select 'New' -> https://portal.azure.com

![](<media/azure1.png>)

**Step 2.** Search for 'Service Fabric Cluster' then select the top option.

![](<media/azure2.png>)

**Step 3.** Select the 'Create' button at the bottom of the 'Service Fabric Cluster' window.

![](<media/azure3.png>)

**Step 4.** Fill out the basics required. Note: ensure you add a unique cluster name as some common ones may already be taken.

Select a secure username and password combination.

Select the subscription and resource group that applies to you're PartsUnlimited HOL.

![](<media/azure4.png>)

**Step 5.** Here we only have one node type (the PartsUnlimited website). We want to give this node type a name and allocate a machine size. We can set the initial VM scale as well.

Add the custom endpoint of 5000 as this is the default port that the app is hosted on.

![](<media/azure5.png>)

![](<media/azure6.png>)

**Step 6.** For demo purposes we will use the 'Unsecure' setting as there are quite a few steps involved in setting this up in Azure. If you want more information visit https://azure.microsoft.com/en-us/documentation/articles/key-vault-get-started/. 

It is strongly advised to use the secure mode in production.

![](<media/azure7.png>)

**Step 7.** Check over the settings one last time and ensure everything looks OK. Once you have validated the settings select 'Create'.

![](<media/azure8.png>)

**Step 8.** On the Azure dashboard, we can now see that the Service Fabric Cluster is being set up. This may take some time.

![](<media/azure9.png>)

After it has sucessfully deployed you should see the following on the dashboard.

![](<media/azure10.png>)


**Step 9.** Select the PartsUnlimited cluster. Then copy the Service Fabric Explorer link and paste it in your browser of choice.

![](<media/azure12.png>)

**Step 10.** This is the Azure Service Fabric Explorer. 

![](<media/service-fabric-no-apps.png>)

**Step 11.** OK, now Azure is ready we need to push up the PartsUnlimitedWebsite solution. Open up PowerShell in administrator mode.

**Note: If you want more than one instance of the application, quickly navigate to your package folder. In ApplicationManifest.xml change the instance count to 5.**

Firstly we want to connect to the cluster.

```powershell
Connect-ServiceFabricCluster [your-server-cluster-address].[location].cloudapp.azure.com:19000
```

**Step 12.** Now we want to upload the application package to the cluster.

```powershell
Copy-ServiceFabricApplicationPackage -ApplicationPackagePath 'C:\local\location\of\package' -ImageStoreConnectionString 'fabric:imagestore' -ApplicationPackagePathInImageStore 'Apps\WebApp'
```

**Step 13.** Then we want to register the application type in our cluster.

```powershell
Register-ServiceFabricApplicationType -ApplicationPathInImageStore 'Apps\WebApp'
```

**Step 14.** Now we want to create a new Service Fabric application.

```powershell
New-ServiceFabricApplication -ApplicationName 'fabric:/WebApp' -ApplicationTypeName 'WebAppType' -ApplicationTypeVersion 1.0
```

**Step 15.** Go back to the Service Fabric dashboard and check to see if the application has deployed successfully.

![](<media/azure11.png>)

You can also view the application via the following url structure -> `<yourClusterName>.<yourRegion>.cloudapp.azure.com:5000`

###Task 1: Run some tests against the cluster.

Now let's cause some mayhem. Service Fabric provides a 'chaos' scenario that will causes faults across the entire Service Fabric cluster.

*"The chaos test scenario generates faults across the entire Service Fabric cluster. The scenario compresses faults generally seen over months or years to a few hours. The combination of interleaved faults with a high fault rate finds corner cases that would otherwise be missed."*

**Step 1.** Run the following commands in PowerShell.

Firstly we want to connect to the cluster (note you may already be connected)

```powershell
Connect-ServiceFabricCluster [your-server-cluster-address].[location].cloudapp.azure.com:19000
```

Now for the core of the script. Specify a desired time to run (the below example will run for 10 minutes)

```powershell
$timeToRun = 10
$maxStabilizationTimeSecs = 180
$concurrentFaults = 3
$waitTimeBetweenIterationsSec = 60

Invoke-ServiceFabricChaosTestScenario -TimeToRunMinute $timeToRun -MaxClusterStabilizationTimeoutSec $maxStabilizationTimeSecs -MaxConcurrentFaults $concurrentFaults -EnableMoveReplicaFaults -WaitTimeBetweenIterationsSec $waitTimeBetweenIterationsSec
```

![](<media/powershell.png>)

**Step 2.** While the test scenario is running you should be able to see errors occurring in the Service Fabric Explorer.

![](<media/faults.png>)

**Step 3.** If we want to run some specific failures you can get a list of possible testability actions from here -> https://azure.microsoft.com/en-us/documentation/articles/service-fabric-testability-actions/

Let's simulate a Service Fabric cluster node failure by stopping a node. To do this we will need to find the name of a node to test.

![](<media/node.png>)

Now that we have the node name of "_website_1" we can run our test.

**Step 4.** Run this sample in PowerShell 

```powershell
Connect-ServiceFabricCluster [your-server-cluster-address].[location].cloudapp.azure.com:19000
    
Stop-ServiceFabricNode -NodeName "_website_1"
```

![](<media/node-stop.png>)

**Step 5.** If you want to turn the node back on use the following command.

```powershell
Start-ServiceFabricNode -NodeName "_website_1"
```

**Step 6.** We can try another test scenario included in the Service Fabric SDK. This is the 'failover test scenario'.

*"The failover test scenario targets a specific service partition instead of the entire cluster, and it leaves other services unaffected. The scenario iterates through a sequence of simulated faults in service validation while your business logic runs. A failure in service validation indicates an issue that needs further investigation. The failover test induces only one fault at a time, as opposed to the chaos test scenario, which can induce multiple faults."*

```powershell
Connect-ServiceFabricCluster [your-server-cluster-address].[location].cloudapp.azure.com:19000

$timeToRun = 10
$maxStabilizationTimeSecs = 180
$waitTimeBetweenFaultsSec = 10
$serviceName = "fabric:/WebApp/WebAppService"

Invoke-ServiceFabricFailoverTestScenario -TimeToRunMinute $timeToRun -MaxServiceStabilizationTimeoutSec $maxStabilizationTimeSecs -WaitTimeBetweenFaultsSec $waitTimeBetweenFaultsSec -ServiceName $serviceName -PartitionKindSingleton
```

![](<media/powershell1.png>)

We can see in the portal the actual instances being tampered with in the Service Fabric Explorer.

![](<media/explorer-fail.png>)

In this lab, you learned about how to get up and running with Service Fabric. You then learned how to test your Service Fabric cluster to ensure the availability of your application. We tried scenarios that compress faults generally seen over months or years to a few hours.