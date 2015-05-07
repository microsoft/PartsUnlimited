// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Builder;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System;

namespace PartsUnlimited.Models
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
        }


        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration config = new Configuration()
                        .AddJsonFile("config.json")
                        .AddEnvironmentVariables();

            var sqlConnectionString = config.Get("Data:DefaultConnection:ConnectionString");
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
