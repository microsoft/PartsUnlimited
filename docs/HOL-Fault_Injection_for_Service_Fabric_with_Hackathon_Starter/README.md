
#Fault Injection for Service Fabric Hackathon Starter

Service uptime is one of the most important metrics for any application. Users are quick to switch if they cannot have access 24/7. Azure Service Fabric is a platform designed to make it easy to package, deploy and scale reliable applications. You have been tasked with improving availability for the hackathon starter app.

**Prerequisites**

- [An active Azure subscription](https://portal.azure.com)

- [Service Fabric SDK](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started)

- [Node.js 6.0+](http://nodejs.org)

- [Git](https://git-scm.com/downloads)

- [MongoDB](https://www.mongodb.com/)

**Tasks Overview**

1. **Get the source code for the Hackathon starter** - in this task we will go through pulling down the hackathon source code and testing it locally.

2. **Prepare the Hackathon Service Fabric package** - in this task we will go through the steps required to create a deployment package for the hackathon starter app.

3. **Set up MongoDB** - in this step we'll set up a MongoDB instance in Azure.

4. **Deploy the Service Fabric package to a windows cluster** - in this step we will deploy the actual service fabric package to Azure.

5. **Run some tests against the cluster** - In this step we'll go through some basic tests you can run against an Azure Service Fabric cluster.

### Task 1: Get the source code for the Hackathon starter.

**Step 1.** Open and command prompt of your choice which has support for git

**Step 2.** Navigate to your chosen working directory

```bash
cd "C:\dev\repos"
```

**Step 3.** Firstly, we want to clone the Hackathon starter repository and install our dependencies.

```bash
# Get the latest snapshot
git clone --depth=1 https://github.com/sahat/hackathon-starter.git hackathon-starter

# Change directory
cd hackathon-starter

# Install NPM dependencies
npm install
```

**Step 4.** We need to make sure MongoDB is running before we try and start the application. Open a second command prompt and run the following command

`mongod`

> If MongoDB is not on the path on Windows it will fail to start.
>
> You can lauch it directly, the default installation directory is `C:\Program Files\MongoDB\Server\3.2\bin`

> You may receive the following error: `Data directory C:\data\db\ not found., terminating`
>
>This means you need to create a 'data' folder on whatever drive you're running the CMD instance on, with a sub folder of 'db'. MongoDB uses this location to store the database.

![](<media/mongod.png>)

**Step 5.** Lets try run the application locally.

```bash
node app.js
```

![](<media/app-running.png>)

**Step 6.** In your browser, navigate to `http://localhost:3000/`

![](<media/hackathon-starter-site.png>)

#### Task 2: Prepare the Hackathon Service Fabric package.

**Step 1.** Create a new folder that we will use to package the Hackathon application in. For example -> `C:\release`

**Step 2.** Let's set up the package structure required by Service Fabric.

Firstly in the package root folder we want to create a new folder called `NodeService` eg `C:\release\NodeService`

**Step 3.** Now add another file to the package root directory (`C:\release`) called ApplicationManifest.xml and add the following to the file.

```xml
<?xml version="1.0" encoding="utf-8"?>
    <ApplicationManifest xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ApplicationTypeName="NodeAppType" ApplicationTypeVersion="1.0" xmlns="http://schemas.microsoft.com/2011/01/fabric">
    <Parameters>
        <Parameter Name="Website_InstanceCount" DefaultValue="-1" />
    </Parameters>
    <ServiceManifestImport>
        <ServiceManifestRef ServiceManifestName="NodeService" ServiceManifestVersion="1.0" />
    </ServiceManifestImport>
    <DefaultServices>
        <Service Name="NodeServiceService">
            <StatelessService ServiceTypeName="NodeService" InstanceCount="[Website_InstanceCount]">
                <SingletonPartition />
            </StatelessService>
        </Service>
    </DefaultServices>
</ApplicationManifest>
```

**Step 4.**
In the NodeService folder create the following ServiceManifest.xml file (`C:\release\NodeService\ServiceManifest.xml`), this file should have the following contents inside.

```xml
    <?xml version="1.0" encoding="utf-8"?>
        <ServiceManifest xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" Name="NodeService" Version="1.0" xmlns="http://schemas.microsoft.com/2011/01/fabric">
        <ServiceTypes>
            <StatelessServiceType ServiceTypeName="NodeService" UseImplicitHost="true">
                <Extensions>
                    <Extension Name="__GeneratedServiceType__">
                    <GeneratedNames xmlns="http://schemas.microsoft.com/2015/03/fabact-no-schema">
                        <DefaultService Name="NodeServiceService" />
                        <ServiceEndpoint Name="NodeServiceTypeEndpoint" />
                    </GeneratedNames>
                    </Extension>
                </Extensions>
            </StatelessServiceType>
        </ServiceTypes>
        <CodePackage Name="C" Version="1.0">
            <EntryPoint>
                <ExeHost>
                    <Program>node.exe</Program>
                    <Arguments>app.js</Arguments>
                    <WorkingFolder>CodePackage</WorkingFolder>
                </ExeHost>
            </EntryPoint>
        </CodePackage>
        <Resources>
            <Endpoints>
                <Endpoint Name="NodeServiceTypeEndpoint" Protocol="http" Type="Input" Port="3000"/>
            </Endpoints>
        </Resources>
    </ServiceManifest>
```

**Step 5.** Inside the NodeService folder, create a folder called C (`C:\release\NodeService\C`). This will store the code for our node application.


**Step 6.** Inside the NodeService folder, create a folder called config (`C:\release\NodeService\config`) You should now have the following folder and file structure:

    |-- ApplicationPackageRoot

        |-- NodeService
            |-- C
            |-- config
            |-- ServiceManifest.xml
        |-- ApplicationManifest.xml



**Step 7.** We need to bundle node and the Hackathon Starter code within our self contained Service Fabric package.

Grab the node.exe from whatever location shows up and copy it to a new deployment folder. For the example below we're using `xcopy "from/here" "to/here"`

> Note that the `where` command is only available in the Windows CMD prompt and not when using PowerShell

```powershell

where node.exe

xcopy "C:\Program Files\nodejs\node.exe" "C:\release\NodeService\C"

xcopy "C:\dev\repos\hackathon-starter\*" "C:\release\NodeService\C" /s /i

```

Alternatively you can use the linux commands `which nodejs` then `cp from to`

```bash

which nodejs

cp node/location/node.exe /release/NodeService/C/

cp -a /repos/hackathon-starter/. /release/NodeService/C/

```
You should now have the following folder and file structure:
    |-- ApplicationPackageRoot

        |-- ApplicationManifest.xml
        |-- NodeService
            |-- C
                |-- Hackathon application files (the ones you got from Github)
                |-- node.exe
            |-- config
            |-- ServiceManifest.xml

### Task 3: Set up our Azure MongoDB instance.

**Step 1.** Navigate to the [Azure portal](https://ms.portal.azure.com) and select 'New'

![](<media/azure1.png>)

**Step 2.** Search for 'MongoDB' and select the 'Database as a service for MongoDB', published by Microsoft.

![](<media/add-mongo.png>)

**Step 3.** On the next promp, scroll down to the bottom and hit 'create'.

![](<media/mongo-create.png>)

**Step 4.** On the next screen specify all the required configuration values as seen below. You may need to accept that you are using a preview service if you haven't used this particular service before. Note you will need to find an available ID as some names may already be taken. Also create a resource group which ties to this lab (e.g. hackathon-sf).

![](<media/mongo-config.png>)

**Step 5.** Navigate to the main resource group list and locate the group you just created. In the example above we used the name 'hackathon-sf'.

![](<media/locate-mongo.png>)

**Step 6.** Now we want to grab the connection string. Select 'Connection String' and copy down the value supplied somewhere for the next step.

![](<media/mongo-connection.png>)

**Step 7.** Navigate back to the hackathon solution Service Fabric package and open the `C:\release\NodeService\C\.env.example` file

At the top of the file you will see two environment variables set 'MONGODB_URI' and 'MONGOLAB_URI'. We want to change the values set to be the connection string we pulled from Azure in the previous step.

For example:

```
MONGODB_URI=mongodb://hack-db:qC9Hhe3Gk...==@hack-db.documents.azure.com:10250/?ssl=true
MONGOLAB_URI=mongodb://hack-db:qC9Hhe...==@hack-db.documents.azure.com:10250/?ssl=true
...
```

### Task 4: Deploy the Service Fabric package to a windows cluster.
Let's deploy to a Windows cluster in Azure.

**Step 1.** First things first, let's create a Service Fabric container for our Hackathon starter site.

Navigate to the Azure portal and select 'New' -> https://ms.portal.azure.com

![](<media/azure1.png>)

**Step 2.** Search for 'Service Fabric Cluster' then select the top option.

![](<media/azure2.png>)

**Step 3.** Select the 'Create' button at the bottom of the 'Service Fabric Cluster' window.

![](<media/azure3.png>)

**Step 4.** Fill out the basics required. Note: ensure you add a unique cluster name as some common ones may already be taken.

Select a secure user name and password combination.

Select the subscription and resource group that applies to this lab.

![](<media/azure4.png>)

**Step 5.** Here we only have one node type. We want to give this node type a name and allocate a machine size. We can set the initial VM scale as well.

Add the custom endpoint of 3000 as this is the default port that the app is hosted on.

![](<media/azure5.png>)

**Step 6.** For demo purposes we will use the insecure setting as there are quite a few steps involved in setting this up in Azure. If you want more information visit https://azure.microsoft.com/en-us/documentation/articles/key-vault-get-started/.

It is strongly advised to use the secure mode in production.

![](<media/azure7.png>)

**Step 7.** Check over the settings one last time and ensure everything looks OK. Once you have validated the settings select 'Create'.

![](<media/azure8.png>)

**Step 8.** On the Azure dashboard, we can now see that the Service Fabric Cluster is being set up. This may take some time.

![](<media/azure9.png>)

After it has successfully deployed you may see the following on the Azure dashboard.

![](<media/azure10.png>)

In some instances as the Service Fabric Cluster provisions the portal may automatically open the blade for you

![](<media/azure10a.png>)

> Note: You must wait until the status listed for your cluster is **Ready** otherwise the next step will fail.

**Step 9.** Select the Hackathon cluster, then select 'Custom fabric settings'. Add a new value for the **EseStore** with the parameter **MaxCursors** and the value **65536**. This is to ensure our node modules folder will be able to be registered correctly. Service fabric currently has a hard limit on the amount of files and will timeout if this value is not increased. *Ideally you should have a build task that minifies and concatenates all your vendor dependencies*. This may take some time to complete as it needs to propagate across the scale set.

![](<media/sf-settings.png>)

![](<media/max-cursors.png>)

> Note: You must again wait until the status listed for your cluster is **Ready** otherwise the next step will fail. This can take a very long time.

Navigate back to the 'Overview' section inside the service fabric cluster. Then copy the 'Service Fabric Explorer' link and paste it in your browser of choice.

![](<media/azure11.png>)

**Step 10.** This is the Azure Service Fabric Explorer.

![](<media/sfe.png>)

**Step 11.** OK, now Azure is ready we need to push up the Hackathon starter solution. Open up PowerShell in administrator mode.

Firstly we want to connect to the cluster.

```powershell
Connect-ServiceFabricCluster [your-server-cluster-address].[location].cloudapp.azure.com:19000
```

Note: ensure the address does not have https:// and must specify port 19000 - eg. `hackathon.westcentralus.cloudapp.azure.com:19000`

**Step 12.** Now we want to upload the application package to the cluster.

```powershell
Copy-ServiceFabricApplicationPackage -ApplicationPackagePath 'C:\release' -ImageStoreConnectionString 'fabric:imagestore' -ApplicationPackagePathInImageStore 'Apps\Hackathon' -TimeoutSec 600
```

**Step 13.** Then we want to register the application type in our cluster.

```powershell
Register-ServiceFabricApplicationType -ApplicationPathInImageStore 'Apps\Hackathon' -TimeoutSec 600
```

**Step 14.** Now we want to create a new Service Fabric application.

```powershell
New-ServiceFabricApplication -ApplicationName 'fabric:/Hackathon' -ApplicationTypeName 'NodeAppType' -ApplicationTypeVersion 1.0
```

**Step 15.** Go back to the Service Fabric dashboard and check to see if the application has deployed successfully.

http://[your-server-cluster-address].[location].cloudapp.azure.com:19080/Explorer/index.html#/

![](<media/azure12.png>)

**Step 16.** Now let's see if the application is also running. Navigate to http://[your-server-cluster-address].[location].cloudapp.azure.com:3000 and you should see the following screen.

![](<media/hackathon-landing.png>)

### Task 5: Run some tests against the cluster.

Now let's cause some mayhem. Service Fabric provides a 'chaos' scenario that will causes faults across the entire Service Fabric cluster.

*"The chaos test scenario generates faults across the entire Service Fabric cluster. The scenario compresses faults generally seen over months or years to a few hours. The combination of interleaved faults with a high fault rate finds corner cases that would otherwise be missed."*

**Step 1.** Run the following commands in PowerShell.

Firstly we want to connect to the cluster (note you may already be connected)
    Connect-ServiceFabricCluster [your-server-cluster-address].[location].cloudapp.azure.com:19000

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

![](<media/sf-error.png>)

**Step 3.** If we want to run some specific failures you can get a list of possible testability actions from here -> https://azure.microsoft.com/en-us/documentation/articles/service-fabric-testability-actions/

Let's simulate a Service Fabric cluster node failure by stopping a node. To do this we will need to find the name of a node to test.

![](<media/node-app.png>)

Now that we have the node name of "_app_1" we can run our test.

**Step 4.** Run this sample in PowerShell

```powershell
Connect-ServiceFabricCluster [your-server-cluster-address].[location].cloudapp.azure.com:19000

Stop-ServiceFabricNode -NodeName "_app_1"
```

![](<media/node-down.png>)

**Step 5.** If you want to turn the node back on use the following command.

```powershell
Start-ServiceFabricNode -NodeName "_app_1"
```

**Step 6.** We can try another test scenario included in the Service Fabric SDK. This is the 'fail-over test scenario'.

*"The failover test scenario targets a specific service partition instead of the entire cluster, and it leaves other services unaffected. The scenario iterates through a sequence of simulated faults in service validation while your business logic runs. A failure in service validation indicates an issue that needs further investigation. The failover test induces only one fault at a time, as opposed to the chaos test scenario, which can induce multiple faults."*

```powershell
Connect-ServiceFabricCluster [your-server-cluster-address].[location].cloudapp.azure.com:19000

$timeToRun = 10
$maxStabilizationTimeSecs = 180
$waitTimeBetweenFaultsSec = 10
$serviceName = "fabric:/WebApp/WebAppService"

Invoke-ServiceFabricFailoverTestScenario -TimeToRunMinute $timeToRun -MaxServiceStabilizationTimeoutSec $maxStabilizationTimeSecs -WaitTimeBetweenFaultsSec $waitTimeBetweenFaultsSec -ServiceName $serviceName -PartitionKindSingleton
```

We can again see in the portal the actual instances being tampered with in the Service Fabric Explorer.

In this lab you have learned how to set up and deploy a node application to service fabric. You have also learned how to set up MongoDB in Azure with ease. You then ran some tests to ensure your infrastructure could handle some potential hosting issues - like nodes randomly restarting and going off-line. This would help with finding potential bottlenecks and fixing them before you release your application.