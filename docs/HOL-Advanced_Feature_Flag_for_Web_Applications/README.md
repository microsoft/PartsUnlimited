HOL - Advanced Feature Flag for Web Applications
====================================================================================

There's a big sales initiative coming up next month to assist with a much anticipated product release. In the past infrastructure stability has been a massive concern and we have lost many customers with downtime and slow server response times. This time around we have a plan to stagger the release to different regions at different times to try distribute the load. You have been tasked with implementing the solution and will take advantage of feature flags to accomplish it.

Note: [Launch Darkly](https://launchdarkly.com/) is a great option for more advanced feature flag management. Version 3.0.0 of the Launch Darkly SDK supports .NET core.

### Pre-requisites: ###
- Visual Studio 2015 Update 3 or higher

- [.Net Core 1.0.1 SDK](https://www.microsoft.com/net/core#windows) installed

### Tasks Overview: ###

1. Add the back-end code required for feature flags - In this task we will go through the steps required to create advanced feature flags which are region and time based.

2. Try it out! - In this task we will see our feature flag in action, simulating a staggered release to countries.

### Task 1: Add the back-end code required for feature flags

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
namespace PartsUnlimited.FeatureFlag
{
    public enum FeatureType
    {
        Default,
        Region
    }
}
```

**Step 6.** Add another class in the same folder called `Feature.cs`. This will define the structure of our features.

```csharp
namespace PartsUnlimited.FeatureFlag
{
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
}
```

- **Feature Type** is an enum which represents the feature type.
- **Active** is going to be the current state for the feature flag.
- **Description** is going to be a brief description of what the feature flag is for.
- **Key** is going to be the unique identifier for that particular feature flag.

**Step 7.** We need an interface we will use to template all our different feature toggle types. Let's call this `IFeatureFlagStrategy.cs` and store it in the same folder we've been using -> `.\PartsUnlimited\src\PartsUnlimitedWebsite\FeatureFlag`. This interface will have two method signatures: 'Can' and 'Do'. 'Can' will be used to decide which strategy should be run. 'Do' will perform the actual work required for the feature flag. Create this in the same 'FeatureFlag' folder we created before.

```csharp

namespace PartsUnlimited.FeatureFlag
{
    public interface IFeatureStrategy
    {
        bool Can(Feature feature);

        bool Do(Feature feature, string comparison);
    }
}
```

**Step 8.** Now we want to create a feature manager class and interface. This will be used later on to toggle our features on and off. Let's call it `FeatureManager.cs` and store it here -> `.\PartsUnlimited\src\PartsUnlimitedWebsite\FeatureFlag\FeatureManager.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartsUnlimited.FeatureFlag
{
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
}

```

Now let's explain the sections of the above code.

- **Features** is a list of features we want to use with the application.
- **GetStatusByKey** is a method we will use to get the current status for a particular key. This will check to see if there's an existing strategy which can handle this feature flag.
- **ChangeFeatureToggles** takes in a list of selected feature toggles. If the key exists in any of our 'features' that status will be toggled.

*Note: It's a much better idea to store these flags in a database. For simplicity's sake we have stored them in memory.*

**Step 9.** Now lets create a region specific feature. The first file will be called `FeatureFlags.cs` and it will be responsible for storing the parameters required for specific features. This should be stored in the same FeatureFlag folder we created earlier `.\PartsUnlimited\src\PartsUnlimitedWebsite\FeatureFlag`.

```csharp

using System;
using System.Collections.Generic;

namespace PartsUnlimited.FeatureFlag
{
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
}

```

The second file will be called `FeatureRegionStrategy.cs` and will contain the core logic of our region specific feature flag. It should be created in the location as the previous file -> `.\PartsUnlimited\src\PartsUnlimitedWebsite\FeatureFlag`.

```csharp

using System;
using System.Linq;

namespace PartsUnlimited.FeatureFlag
{
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

            return feature.Type == FeatureType.Region;
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
}
```

**Step 10.** Now we want to create a static constants class to store the key and description of our feature flags. Let's call this `FeatureConstants.cs` and store it here -> `.\PartsUnlimited\src\PartsUnlimitedWebsite`

```csharp

namespace PartsUnlimited
{
    public static class FeatureConstants
    {
        public static string BulkBuyKey => "BulkBuyKey";
        public static string BulkBuyDescription => "Ability to quickly add 10 of one item to the cart";
    }
}
```

**Step 11.** Add the following section to your `config.json`, set any date time string for now as we will change this later. This file can be found here -> `.\PartsUnlimited\src\PartsUnlimitedWebsite\config.json`

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


**Step 12.** Now we need to manage our dependencies. Navigate to `Startup.cs` located here -> `.\PartsUnlimited\src\PartsUnlimitedWebsite\Startup.cs`. Add the following code below to the ConfigureServices method.

```csharp
...
using System.Collections.Generic;
using System.Linq;
using PartsUnlimited.FeatureFlag;

namespace PartsUnlimited
{
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
            new FeatureRegionStrategy(featureFlags)
        };
        var features = new List<Feature>
        {
            new Feature(FeatureConstants.BulkBuyKey, FeatureConstants.BulkBuyDescription, true, FeatureType.Region)
        };
        services.AddScoped<IFeatureManager>(m => new FeatureManager(features, strategies));
        ...
    }
}
```

**Step 13.** We need to store a user's location in order to apply our region flag correctly. Navigate to `.\PartsUnlimited\src\PartsUnlimited.Models\ApplicationUser.cs` and add the 'Location' property as seen below.

```csharp
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public string Location { get; set; }
}
```

**Step 14.** Navigate to `.\PartsUnlimited\src\PartsUnlimitedWebsite\Models\ManageViewModels.cs` and look inside the IndexViewModel class. We need to add the location and feature flags properties here.

```csharp
...
using PartsUnlimited.FeatureFlag;

public class IndexViewModel
{
    ...
    public string Location { get; set; }
    public IFeatureFlags FeatureFlags { get; set; }
}
```

**Step 15.** Navigate to the `.\PartsUnlimited\src\PartsUnlimitedWebsite\Controllers\ManageController.cs` class and add the **IFeatureManager** to the constructor.

```csharp
...
using PartsUnlimited.FeatureFlag;

...
[Authorize]
public class ManageController : Controller
{
    private readonly IFeatureManager _featureManager;
    private readonly IFeatureFlags _featureFlags;

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

**Step 16.** Now in the Index method inside `.\PartsUnlimited\src\PartsUnlimitedWebsite\Controllers\ManageController.cs` we need to populate our location value with the correct value from the user.

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

**Step 17.** Navigate to `Index.cshtml` inside the Views -> Manage folder, we then need add the following at the top of the file.

```csharp
@using System.Threading.Tasks
```

Then at the bottom of the page **just inside the last closing `</dl>` tag** we want to add the following code.

```csharp
...

<dt>Location:</dt>
<dd>
    @using (Html.BeginForm("AddLocation", "Manage", FormMethod.Post, new { @class = "form-horizontal", role = "form" }))
    {
        @Html.AntiForgeryToken()

        @Html.DropDownListFor(x => x.Location, Model.FeatureFlags.Regions.Keys.ToArray().Select(i => new SelectListItem { Text = i, Value = i, Selected = (i == Model.Location) }))

        <span>[ <input type="button" value="Save location" class="btn btn-link" data-toggle="modal" data-target="#confirmation-modal"/> ]</span>

        <div class="modal fade" id="confirmation-modal"  tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <p>Are you sure you wish to change your location?</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" style="float: left;" data-dismiss="modal">Close</button>
                        <button type="submit" class="btn btn-primary" style="float: left;">Save changes</button>
                    </div>
                </div><!-- /.modal-content -->
            </div><!-- /.modal-dialog -->
        </div><!-- /.modal -->
    }
</dd>
...
```

**Step 18.** Now we need to ensure that when a user clicks the 'save location' button it actually saves! Navigate to `.\PartsUnlimited\src\PartsUnlimitedWebsite\Controllers\ManageController.cs` again and create a new POST method.

```csharp
using System;
...

[Authorize]
public class ManageController : Controller
{
    ...

    //
    // POST: /Manage/AddLocation
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

**Step 19.** We need a way to bulk add items to a users cart. Navigate to `.\PartsUnlimited\src\PartsUnlimited.Models\ShoppingCart.cs` and add the following method.

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

**Step 20.** Let's wire up the required feature information to `StoreController.cs` located here -> `.\PartsUnlimited\src\PartsUnlimitedWebsite\Controllers\StoreController.cs`. We want to add the following code to the controller.

```csharp
...
using Microsoft.AspNetCore.Identity;
using PartsUnlimited.FeatureFlag;
using System.Threading.Tasks;

namespace PartsUnlimited.Controllers
{
    public class StoreController : Controller
    {
        ...
        private readonly IFeatureManager _featureManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoreController(IPartsUnlimitedContext context, IMemoryCache memoryCache, IFeatureManager featureManager, UserManager<ApplicationUser> userManager)
        {
            ...
            if (featureManager == null) throw new ArgumentNullException(nameof(featureManager));
            if (userManager == null) throw new ArgumentNullException(nameof(userManager));

            ...
            _featureManager = featureManager;
            _userManager = userManager;
        }

        ...

        public async Task<IActionResult> Details(int id)
        {
            Product productData;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(_userManager.GetUserId(HttpContext.User));
                ViewBag.IsFeatureActive = _featureManager.GetStatusByKey(FeatureConstants.BulkBuyKey, user.Location ?? string.Empty);
            }
            else
            {
                ViewBag.IsFeatureActive = false;
            }

            ...

            return View(productData);
        }
    }
}

```

We also want to include the bulk add method to `ShoppingCartController.cs` located here -> `.\PartsUnlimited\src\PartsUnlimitedWebsite\Controllers\ShoppingCartController.cs`.

```csharp

        //
        // GET: /ShoppingCart/BulkAddToCart/5
        public async Task<IActionResult> BulkAddToCart(int id)
        {
            // Retrieve the product from the database
            var addedProduct = _db.Products
                .Single(product => product.ProductId == id);

            // Start timer for save process telemetry
            var startTime = System.DateTime.Now;

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(_db, HttpContext);

            cart.BulkAddToCart(addedProduct);

            await _db.SaveChangesAsync(HttpContext.RequestAborted);

            // Trace add process
            var measurements = new Dictionary<string, double>()
            {
                {"ElapsedMilliseconds", System.DateTime.Now.Subtract(startTime).TotalMilliseconds }
            };
            _telemetry.TrackEvent("Cart/Server/Add", null, measurements);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

```


**Step 21.** Now lets add the actual feature toggle on the `Details.cshtml` file located here -> `.\PartsUnlimited\src\PartsUnlimitedWebsite\Views\Store\Details.cshtml`. Take note of the comments below - we want to find where the first section (under the first comment) of code is and replace it with the second section (under the second comment).

Look for the following tag below.

```csharp

<a href="@Url.Action("AddToCart", "ShoppingCart", new { id = Model.ProductId })" class="btn">Add to Cart</a>

```

Replace the code above with the code below.

```csharp

<a href="@Url.Action("AddToCart", "ShoppingCart", new { id = Model.ProductId })" class="btn">Add to Cart</a>

@{
    if (ViewBag.IsFeatureActive)
    {
        <a href="@Url.Action("BulkAddToCart", "ShoppingCart", new { id = Model.ProductId })" class="btn">Bulk Add (10) to Cart</a>
    }
}
```

### Task 2: Try it out!

**Step 1.** Before launching the site, locate the  `.\PartsUnlimited\src\PartsUnlimitedWebsite\config.json` file and alter the region active times. Make the New Zealand region five minutes from now and the Australian region 10 minutes from now.

**Step 2.** Now launch the site. You can do this but pressing f5 or hitting the button shown below in Visual Studio.

![](<media/iis-express.png>)

![](<media/splash.png>)

**Step 3.** Now log in with any account.

Or alternatively you can use the administrator account. This can be found in `.\PartsUnlimited\src\PartsUnlimitedWebsite\config.json`.

```json
"AdminRole": {
    "UserName": "Administrator@test.com",
    "Password": "YouShouldChangeThisPassword1!"
}
```

**Step 4.** Select 'Manage Account' at the top right corner.

![](<media/manage.png>)

**Step 5.** Now select 'Australia' and then save it.

![](<media/location.png>)

**Step 6.** Now navigate to the 'Breaks' section from the main navigation.

![](<media/breaks.png>)

Notice the bulk buy is not available yet. This should change in a couple of minutes.

![](<media/break-no-bulk.png>)

**Step 7.** Once the current time is past the time set in the `.\PartsUnlimited\src\PartsUnlimitedWebsite\config.json` for New Zealand. Refresh the page and you should see the bulk buy option. The same thing will happen 5 minutes later for any Australian users.

![](<media/break-bulk.png>)

In this lab you have learned how to implement time and region based feature flags for a web application. This gives you the ability stagger releases to assist with distributing load or potentially A/B testing in regions that are more accustomed to change.