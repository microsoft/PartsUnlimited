# How to Use Entity Framework Migrations

The Parts Unlimited model project is setup to use [EntityFramework 7](https://github.com/aspnet/EntityFramework).  

## Set up Environment##
1. Open a new command prompt.
2. Navigate to the src\PartsUnlimited.Models directory.
3. Run `dnvm upgrade` to install necessary ASP.NET 5 tools, if not done already.
4. Run `dnu restore` to load all the packages needed by PartsUnlimited.Models project.

## Setup Connection String##
Entity Framework requires a connection string to the database to which the migrations will be applied.  There are a couple places Entity Framework looks to find the connection string.  The first is the OnConfiguring method in the DbContext for the project, ProductsUnlimitedContext in this case.  Connection strings may also be specified the ConfigureServices method of the Startup class, which is what PartsUnlimited.Models has done.  Our ConfigureServices method will look for connection strings in the config.json file or environment variables.

1. Open src\PartsUnlimited.Models\config.json.
2. Add a entry for DefaultConnection as shown below

```
    "Data": {
        "DefaultConnection": {
            "ConnectionString": "Server=tcp:{server}.database.windows.net,1433;Database={database};User ID={AdminUserName};Password={password};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
        }
    }
```

3. Save the file.

##How to Add a Migration
From the command prompt run `dnx ef migrations add MyMigration`

Migrations will be added to the database to described in the DefaultConnection configuration entry.  The name of the migration must  be unique.

##How to Apply a Migration
From the command prompt, run `dnx ef migrations apply`.  The command must be run from the directory where project.json resides.

Entity Framework will inspect the database specified in the DefaultConnection connection string and apply any missing migrations.  PartsUnlimited.Models has an InitialMigration already added.  Migration information can be seen under src\PartsUnlimited.Models\Migrations.

Applying a migration does not automatically seed the database with data.  The PartsUnlimited website is configured to input sample data into the database only if the tables are empty as is the case when apply the initial migration for PartsUnlimited.Models.

##Migration Commands
There are several commands for EntityFramework, including listing available migrations, applying, removing and adding migrations.  Run `dnx ef migrations` to see a list of available commands.
