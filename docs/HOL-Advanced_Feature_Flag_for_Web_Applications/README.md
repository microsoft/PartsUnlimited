HOL - Advanced Feature Flag for Web Applications
====================================================================================

There's a big sales initiative coming up next month to assist with a much anticipated product release. In the past infrastructure stability has been a massive concern and we have lost many customers with downtime and slow server response times. This time around we have a plan to stagger the release to different regions at different times to try distribute the load. You have been tasked with implementing the solution.

### Pre-requisites: ###
- Visual Studio 2015 Update 3 or higher

### Tasks Overview: ###

1. Add the backend code required for feature flags - In this task we will go through the steps required to create advanced feature flags which are region and time based.

2. Try it out! - In this task we will see our feature flag in action, simulating a staggered release to countries.

### Task 1: Add the backend code required for feature flags

**Step 1.** Clone the PartsUnlimited repository to a local directory.

* Open a command line (one that supports Git) and navigate to the directory where you want to store your local repositories. For example in a Windows OS, you can create and navigate to `C:\Source\Repos`.

* Clone the repository with the following command:

    git clone https://github.com/Microsoft/PartsUnlimited.git
	> After a few seconds of downloading, all of the code should now be on your local machine.

* Move into the repository directory that was just created. In a Windows OS, you can use this command:

    cd PartsUnlimited

**Step 2.** Open the PartsUnlimited solution with Visual Studio

In the command line, type the following.

    start PartsUnlimited.sln

Or navigate to where you cloned the repository to e.g. `C:\Source\Repos` with explorer and double click on PartsUnlimited.sln

**Step 3.** Let's create a folder for our feature flag related classes. On the PartsUnlimitedWebsite solution, right click and create a new folder. We can call it something like 'FeatureFlag'.

![](<media/new-folder.png>)

**Step 4.** Now we will create the first feature flag class. Right click on the newly created **FeatureFlag** folder -> select **'Add'** -> select **'Class...'**.

![](<media/new-class.png>)

**Step 5.** Create a new class in this folder called `FeatureType.cs`. This will be used to define different types of features we have. 

```csharp
public enum FeatureType
{
    Default,
    Region
}
```

**Step 6.** Add another class in the same folder called `Feature.cs`. This will define the structure of our features.

```csharp
public class Feature
{
    public Feature(string key, string description, bool active, FeatureType type)
    {
        Key = key;
        Description = description;
        Active = active;
        Type = type;
    }

    public FeatureType Type { get; }

    public bool Active { get; set; }

    public string Description { get; }

    public string Key { get; }

}
```
- **Feature Type** is an enum which represents the feature type.
- **Active** is going to be the current state for the feature flag. 
- **Description** is going to be a brief description of what the feature flag is for.
- **Key** is going to be the unique identifier for that particular feature flag.

**Step 7.** We need an interface we will use to template all our different feature toggle types. This interface will have two method signatures: 'Can' and 'Do'. 'Can' will be used to decide which strategy should be run. 'Do' will perform the actual work required for the feature flag. Create this in the same 'FeatureFlag' folder we created before.

```csharp
public interface IFeatureStrategy
{
    bool Can(Feature feature);

    bool Do(Feature feature, string comparison);
}
```

**Step 8.** Now we want to create a feature manager class and interface. This will be used later on to toggle our features on and off. 

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public interface IFeatureManager
{
    bool GetStatusByKey(string key, string comparison);
    void ChangeFeatureToggles(string[] selectedItems);
    IEnumerable<Feature> RetrieveFeatures();
}

public class FeatureManager: IFeatureManager
{
    private readonly IEnumerable<Feature> _features;
    private readonly IEnumerable<IFeatureStrategy> _strategies;

    public FeatureManager(IEnumerable<Feature> features, IEnumerable<IFeatureStrategy> strategies)
    {
        if (features == null)
            throw new ArgumentNullException(nameof(features));
        if (strategies == null)
            throw new ArgumentNullException(nameof(strategies));

        _features = features;
        _strategies = strategies;
    }

    public IEnumerable<Feature> RetrieveFeatures()
    {
        return _features;
    }

    public bool GetStatusByKey(string key, string comparison)
    {
        if (comparison == null)
            throw new ArgumentNullException(nameof(comparison));
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        Feature feature = _features.SingleOrDefault(f => f.Key == key);

        foreach (IFeatureStrategy strategy in _strategies)
        {
            if (strategy.Can(feature))
            {
                return strategy.Do(feature, comparison);
            }
        }
        throw new Exception(string.Format("Unable to find a feature strategy for the key {0} with the comparison of {1}", key, comparison));
    }

    public void ChangeFeatureToggles(string[] selectedItems)
    {
        if (selectedItems == null)
            throw new ArgumentNullException(nameof(selectedItems));

        _features.AsParallel().ForAll(
            f =>
            {
                f.Active = selectedItems.Contains(f.Key);
            });
    }

}
```

Now let's explain the sections of the above code.

- **Features** is a list of features we want to use with the application.
- **GetStatusByKey** is a method we will use to get the current status for a particular key. This will check to see if there's an existing strategy which can handle this feature flag.
- **ChangeFeatureToggles** takes in a list of selected feature toggles. If the key exists in any of our 'features' that status will be toggled.

*Note: It's a much better idea to store these flags in a database. For simplicity's sake we have stored them in memory.*

**Step 9.** Now lets create a region specific feature. 

```csharp

public interface IFeatureFlags
{
    Dictionary<string, DateTime> Regions { get; }
}

public class FeatureFlags : IFeatureFlags
{
    public Dictionary<string, DateTime> Regions { get; }

    public FeatureFlags(Dictionary<string, DateTime> regions)
    {
        if (regions == null)
            throw new ArgumentNullException(nameof(regions));
        Regions = regions;
    }
}

```


```csharp
public class FeatureRegionStrategy : IFeatureStrategy
    {
        private readonly IFeatureFlags _featureFlags;

        public FeatureRegionStrategy(IFeatureFlags featureFlags)
        {
            if (featureFlags == null)
                throw new ArgumentNullException(nameof(featureFlags));
            _featureFlags = featureFlags;
        }
        public bool Can(Feature feature)
        {
            if (feature == null)
                throw new ArgumentNullException(nameof(feature));

            return feature.Type == FeatureType.Collection;
        }

        public bool Do(Feature feature, string comparison)
        {
            if (feature == null)
                throw new ArgumentNullException(nameof(feature));
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));

            string[] activeRegions = _featureFlags.Regions.Keys.ToArray();

            if (feature.Active)
            {
                bool isRegionActive = activeRegions.Contains(comparison);

                if (isRegionActive)
                {
                    DateTime featureActiveFromTime = _featureFlags.Regions[comparison];
                    return DateTime.Now > featureActiveFromTime;
                }
            }
            return false;
        }
    }
```

**Step 10.** Now we want to create a static constants class to store the key and description of our feature flags.

```csharp
public static class FeatureConstants
{
    public static string BulkBuyKey => "BulkBuyKey";
    public static string BulkBuyDescription => "Ability to quickly add 10 of one item to the cart";
}
```

**Step1 11.** Add the following section to your config.json, set any date time string for now as we will change this later.

```json
{
    ...
    "FeatureFlags": {
        "Region": {
            "New Zealand": "2016-11-04T12:20:29",
            "Australia": "2016-11-04T12:25:29"
        }
    }
}
```


**Step 12.** Now we need to manage our dependencies. Navigate to Startup.cs

```csharp

    public void ConfigureServices(IServiceCollection services)
    {
        ...
        // This will get the configuration section out of the config.json file
        IEnumerable<IConfigurationSection> configurationSections = Configuration.GetSection("FeatureFlags:Region").GetChildren();

        // This will map the config.json region values to a dictionary<string, Date>
        Dictionary<string, DateTime> regions = configurationSections.ToDictionary(config => config.Key, config => DateTime.Parse(config.Value));

        FeatureFlags featureFlags = new FeatureFlags(regions);

        // This will make our feature flags available across the application
        services.AddScoped<IFeatureFlags>(
            p => featureFlags);
        
        // Here is where we will define our feature flag strategies and bind them to the feature manager
        var strategies = new List<IFeatureStrategy> { 
            new FeatureRegionStrategy()
        };
        var features = new List<Feature>
        {
            new Feature(FeatureConstants.BulkBuyKey, FeatureConstants.BulkBuyDescription, true, FeatureType.Collection)
        };
        services.AddScoped<IFeatureManager>(m => new FeatureManager(features, strategies));
        ...
    }
```

**Step 13.** We need to store a users location in order to apply our region flag correctly. Navigate to ApplicationUser.cs and add the 'Location' property as seen below.

```csharp
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public string Location { get; set; }
}
```

**Step 14.** Navigate to ManageViewModels.cs and look inside the IndexViewModel class. We need to add the location property here as well.

```csharp
public class IndexViewModel
{
    ...
    public string Location { get; set; }
    public IFeatureFlags FeatureFlags { get; set; }
}
```

**Step 15.** Navigate to the ManageController class and add the **IFeatureManager** to the constructor.

```csharp
[Authorize]
public class ManageController : Controller
{
    private readonly IFeatureManager _featureManager;

    public ManageController(UserManager<ApplicationUser> userManager, 
    SignInManager<ApplicationUser> signInManager, 
    IFeatureManager featureManager, IFeatureFlags featureFlags)
    {
        _featureManager = featureManager;
        _featureFlags = featureFlags;
        UserManager = userManager;
        SignInManager = signInManager;
    }
    
    ...
}
```

**Step 16.** Now in the Index call we need to populate our location value with the correct value from the user.

```csharp
//
// GET: /Manage/Index
public async Task<ActionResult> Index(ManageMessageId? message = null)
{
    ...

    var model = new IndexViewModel
    {
        ...

        Location = user.Location,
        FeatureFlags = _featureFlags
    };

    return View(model);
}
```

**Step 17.** At the bottom of Index.cshtml inside the Views -> Manage folder we need add the following at the bottom of the page **just inside the last closing </dl> tag**.

```csharp
...
<dt>Location:</dt>
<dd>
    @using (Html.BeginForm("AddLocale", "Manage", FormMethod.Post, new { @class = "form-horizontal", role = "form" }))
    {
        @Html.AntiForgeryToken()

        Feature selectList = Model.Features.First(f => f.Type == FeatureType.Collection);
        @Html.DropDownListFor(x => x.Location, Model.FeatureFlags.Regions.Keys.ToArray().Select(i => new SelectListItem { Text = i, Value = i, Selected = (i == Model.Location) }))
        <span>[ <input type="submit" value="Save location" class="btn btn-link" /> ]</span>
    }
</dd>
...
```

**Step 18.** Now we need to ensure that when a user clicks the 'save location' button it actually saves! Navigate to the ManageController again and create a new POST method.

```csharp
[Authorize]
public class ManageController : Controller
{
    ...

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLocation(IndexViewModel indexViewModel)
    {
        if (indexViewModel == null)
            throw new ArgumentNullException(nameof(indexViewModel));

        var user = await GetCurrentUserAsync();
        user.Location = indexViewModel.Location;
        await UserManager.UpdateAsync(user);
        return RedirectToAction("Index");
    }
}
```

**Step 19.** We need a way to bulk add items to a users cart. Navigate to ShoppingCart.cs

```csharp
public void BulkAddToCart(Product product)
{
    const int bulkItemAmount = 10;
    // Get the matching cart and product instances
    var cartItem = _db.CartItems.SingleOrDefault(
        c => c.CartId == ShoppingCartId
        && c.ProductId == product.ProductId);

    if (cartItem == null)
    {
        // Create a new cart item if no cart item exists
        cartItem = new CartItem
        {
            ProductId = product.ProductId,
            CartId = ShoppingCartId,
            Count = bulkItemAmount,
            DateCreated = DateTime.Now
        };

        _db.CartItems.Add(cartItem);
    }
    else
    {
        // If the item does exist in the cart, then add one to the quantity
        cartItem.Count += bulkItemAmount;
    }
}
```

**Step 20.** Now lets add the actual feature toggle on the Details.cshtml file located under Views->Store

```csharp
// this a tag should already exist!
<a href="@Url.Action("AddToCart", "ShoppingCart", new { id = Model.ProductId })" class="btn">Add to Cart</a>

// new code to add
@{
    if (ViewBag.IsFeatureActive)
    {
        <a href="@Url.Action("BulkAddToCart", "ShoppingCart", new { id = Model.ProductId })" class="btn">Bulk Add (10) to Cart</a>
    }
}
```

### Task 2: Try it out!

**Step 1.** Before launching the site, locate the config.json file and alter the region active times. Make the New Zealand region five minutes from now and the Australian region 10 minutes from now.

**Step 2.** Now launch the site. You can do this but pressing f5 or hitting the button shown below in Visual Studio. 

![](<media/iis-express.png>)

![](<media/splash.png>)

**Step 3.** Now log in with any account. 

Or aternatively you can use the administrator account. This can be found in config.json.

```json
"AdminRole": {
    "UserName": "Administrator@test.com",
    "Password": "YouShouldChangeThisPassword1!"
}
```

**Step 4.** Select 'Manage Account' at the top right corner.

![](<media/manage.png>)

**Step 5.** Now select 'New Zealand' and then save it.

![](<media/location.png>)

**Step 6.** Now navigate to the 'Breaks' section from the main navigation.

![](<media/breaks.png>)

Notice the bulk buy is not avaiable yet. This should change in a couple of minutes.

![](<media/break-no-bulk.png>)

**Step 7.** Once the current time is past the time set in the config.json for New Zealand. Refresh the page and you should see the bulk buy option. The same thing will happen 5 minutes later for any Australian users.

![](<media/break-bulk.png>)

In this lab you have learned how to implement time and region based feature flags for a web application. This gives you the ability stagger releases to assist with distributing load or potentially A/B testing in regions that are more accustomed to change.