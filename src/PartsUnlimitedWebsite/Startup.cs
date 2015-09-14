// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using PartsUnlimited.Areas.Admin;
using PartsUnlimited.Models;
using PartsUnlimited.Queries;
using PartsUnlimited.Recommendations;
using PartsUnlimited.Search;
using PartsUnlimited.Security;
using PartsUnlimited.Telemetry;
using PartsUnlimited.WebsiteConfiguration;
using System;

namespace PartsUnlimited
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IApplicationEnvironment env)
        {
            //Below code demonstrates usage of multiple configuration sources. For instance a setting say 'setting1' is found in both the registered sources, 
            //then the later source will win. By this way a Local config can be overridden by a different setting while deployed remotely.
            var builder = new ConfigurationBuilder(env.ApplicationBasePath)
                        .AddJsonFile("config.json")
                        .AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //If this type is present - we're on mono
            var runningOnMono = Type.GetType("Mono.Runtime") != null;
            var sqlConnectionString = Configuration.Get("Data:DefaultConnection:ConnectionString");
            var useInMemoryDatabase = string.IsNullOrWhiteSpace(sqlConnectionString);

            // Add EF services to the services container
            if (useInMemoryDatabase || runningOnMono)
            {
                services.AddEntityFramework()
                        .AddInMemoryStore()
                        .AddDbContext<PartsUnlimitedContext>(options =>
                        {
                            options.UseInMemoryStore();
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
            services.ConfigureAuthorization(auth =>
            {
                auth.AddPolicy(AdminConstants.Role,
                    new AuthorizationPolicyBuilder()
                        .RequireClaim(AdminConstants.ManageStore.Name, AdminConstants.ManageStore.Allowed)
                        .Build());
            });

            // Add implementations
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddScoped<IOrdersQuery, OrdersQuery>();
            services.AddScoped<IRaincheckQuery, RaincheckQuery>();

            services.AddSingleton<ITelemetryProvider, EmptyTelemetryProvider>();
            services.AddScoped<IProductSearch, StringContainsProductSearch>();

            SetupRecommendationService(services);

            services.AddScoped<IWebsiteOptions>(p =>
            {
                var telemetry = p.GetRequiredService<ITelemetryProvider>();

                return new ConfigurationWebsiteOptions(Configuration.GetConfigurationSection("WebsiteOptions"), telemetry);
            });

            services.AddScoped<IApplicationInsightsSettings>(p =>
            {
                return new ConfigurationApplicationInsightsSettings(Configuration.GetConfigurationSection("Keys:ApplicationInsights"));
            });

            // Associate IPartsUnlimitedContext with context
            services.AddTransient<IPartsUnlimitedContext>(s => s.GetService<PartsUnlimitedContext>());

            // We need access to these settings in a static extension method, so DI does not help us :(
            ContentDeliveryNetworkExtensions.Configuration = new ContentDeliveryNetworkConfiguration(Configuration.GetConfigurationSection("CDN"));

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
            var azureMlConfig = new AzureMLFrequentlyBoughtTogetherConfig(Configuration.GetConfigurationSection("Keys:AzureMLFrequentlyBoughtTogether"));

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

        //This method is invoked when KRE_ENV is 'Development' or is not defined
        //The allowed values are Development,Staging and Production
        public void ConfigureDevelopment(IApplicationBuilder app)
        {
            //Display custom error page in production when error occurs
            //During development use the ErrorPage middleware to display error information in the browser
            app.UseErrorPage(ErrorPageOptions.ShowAll);

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
            app.UseErrorHandler("/Home/Error");
            Configure(app);
        }

        //This method is invoked when KRE_ENV is 'Production'
        //The allowed values are Development,Staging and Production
        public void ConfigureProduction(IApplicationBuilder app)
        {
            app.UseErrorHandler("/Home/Error");
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
            app.AddLoginProviders(new ConfigurationLoginProviders(Configuration.GetConfigurationSection("Authentication")));

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

            SampleData.InitializePartsUnlimitedDatabaseAsync(
                app.ApplicationServices.GetService<PartsUnlimitedContext>,
                app.ApplicationServices.GetService<UserManager<ApplicationUser>>(),
                Configuration.GetConfigurationSection("AdminRole")
                ).Wait();
        }
    }
}