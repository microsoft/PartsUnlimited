using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using PartsUnlimited.Areas.Admin;

namespace PartsUnlimited.Models
{
    public class SQLDataSeeder : IDataSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private static string AdminRoleSectionName = "AdminRole";
        private static string DefaultAdminNameKey = "UserName";
        private static string DefaultAdminPasswordKey = "Password";

        public SQLDataSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Seed(SampleData data)
        {
            await SeedInternal(data, null);
        }

        public async Task Seed(SampleData data, Func<IEnumerable<Category>, Task<IEnumerable<IProduct>>> loadProducts)
        {
            await SeedInternal(data, loadProducts);
        }

        private async Task SeedInternal(SampleData data, Func<IEnumerable<Category>, Task<IEnumerable<IProduct>>> products)
        {
            bool createProductsInSQl= products == null;

            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<PartsUnlimitedContext>();
                bool dbNewlyCreated = await db.Database.EnsureCreatedAsync();

                //Seeding a database using migrations is not yet supported. (https://github.com/aspnet/EntityFramework/issues/629.)
                //Add seed data, only if the tables are empty.
                bool tablesEmpty = !db.Products.Any() && !db.Orders.Any() && !db.Categories.Any() && !db.Stores.Any() && !db.Promo.Any();

                if (dbNewlyCreated || tablesEmpty)
                {
                    await InsertReferenceData(data);
                    IEnumerable<IProduct> createdProducts;
                    if (createProductsInSQl)
                    {
                        createdProducts = await InsertProductData(data, db.Categories.AsEnumerable());
                    }
                    else
                    {
                        createdProducts = await products.Invoke(db.Categories.AsEnumerable());
                    }
                    await InsertHistoricalData(data, _serviceProvider, createdProducts, db.Stores, db.Promo);
                    await CreateAdminUser(_serviceProvider);
                }
            }
        }

        /// <summary>
        /// Returns configuration section for AdminRole.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private static IConfigurationSection GetAdminRoleConfiguration(IServiceProvider serviceProvider)
        {
            var appEnv = serviceProvider.GetService<IApplicationEnvironment>();

            var builder = new ConfigurationBuilder().SetBasePath(appEnv.ApplicationBasePath)
                        .AddJsonFile("config.json")
                        .AddEnvironmentVariables();
            var configuration = builder.Build();
            return configuration.GetSection(AdminRoleSectionName);
        }

        /// <summary>
        /// Creates a store manager user who can manage the inventory.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            IConfigurationSection configuration = GetAdminRoleConfiguration(serviceProvider);
            UserManager<ApplicationUser> userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(configuration[DefaultAdminNameKey]);

            if (user == null)
            {
                user = new ApplicationUser { UserName = configuration[DefaultAdminNameKey] };
                await userManager.CreateAsync(user, configuration[DefaultAdminPasswordKey]);
                await userManager.AddClaimAsync(user, new Claim(AdminConstants.ManageStore.Name, AdminConstants.ManageStore.Allowed));
            }
        }

        private async Task<IEnumerable<IProduct>> InsertProductData(SampleData data, IEnumerable<Category> categories)
        {
            var seededProducts = data.GetProducts(categories).ToList();
            var typedProducts = seededProducts.OfType<Product>();
            await AddOrUpdateAsync(a => a.Title, typedProducts);
            return typedProducts;
        }

        private async Task InsertReferenceData(SampleData data)
        {
            var promo = data.GetPromo().ToList();
            await AddOrUpdateAsync(a => a.PromoId, promo);

            var categories = data.GetCategories().ToList();
            await AddOrUpdateAsync(g => g.Name, categories);

            var stores = data.GetStores().ToList();
            await AddOrUpdateAsync(a => a.Name, stores);
        }

        private async Task InsertHistoricalData(SampleData data, IServiceProvider serviceProvider, IEnumerable<IProduct> products, IEnumerable<Store> stores, IEnumerable<Promo> promos)
        {
            var workingProducts = products.ToList();
            var rainchecks = data.GetRainchecks(stores, workingProducts).ToList();
            await AddOrUpdateAsync(a => a.RaincheckId, rainchecks);

            PopulateOrderHistory(serviceProvider, workingProducts, promos.ToList(), data);
        }

        private void PopulateOrderHistory(IServiceProvider serviceProvider, IList<IProduct> products, IList<Promo> promo, SampleData data)
        {
            IConfigurationSection configuration = GetAdminRoleConfiguration(serviceProvider);
            string userName = configuration[DefaultAdminNameKey];

            var recomendationCombinations = new[] {
                new{ Transactions = new []{1, 3, 8}, Multiplier = 60 },
                new{ Transactions = new []{2, 6}, Multiplier = 10 },
                new{ Transactions = new []{4, 11}, Multiplier = 20 },
                new{ Transactions = new []{5, 14}, Multiplier = 10 },
                new{ Transactions = new []{6, 16, 18}, Multiplier = 20 },
                new{ Transactions = new []{7, 17}, Multiplier = 25 },
                new{ Transactions = new []{8, 1}, Multiplier = 5 },
                new{ Transactions = new []{10, 17,9}, Multiplier = 15 },
                new{ Transactions = new []{11, 5}, Multiplier = 15 },
                new{ Transactions = new []{12, 8}, Multiplier = 5 },
                new{ Transactions = new []{13, 15}, Multiplier = 50 },
                new{ Transactions = new []{14, 15}, Multiplier = 30 },
                new{ Transactions = new []{16, 18}, Multiplier = 80 }
            };

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<PartsUnlimitedContext>();

                foreach (var item in recomendationCombinations)
                {
                    var orders = data.BuildOrders(item.Multiplier, userName, promo);

                    foreach (var ord in orders)
                    {
                        db.Add(ord);
                        var orderDetails = data.BuildOrderDetail(item.Transactions, ord, products);

                        foreach (var orderDetail in orderDetails)
                        {
                            db.Add(orderDetail);
                        }

                    }
                }

                db.SaveChanges();
            }
        }

        private async Task AddOrUpdateAsync<TEntity>(
            Func<TEntity, object> propertyToMatch, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            // Query in a separate context so that we can attach existing entities as modified
            List<TEntity> existingData;
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<PartsUnlimitedContext>();
                existingData = db.Set<TEntity>().ToList();
            }

            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<PartsUnlimitedContext>();
                foreach (var item in entities)
                {
                    db.Entry(item).State = existingData.Any(g => propertyToMatch(g).Equals(propertyToMatch(item)))
                        ? EntityState.Modified
                        : EntityState.Added;
                }
                await db.SaveChangesAsync();
            }
        }
    }
}