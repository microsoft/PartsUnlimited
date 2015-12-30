// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartsUnlimited.Areas.Admin;
using PartsUnlimited.Models;
using PartsUnlimited.Queries;
using PartsUnlimited.Recommendations;
using PartsUnlimited.Search;
using PartsUnlimited.Security;
using PartsUnlimited.Telemetry;
using PartsUnlimited.WebsiteConfiguration;
using System;
using PartsUnlimited.Cache;
using PartsUnlimited.Repository;

namespace PartsUnlimited
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            //Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
            //then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //If this type is present - we're on mono
            var runningOnMono = Type.GetType("Mono.Runtime") != null;
            var sqlConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];
            var useInMemoryDatabase = string.IsNullOrWhiteSpace(sqlConnectionString);

            // Add EF services to the services container
            if (useInMemoryDatabase || runningOnMono)
            {
                services.AddEntityFramework()
                        .AddInMemoryDatabase()
                        .AddDbContext<PartsUnlimitedContext>(options =>
                        {
                            options.UseInMemoryDatabase();
                        });
            }
            else
            {
                services.AddEntityFramework()
                        .AddSqlServer()
                        .AddDbContext<PartsUnlimitedContext>(options =>
                        {
                            options.UseSqlServer(sqlConnectionString);
                        });
            }

            // Add Identity services to the services container
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<PartsUnlimitedContext>()
                .AddDefaultTokenProviders();


            // Configure admin policies
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(AdminConstants.Role,
                    authBuilder =>
                    {
                        authBuilder.RequireClaim(AdminConstants.ManageStore.Name, AdminConstants.ManageStore.Allowed);
                    });

            });

            // Add implementations            
            SetupCache(services);

            SetupAzureStorage(services);
            SetupVisionApi(services);

            SetupRepository(services);
            services.AddScoped<IOrdersQuery, OrdersQuery>();
            services.AddScoped<IRaincheckQuery, RaincheckQuery>();

            services.AddSingleton<ITelemetryProvider, EmptyTelemetryProvider>();
            services.AddScoped<IProductSearch, StringContainsProductSearch>();

            SetupRecommendationService(services);

            services.AddScoped<IWebsiteOptions>(p =>
            {
                var telemetry = p.GetRequiredService<ITelemetryProvider>();

                return new ConfigurationWebsiteOptions(Configuration.GetSection("WebsiteOptions"), telemetry);
            });

            services.AddScoped<IApplicationInsightsSettings>(p =>
            {
                return new ConfigurationApplicationInsightsSettings(Configuration.GetSection("Keys:ApplicationInsights"));
            });

            // Associate IPartsUnlimitedContext with context
            services.AddTransient<IPartsUnlimitedContext>(s => s.GetService<PartsUnlimitedContext>());

            // We need access to these settings in a static extension method, so DI does not help us :(
            ContentDeliveryNetworkExtensions.Configuration = new ContentDeliveryNetworkConfiguration(Configuration.GetSection("CDN"));

            // Add MVC services to the services container
            services.AddMvc();

            //Add all SignalR related services to IoC.
            services.AddSignalR();

            //Add InMemoryCache
            services.AddSingleton<IMemoryCache, MemoryCache>();

            // Add session related services.
            services.AddCaching();
            services.AddSession();
        }

        private void SetupRecommendationService(IServiceCollection services)
        {
            var azureMlConfig = new AzureMLFrequentlyBoughtTogetherConfig(Configuration.GetSection("Keys:AzureMLFrequentlyBoughtTogether"));

            // If keys are not available for Azure ML recommendation service, register an empty recommendation engine
            if (string.IsNullOrEmpty(azureMlConfig.AccountKey) || string.IsNullOrEmpty(azureMlConfig.ModelName))
            {
                services.AddSingleton<IRecommendationEngine, EmptyRecommendationsEngine>();
            }
            else
            {
                services.AddSingleton<IAzureMLAuthenticatedHttpClient, AzureMLAuthenticatedHttpClient>();
                services.AddInstance<IAzureMLFrequentlyBoughtTogetherConfig>(azureMlConfig);
                services.AddScoped<IRecommendationEngine, AzureMLFrequentlyBoughtTogetherRecommendationEngine>();
            }
        }

        private void SetupCache(IServiceCollection services)
        {
            var redisConfig = new RedisCacheConfig(Configuration.GetSection("Keys:RedisCache"));
            services.AddSingleton<IMemoryCache, MemoryCache>();

            // If keys are not available for Redis Cache use in memory cache as primary cache.
            if (string.IsNullOrEmpty(redisConfig.AccessKey) || string.IsNullOrEmpty(redisConfig.HostName))
            {
                services.AddSingleton<IPartsUnlimitedCache, PartsUnlimitedMemoryCache>();
            }
            else
            {
                services.AddSingleton<PartsUnlimitedMemoryCache>();
                services.AddSingleton<PartsUnlimitedRedisCache>();
                services.AddInstance<IRedisCacheConfiguration>(redisConfig);
                services.AddSingleton<IPartsUnlimitedCache, PartUnlimitedMultilevelCache>();
            }

            services.AddSingleton<ICacheCoordinator, CacheCoordinator>();
        }

        private void SetupAzureStorage(IServiceCollection services)
        {
            var docDbConfig = new DocDbConfiguration(Configuration.GetSection("Keys:DocDb"));
            var storageConfig = new AzureStorageConfiguration(Configuration.GetSection("Keys:AzureStorage"), docDbConfig);
            services.AddInstance<IAzureStorageConfiguration>(storageConfig);
        }

        private void SetupVisionApi(IServiceCollection services)
        {
            var visionApiConfig = new VisionApiConfiguration(Configuration.GetSection("Keys:VisionApi"));
            services.AddInstance<IVisionApiConfiguration>(visionApiConfig);
        }

        private void SetupRepository(IServiceCollection services)
        {
            var docDbConfig = new DocDbConfiguration(Configuration.GetSection("Keys:DocDb"));
            services.AddSingleton<SqlProductRepository>();
            services.AddScoped<ICategoryLoader, CategoryLoader>();

            if (string.IsNullOrEmpty(docDbConfig.URI)
                || string.IsNullOrEmpty(docDbConfig.Key))
            {    
                services.AddScoped<IProductRepository, SqlProductRepository>();
                services.AddScoped<IProductLoader, SqlProductRepository>();
                services.AddScoped<IDataSeeder, SQLDataSeeder>();
            }
            else
            {
                services.AddScoped<SQLDataSeeder>();
                services.AddInstance<IDocDbConfiguration>(docDbConfig);
                services.AddScoped<DocDbProductRepository>();
                services.AddScoped<IProductRepository, DocDbProductRepository>();
                services.AddScoped<IProductLoader, DocDbProductRepository>();
                services.AddScoped<IDataSeeder, DocDbSeeder>();
            }
        }

        //This method is invoked when KRE_ENV is 'Development' or is not defined
        //The allowed values are Development,Staging and Production
        public void ConfigureDevelopment(IApplicationBuilder app)
        {
            //Display custom error page in production when error occurs
            //During development use the ErrorPage middleware to display error information in the browser
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage(DatabaseErrorPageExtensions.EnableAll);

            // Add the runtime information page that can be used by developers
            // to see what packages are used by the application
            // default path is: /runtimeinfo
            app.UseRuntimeInfoPage();

            Configure(app);
        }

        //This method is invoked when KRE_ENV is 'Staging'
        //The allowed values are Development,Staging and Production
        public void ConfigureStaging(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/Home/Error");
            Configure(app);
        }

        //This method is invoked when KRE_ENV is 'Production'
        //The allowed values are Development,Staging and Production
        public void ConfigureProduction(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/Home/Error");
            Configure(app);
        }

        public void Configure(IApplicationBuilder app)
        {
            // Configure Session.
            app.UseSession();

            //Configure SignalR
            app.UseSignalR();

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline
            app.UseIdentity();

            // Add login providers (Microsoft/AzureAD/Google/etc).  This must be done after `app.UseIdentity()`
            app.AddLoginProviders(new ConfigurationLoginProviders(Configuration.GetSection("Authentication")));

            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}",
                    defaults: new { action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "api",
                    template: "{controller}/{id?}");
            });

            //Populates the PartsUnlimited sample data
            var dataSeeder = app.ApplicationServices.GetService<IDataSeeder>();
            var data = new SampleData();
            dataSeeder.Seed(data).Wait();
        }
    }
}