// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Builder;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Dnx.Runtime;
using System;

namespace PartsUnlimited.Models
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
            var sqlConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];
            if (!String.IsNullOrEmpty(sqlConnectionString))
            {
                services.AddEntityFramework()
                        .AddSqlServer()
                        .AddDbContext<PartsUnlimitedContext>(options =>
                        {
                            options.UseSqlServer(sqlConnectionString);
                        });
            }
        }
    }
}
