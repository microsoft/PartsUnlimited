using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Data.Entity;

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
                        .AddJsonFile("Config.json")
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
