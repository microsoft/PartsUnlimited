HOL - Feature Flag for Web Applications
====================================================================================
Feature flags provide the ability to turn features of your application on and off at a moments notice, no code deployments required. With feature flags you can test your features in production, getting early feedback from a subset of production users, and incrementally enable it to different sets of users. Once the feature is enabled for everyone and becomes part of the product, the flag code can be removed.
In this lab you will add the foundation for feature flags to the PartsUnlimited project, and implement a simple feature flag for phone number validation. 
The users will be able to self-subscribe to this feature. For your own projects you can decide how the flags are enabled and to which customers. 

Note: [Launch Darkly](https://launchdarkly.com/) is a great option for more advanced feature flag management. Version 3.0.0 of the Launch Darkly SDK supports .NET core.

### Pre-requisites: ###
- Visual Studio 2015 Update 3 or higher

### Tasks Overview: ###
**Task 1. Add the backend code required for feature flags** - in this task we will add in the required code to enable feature flags for our application.

**Task 2. Create a new page under the user section for users to enable/disable features** - in this task we will add a section for users to opt in for specific features.

**Task 3. Try it out!** - in this task we will try out our feature flag to ensure it's working as expected.

###Task 1: Add the backend code required for feature flags

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

Or navigate to where you cloned the repository to e.g. `C:\Source\Repos\PartsUnlimited` with explorer and double click on PartsUnlimited.sln

**Step 3.** Let's create a folder for our feature flag related classes. On the PartsUnlimitedWebsite solution, right click and create a new folder. We can call it something like 'FeatureFlag'.

![](<media/new-folder.png>)

**Step 4.** Now we will create the first feature flag class, this will represent a feature flag in our application. Right click on the newly created **FeatureFlag** folder -> select **'Add'** -> select **'Class...'**.

![](<media/new-class.png>)

**Step 5.** Name the new class **'Feature.cs'** then select **'Add'**

![](<media/modal-class.png>)

**Step 6.** Add the following properties for the Feature.cs class

```csharp
public class Feature
{
    public string Key { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
}
```

- **Key** is going to be the unique identifier for that particular feature flag.
- **Description** is going to be a brief description of what the feature flag is for.
- **Active** is going to be the current state for the feature flag. 

**Step 7.** We also want to add a property in ApplicationUser.cs called **ActiveFeatures**

```csharp
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }

    public string ActiveFeatures { get; set; }
}
```

**Step 8.** Now we want to create a simple feature manager class. This will be used later on to toggle our features on and off. Under the same FeatureFlag folder create a new class called **FeatureManager.cs**

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PartsUnlimited.Models;

public interface IFeatureManager
{
    Task<bool> GetStatusByKey(string key, ClaimsPrincipal userClaim);
    Task ChangeFeatureToggles(string[] selectedItems, ClaimsPrincipal userClaim);
    Task<IEnumerable<Feature>> GetUserFeatures(ClaimsPrincipal userClaim);
}

public class FeatureManager : IFeatureManager
{
    private readonly UserManager<ApplicationUser> _userManager;

    public FeatureManager(UserManager<ApplicationUser> userManager)
    {
        if (userManager == null)
            throw new ArgumentNullException(nameof(userManager));
        _userManager = userManager;
    }

    public static IEnumerable<Feature> Features = new List<Feature>
    {
        new Feature
        {
            Description = "New screen for adding phone numbers",
            Key = PhoneNumberKey,
            Active = false
        }
    };

    public async Task<IEnumerable<Feature>> GetUserFeatures(ClaimsPrincipal userClaim)
    {
        ApplicationUser applicationUser = await _userManager.GetUserAsync(userClaim);

        IEnumerable<string> applicationUserActiveFeatures = !string.IsNullOrEmpty(applicationUser.ActiveFeatures) ? applicationUser.ActiveFeatures.Split(',') : Enumerable.Empty<string>();

        return applicationUserActiveFeatures.Select(feature => Features.First(f => f.Key == feature)).ToList();
    }

    public async Task<bool> GetStatusByKey(string key, ClaimsPrincipal userClaim)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        ApplicationUser applicationUser = await _userManager.GetUserAsync(userClaim);

        return !string.IsNullOrEmpty(applicationUser.ActiveFeatures) && applicationUser.ActiveFeatures.Split(',').Any(f => f == key);
    }

    public async Task ChangeFeatureToggles(string[] selectedItems, ClaimsPrincipal userClaim)
    {
        if (selectedItems == null)
            throw new ArgumentNullException(nameof(selectedItems));

        ApplicationUser applicationUser = await _userManager.GetUserAsync(userClaim);

        applicationUser.ActiveFeatures = selectedItems.Any() ? string.Join(",", selectedItems) : string.Empty;

        await _userManager.UpdateAsync(applicationUser);
    }

    public const string PhoneNumberKey = "Phone-Number";

}
```

Now let's explain the sections of the above code. 

- **Features** is a list of features we want to use with the application.
- **GetStatusByKey** is a method we will use to get the current status for a given user for a particular feature key.
- **ChangeFeatureToggles** takes in a list of selected feature toggles and a user. If the key exists that status will be toggled for that user.
- **GetUserFeatures** will get the current list of active features for a given user.

*Note: In a real world implementation we would store these flags in a database. For simplicity's sake we have stored them in memory. This also gives you the ability to extenalize the on/off switch for your features*

We also want to set up our dependency injection for the FeatureManager. Navigate to Startup.cs and add the service binding shown below.

```csharp
...
using PartsUnlimited.FeatureFlag;

public class Startup
{
    ...

    public void ConfigureServices(IServiceCollection services) {
        ...

        services.AddScoped<IFeatureManager, FeatureManager>();
    }
    ...
}
```

**Step 9.** Now that we have the basics set up let's create our first actual feature. We want to display a different placeholder and utilize HTML5 validation for updating phone numbers. In ManageViewModels.cs we want to add an additional properties on the AddPhoneNumberViewModel class. This will be used on the view to check if the new phone number feature is active.

```csharp
    ...
    public class AddPhoneNumberViewModel
    {
        ...
        public bool IsPhoneFeatureActive { get; set; }
    }
    ...
```

In ManageController.cs we want to alter two of the existing methods. This is located here -> `.\PartsUnlimited\src\PartsUnlimitedWebsite\Controllers\ManageController.cs`

Note the `AddPhoneNumber()` method already exists but we need to change a few things. This needs to become an async method that returns `Task<IActionResult>`. Replace the whole method with the one below.

Also for the `AddPhoneNumber(AddPhoneNumberViewModel model)` we will be replacing the existing return statement with the one displayed below.

```csharp
using System;
...
using PartsUnlimited.FeatureFlag;

public class ManageController : Controller
{
    private readonly IFeatureManager _featureManager;

    public ManageController(..., IFeatureManager featureManager)
    {
        if (featureManager == null)
            throw new ArgumentNullException(nameof(featureManager));
        _featureManager = featureManager;
        ...
    }

    ...

    //
    // GET: /Account/AddPhoneNumber
    public async Task<IActionResult> AddPhoneNumber()
    {
        var statusByKey = await _featureManager.GetStatusByKey(FeatureManager.PhoneNumberKey, HttpContext.User);
        return View(new AddPhoneNumberViewModel { IsPhoneFeatureActive = statusByKey });
    }

    //
    // POST: /Account/AddPhoneNumber
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
    {
        ...
        var statusByKey = await _featureManager.GetStatusByKey(FeatureManager.PhoneNumberKey, HttpContext.User);
        return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number, IsPhoneFeatureActive = statusByKey });
    }
}
```

**Step 10.** Now we want to conditionally display a block of code relating to the new feature. Navigate to AddPhoneNumber.cshtml

Find the following line of code in the same file (**AddPhoneNumber.cshtml**), located at line 20.

```csharp
@Html.TextBoxFor(m => m.Number, new { @class = "form-control", placeholder = "Phone Number" })
```
Replace it with this code block

```csharp    
@{
    if (Model.IsPhoneFeatureActive)
    {
        @Html.TextBoxFor(m => m.Number, new { @class = "form-control", placeholder = "555-555-5555", type = "tel", pattern = "\\d{3}[\\-]\\d{3}[\\-]\\d{4}", id = "phone" })
    }
    else
    {
        @Html.TextBoxFor(m => m.Number, new { @class = "form-control", placeholder = "Phone Number" })
    }
}
```

The key difference here is inside the input tag properties 

- **placeholder = "555-555-5555"** this will display a sample number for the display to assist users in knowing what format is required.
- **type = "tel"** this is the new HTML5 input type for phone numbers
- **pattern = "\\d{3}[\\-]\\d{3}[\\-]\\d{4}"** this is a regex patter to ensure the user types in a phone number with the format 555-555-5555

At the bottom of the file, under the closing section tag, add the following 

```csharp
@{
    if (Model.IsPhoneFeatureActive)
    {
        <script>
            (function() {
                var phone = document.getElementById("phone");

                phone.addEventListener("keyup", function () {
                    if (phone.validity.patternMismatch) {
                        phone.setCustomValidity("Please enter the correct phone number format e.g. 555-555-5555");
                    } else {
                        phone.setCustomValidity("");
                    }
                });
            })();
        </script>
    }
}
```


**Step 11.** We also want to make sure that the proper CSS is applied for the HTML5 validation. Navigate to **Site.scss** (located in `./PartsUnlimited/src/PartsUnlimitedWebsite/Content/Site.scss`)

**Step 12.** Add the following to Site.scss - this will ensure a red border is applied when the pattern or input field type is invalid.

    input:invalid {border:3px solid red;}
    input[pattern]:invalid{border:3px solid red;}

###Task 2: Create a new page under the user section for users to enable/disable features
Now we need to create an administration view to toggle these features on and off from the site.

**Step 1.** In order to create a new page on the site, we need to create a ViewModel that will represent the data we need to use on that page. 

Under the `./PartsUnlimited/src/PartsUnlimitedWebsite/ViewModels` folder, create a new class called FeaturesViewModel.cs

```csharp
using System.Collections.Generic;
using PartsUnlimited.FeatureFlag;

namespace PartsUnlimited.ViewModels
{
    public class FeaturesViewModel
    {
        public IEnumerable<Feature> AvailableFeatures { get; set; }
        public IEnumerable<Feature> UserFeatures { get; set; }
    }
}

```

**Step 2.** Navigate to `./PartsUnlimited/src/PartsUnlimitedWebsite/Controllers` and create a new controller called **FeaturesController.cs**

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PartsUnlimited.FeatureFlag;
using PartsUnlimited.ViewModels;

namespace PartsUnlimited.Controllers
{
    public class FeaturesController : Controller
    {
        private readonly IFeatureManager _featureManager;

        public FeaturesController(IFeatureManager featureManager)
        {
            if (featureManager == null)
                throw new ArgumentNullException(nameof(featureManager));
            _featureManager = featureManager;
        }

        public async Task<IActionResult> Index()
        {
            FeaturesViewModel featuresViewModel = new FeaturesViewModel
            {
                UserFeatures = await _featureManager.GetUserFeatures(HttpContext.User),
                AvailableFeatures = FeatureManager.Features
            };

            return View(featuresViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> SaveFeatureToggles(string[] selectedItems)
        {
            await _featureManager.ChangeFeatureToggles(selectedItems, HttpContext.User);

            return RedirectToAction("Index", "Features");
        }
    }
}
```

**Step 3.** Now to tie everything together we need a view. Navigate to PartsUnlimitedWebsite -> Views and create a new folder called **'Features'**

**Step 4.** Inside the folder you just created, right click and select 'Add'. Now add a View called **Index.cshtml**. Insert the contents shown below in to this new file.

![](<media/add-view.png>)

![](<media/add-view-dialog.png>)

```csharp
@model PartsUnlimited.ViewModels.FeaturesViewModel
<br />
<div class="panel panel-primary" style="width:100%">
    <div class="panel-heading">
        <span style="font-size: 20px; font-style: oblique"><span><span style="margin-right: 5px" class="glyphicon glyphicon-star"></span>Enable or disable features</span></span>
    </div>

    @using (Html.BeginForm("SaveFeatureToggles", "Features", FormMethod.Post))
    {
        <table class="table table-striped table-hover table-responsive table-condensed">
            <tr>
                <th>
                    <h4><span style="font-weight: bolder">Description</span></h4>
                </th>
                <th>
                    <h4><span style="font-weight: bolder">Key</span></h4>
                </th>
                <th>
                    <h4><span style="font-weight: bolder">Active</span></h4>
                </th>
                <th></th>
            </tr>

            @foreach (var item in Model.AvailableFeatures)
            {
                <tr>
                    <td class="col-lg-4">
                        <span style="font-size: 17px;">@item.Description</span>
                    </td>
                    <td class="col-lg-4">
                        <span style="font-size: 17px;">@item.Key</span>
                    </td>
                    <td class="col-lg-4">
                        <div>
                            @Html.AntiForgeryToken()
                            <input type="checkbox" name="selectedItems" value="@item.Key" checked="@Model.UserFeatures.Any(a => a.Key == item.Key)">
                        </div>
                    </td>
                </tr>
            }
        </table>

        <div><input type="submit" value="Save" style="float: right;" /></div>
    }


</div>
<br /><br />
```

**Step 5.** We need to link to our new page. Navigate to PartsUnlimitedWebsite -> Views -> Shared -> _Login.cshtml

Add the following list item to the bottom of the menu.

```csharp
<ul>
    ...
    <li> @Html.ActionLink("View Features", "Index", "Features", new { area = string.Empty }) </li>  
</ul>
```

###Task 3: Try it out!

**Step 1.** Now launch the site. You can do this but pressing f5 or hitting the button shown below in Visual Studio. 

![](<media/iis-express.png>)

![](<media/splash.png>)

**Step 2.** Now log in with the administrator account. This can be found in config.json.

```json
"AdminRole": {
    "UserName": "Administrator@test.com",
    "Password": "YouShouldChangeThisPassword1!"
}
```

![](<media/login.png>)

**Step 3.** Navigate to the page we created earlier under Profile -> View Features. 

![](<media/view-features.png>)

**Step 4.** Now we can see our active feature flags displayed in a list. We can also toggle them on and off from here!

![](<media/view-features-unticked.png>)

**Step 5.** In your browser navigate back to the home page.

![](<media/home.png>)

**Step 6.** Select Profile -> Manage Account.

![](<media/manage-account.png>)

**Step 7.** Now we need to modify our phone number. Select 'Add' next to the 'Phone Number' label.

![](<media/add-phone.png>)

![](<media/placeholder-before.png>)

![](<media/error-submit-before.png>)

![](<media/after-submit.png>)

**Step 8.** Make sure you saved the changes, then navigate back to the feature flag page (Profile -> View Features). Now toggle the flag to be on. Let's see if our changes are pushed through.

**Step 9.** Navigate back to the user management page (Profile -> Manage Account)

Notice the placeholder is different to the one before.

![](<media/placeholder.png>)

Also when you start typing a red border appears around the input. This will continue until you supply a valid input similar to the placeholder pattern.

![](<media/text-error.png>)

Try clicking submit, you will notice the error dialog pop up asking the user to enter the correct format.

![](<media/validation.png>)

Now when we add the correct format the red border will disappear.

![](<media/valid-number.png>)

Next steps
----------
In this lab you have learned how to implement feature flags in a web application. This gives you the ability to seamlessly manage features in your application. You can give users access to beta functionality to give you better test coverage and deliver continuous improvement.

- [Deployment to Azure](https://github.com/Microsoft/PartsUnlimited/tree/master/docs/Deployment.md)

- [Controlling exposure through feature flags in VS Team Services](https://blogs.msdn.microsoft.com/buckh/2016/09/30/controlling-exposure-through-feature-flags-in-vs-team-services/)