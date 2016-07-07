// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using PartsUnlimited.Areas.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace PartsUnlimited.Models
{
    public static class SampleData
    {
        private static string AdminRoleSectionName = "AdminRole";
        private static string DefaultAdminNameKey = "UserName";
        private static string DefaultAdminPasswordKey = "Password";

        public static async Task InitializePartsUnlimitedDatabaseAsync(IServiceProvider serviceProvider, bool createUser = true)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<PartsUnlimitedContext>();


                bool dbNewlyCreated = await db.Database.EnsureCreatedAsync();

                //Seeding a database using migrations is not yet supported. (https://github.com/aspnet/EntityFramework/issues/629)
                //Add seed data, only if the tables are empty.
                bool tablesEmpty = !db.Products.Any() && !db.Orders.Any() && !db.Categories.Any() && !db.Stores.Any();

                if (dbNewlyCreated || tablesEmpty)
                {
                    await InsertTestData(serviceProvider);
                    await CreateAdminUser(serviceProvider);
                }
            }
        }

        public static async Task InsertTestData(IServiceProvider serviceProvider)
        {
            var categories = GetCategories().ToList();
            await AddOrUpdateAsync(serviceProvider, g => g.Name, categories);

            var products = GetProducts(categories).ToList();
            await AddOrUpdateAsync(serviceProvider, a => a.Title, products);

            var stores = GetStores().ToList();
            await AddOrUpdateAsync(serviceProvider, a => a.Name, stores);

            var rainchecks = GetRainchecks(stores, products).ToList();
            await AddOrUpdateAsync(serviceProvider, a => a.RaincheckId, rainchecks);

            PopulateOrderHistory(serviceProvider, products);
        }

        private static async Task AddOrUpdateAsync<TEntity>(
            IServiceProvider serviceProvider,
            Func<TEntity, object> propertyToMatch, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            // Query in a separate context so that we can attach existing entities as modified
            List<TEntity> existingData;
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<PartsUnlimitedContext>();
                existingData = db.Set<TEntity>().ToList();
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
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

       /// <summary>
       /// Returns configuration section for AdminRole.
       /// </summary>
       /// <param name="serviceProvider"></param>
       /// <returns></returns>
       private static IConfigurationSection GetAdminRoleConfiguration(IServiceProvider serviceProvider)
        {
            var appEnv = serviceProvider.GetService<IHostingEnvironment>();

            var builder = new ConfigurationBuilder().SetBasePath(appEnv.ContentRootPath)
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

        /// <summary>
        /// Generate an enumeration of rainchecks.  The random number generator uses a seed to ensure 
        /// that the sequence is consistent, but provides somewhat random looking data.
        /// </summary>
        public static IEnumerable<Raincheck> GetRainchecks(IEnumerable<Store> stores, IList<Product> products)
        {
            var random = new Random(1234);

            foreach (var store in stores)
            {
                for (var i = 0; i < random.Next(1, 5); i++)
                {
                    yield return new Raincheck
                    {
                        StoreId = store.StoreId,
                        Name = $"John Smith{random.Next()}",
                        Quantity = random.Next(1, 10),
                        ProductId = products[random.Next(0, products.Count)].ProductId,
                        SalePrice = Math.Round(100 * random.NextDouble(), 2)
                    };
                }
            }
        }

        public static IEnumerable<Store> GetStores()
        {
            return Enumerable.Range(1, 20).Select(id => new Store { Name = $"Store{id}" });
        }

        public static IEnumerable<Category> GetCategories()
        {
            yield return new Category { Name = "Brakes", Description = "Brakes description", ImageUrl = "product_brakes_disc.jpg" };
            yield return new Category { Name = "Lighting", Description = "Lighting description", ImageUrl = "product_lighting_headlight.jpg" };
            yield return new Category { Name = "Wheels & Tires", Description = "Wheels & Tires description", ImageUrl = "product_wheel_rim.jpg" };
            yield return new Category { Name = "Batteries", Description = "Batteries description", ImageUrl = "product_batteries_basic-battery.jpg" };
            yield return new Category { Name = "Oil", Description = "Oil description", ImageUrl = "product_oil_premium-oil.jpg" };
        }

        public static void PopulateOrderHistory(IServiceProvider serviceProvider, IEnumerable<Product> products)
        {
            var random = new Random(1234);
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

            IConfigurationSection configuration = GetAdminRoleConfiguration(serviceProvider);
            string userName = configuration[DefaultAdminNameKey];

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<PartsUnlimitedContext>();

                var orders = new List<Order>();
                foreach (var combination in recomendationCombinations)
                {
                    for (int i = 0; i < combination.Multiplier; i++)
                    {
                        var order = new Order
                        {
                            Username = userName,
                            OrderDate = DateTime.Now,
                            Name = $"John Smith{random.Next()}",
                            Address = "15010 NE 36th St",
                            City = "Redmond",
                            State = "WA",
                            PostalCode = "98052",
                            Country = "United States",
                            Phone = "425-703-6214",
                            Email = userName
                        };

                        db.Orders.Add(order);
                        decimal total = 0;
                        foreach (var id in combination.Transactions)
                        {
                            var product = products.Single(x => x.RecommendationId == id);
                            var orderDetail = GetOrderDetail(product, order);
                            db.OrderDetails.Add(orderDetail);
                            total += orderDetail.UnitPrice;
                        }

                        order.Total = total;
                    }
                }

                db.SaveChanges();
            }
        }

        public static OrderDetail GetOrderDetail(Product product, Order order)
        {
            var random = new Random();
            int quantity;
            switch (product.Category.Name)
            {
                case "Brakes":
                case "Wheels & Tires":
                    {
                        quantity = random.Next(1, 5);
                        break;
                    }
                default:
                    {
                        quantity = random.Next(1, 3);
                        break;
                    }
            }

            return new OrderDetail
            {
                ProductId = product.ProductId,
                UnitPrice = product.Price,
                OrderId = order.OrderId,
                Quantity = quantity,
            };
        }

        public static IEnumerable<Product> GetProducts(IEnumerable<Category> categories)
        {
            var categoriesMap = categories.ToDictionary(c => c.Name, c => c);

            yield return new Product
            {
                SkuNumber = "LIG-0001",
                Title = "Halogen Headlights (2 Pack)",
                Category = categoriesMap["Lighting"],
                CategoryId = categoriesMap["Lighting"].CategoryId,
                Price = 38.99M,
                SalePrice = 38.99M,
                ProductArtUrl = "product_lighting_headlight.jpg",
                ProductDetails = "{ \"Light Source\" : \"Halogen\", \"Assembly Required\": \"Yes\", \"Color\" : \"Clear\", \"Interior\" : \"Chrome\", \"Beam\": \"low and high\", \"Wiring harness included\" : \"Yes\", \"Bulbs Included\" : \"No\",  \"Includes Parking Signal\" : \"Yes\"}",
                Description = "Our Halogen Headlights are made to fit majority of vehicles with our  universal fitting mold. Product requires some assembly.",
                Inventory = 10,
                LeadTime = 0,
                RecommendationId = 1
            };

            yield return new Product
            {
                SkuNumber = "LIG-0002",
                Title = "Bugeye Headlights (2 Pack)",
                Category = categoriesMap["Lighting"],
                CategoryId = categoriesMap["Lighting"].CategoryId,
                Price = 48.99M,
                SalePrice = 48.99M,
                ProductArtUrl = "product_lighting_bugeye-headlight.jpg",
                ProductDetails = "{ \"Light Source\" : \"Halogen\", \"Assembly Required\": \"Yes\", \"Color\" : \"Clear\", \"Interior\" : \"Chrome\", \"Beam\": \"low and high\", \"Wiring harness included\" : \"No\", \"Bulbs Included\" : \"Yes\",  \"Includes Parking Signal\" : \"Yes\"}",
                Description = "Our Bugeye Headlights use Halogen light bulbs are made to fit into a standard bugeye slot. Product requires some assembly and includes light bulbs.",
                Inventory = 7,
                LeadTime = 0,
                RecommendationId = 2
            };

            yield return new Product
            {
                SkuNumber = "LIG-0003",
                Title = "Turn Signal Light Bulb",
                Category = categoriesMap["Lighting"],
                CategoryId = categoriesMap["Lighting"].CategoryId,
                Price = 6.49M,
                SalePrice = 6.49M,
                ProductArtUrl = "product_lighting_lightbulb.jpg",
                ProductDetails = "{ \"Color\" : \"Clear\", \"Fit\" : \"Universal\", \"Wattage\" : \"30 Watts\", \"Includes Socket\" : \"Yes\"}",
                Description = " Clear bulb that with a universal fitting for all headlights/taillights.  Simple Installation, low wattage and a clear light for optimal visibility and efficiency.",
                Inventory = 18,
                LeadTime = 0,
                RecommendationId = 3
            };

            yield return new Product
            {
                SkuNumber = "WHE-0001",
                Title = "Matte Finish Rim",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                Price = 75.99M,
                SalePrice = 75.99M,
                ProductArtUrl = "product_wheel_rim.jpg",
                ProductDetails = "{ \"Material\" : \"Aluminum alloy\",  \"Design\" : \"Spoke\", \"Spokes\" : \"9\",  \"Number of Lugs\" : \"4\", \"Wheel Diameter\" : \"17 in.\", \"Color\" : \"Black\", \"Finish\" : \"Matte\" } ",
                Description = "A Parts Unlimited favorite, the Matte Finish Rim is affordable low profile style. Fits all low profile tires.",
                Inventory = 4,
                LeadTime = 0,
                RecommendationId = 4
            };

            yield return new Product
            {
                SkuNumber = "WHE-0002",
                Title = "Blue Performance Alloy Rim",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                Price = 88.99M,
                SalePrice = 88.99M,
                ProductArtUrl = "product_wheel_rim-blue.jpg",
                ProductDetails = "{ \"Material\" : \"Aluminum alloy\",  \"Design\" : \"Spoke\", \"Spokes\" : \"5\",  \"Number of Lugs\" : \"4\", \"Wheel Diameter\" : \"18 in.\", \"Color\" : \"Blue\", \"Finish\" : \"Glossy\" } ",
                Description = "Stand out from the crowd with a set of aftermarket blue rims to make you vehicle turn heads and at a price that will do the same.",
                Inventory = 8,
                LeadTime = 0,
                RecommendationId = 5
            };

            yield return new Product
            {
                SkuNumber = "WHE-0003",
                Title = "High Performance Rim",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                Price = 99.99M,
                SalePrice = 99.49M,
                ProductArtUrl = "product_wheel_rim-red.jpg",
                ProductDetails = "{ \"Material\" : \"Aluminum alloy\",  \"Design\" : \"Spoke\", \"Spokes\" : \"12\",  \"Number of Lugs\" : \"5\", \"Wheel Diameter\" : \"18 in.\", \"Color\" : \"Red\", \"Finish\" : \"Matte\" } ",
                Description = "Light Weight Rims with a twin cross spoke design for stability and reliable performance.",
                Inventory = 3,
                LeadTime = 0,
                RecommendationId = 6
            };

            yield return new Product
            {
                SkuNumber = "WHE-0004",
                Title = "Wheel Tire Combo",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                Price = 72.49M,
                SalePrice = 72.49M,
                ProductArtUrl = "product_wheel_tyre-wheel-combo.jpg",
                ProductDetails = "{ \"Material\" : \"Steel\",  \"Design\" : \"Spoke\", \"Spokes\" : \"8\",  \"Number of Lugs\" : \"4\", \"Wheel Diameter\" : \"19 in.\", \"Color\" : \"Gray\", \"Finish\" : \"Standard\", \"Pre-Assembled\" : \"Yes\" } ",
                Description = "For the endurance driver, take advantage of our best wearing tire yet. Composite rubber and a heavy duty steel rim.",
                Inventory = 0,
                LeadTime = 4,
                RecommendationId = 7
            };

            yield return new Product
            {
                SkuNumber = "WHE-0005",
                Title = "Chrome Rim Tire Combo",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                Price = 129.99M,
                SalePrice = 129.99M,
                ProductArtUrl = "product_wheel_tyre-rim-chrome-combo.jpg",
                ProductDetails = "{ \"Material\" : \"Aluminum alloy\",  \"Design\" : \"Spoke\", \"Spokes\" : \"10\",  \"Number of Lugs\" : \"5\", \"Wheel Diameter\" : \"17 in.\", \"Color\" : \"Silver\", \"Finish\" : \"Chrome\", \"Pre-Assembled\" : \"Yes\" } ",
                Description = "Save time and money with our ever popular wheel and tire combo. Pre-assembled and ready to go.",
                Inventory = 1,
                LeadTime = 0,
                RecommendationId = 8
            };

            yield return new Product
            {
                SkuNumber = "WHE-0006",
                Title = "Wheel Tire Combo (4 Pack)",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                Price = 219.99M,
                SalePrice = 219.99M,
                ProductArtUrl = "product_wheel_tyre-wheel-combo-pack.jpg",
                ProductDetails = "{ \"Material\" : \"Steel\",  \"Design\" : \"Spoke\", \"Spokes\" : \"8\",  \"Number of Lugs\" : \"5\", \"Wheel Diameter\" : \"19 in.\", \"Color\" : \"Gray\", \"Finish\" : \"Standard\", \"Pre-Assembled\" : \"Yes\" } ",
                Description = "Having trouble in the wet? Then try our special patent tire on a heavy duty steel rim. These wheels perform excellent in all conditions but were designed specifically for wet weather.",
                Inventory = 3,
                LeadTime = 0,
                RecommendationId = 9
            };

            yield return new Product
            {
                SkuNumber = "BRA-0001",
                Title = "Disk and Pad Combo",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                Price = 25.99M,
                SalePrice = 25.99M,
                ProductArtUrl = "product_brakes_disk-pad-combo.jpg",
                ProductDetails = "{ \"Disk Design\" : \"Cross Drill Slotted\", \" Pad Material\" : \"Ceramic\", \"Construction\" : \"Vented Rotor\", \"Diameter\" : \"10.3 in.\", \"Finish\" : \"Silver Zinc Plated\", \"Hat Finish\" : \"Silver Zinc Plated\", \"Material\" : \"Cast Iron\" }",
                Description = "Our brake disks and pads perform the best togeather. Better stopping distances without locking up, reduced rust and dusk.",
                Inventory = 0,
                LeadTime = 6,
                RecommendationId = 10
            };

            yield return new Product
            {
                SkuNumber = "BRA-0002",
                Title = "Brake Rotor",
                Category = categoriesMap["Brakes"],
                CategoryId = categoriesMap["Brakes"].CategoryId,
                Price = 18.99M,
                SalePrice = 18.99M,
                ProductArtUrl = "product_brakes_disc.jpg",
                ProductDetails = "{ \"Disk Design\" : \"Cross Drill Slotted\",  \"Construction\" : \"Vented Rotor\", \"Diameter\" : \"10.3 in.\", \"Finish\" : \"Silver Zinc Plated\", \"Hat Finish\" : \"Black E-coating\",  \"Material\" : \"Cast Iron\" }",
                Description = "Our Brake Rotor Performs well in wet coditions with a smooth responsive feel. Machined to a high tolerance to ensure all of our Brake Rotors are safe and reliable.",
                Inventory = 4,
                LeadTime = 0,
                RecommendationId = 11
            };

            yield return new Product
            {
                SkuNumber = "BRA-0003",
                Title = "Brake Disk and Calipers",
                Category = categoriesMap["Brakes"],
                CategoryId = categoriesMap["Brakes"].CategoryId,
                Price = 43.99M,
                SalePrice = 43.99M,
                ProductArtUrl = "product_brakes_disc-calipers-red.jpg",
                ProductDetails = "{\"Disk Design\" : \"Cross Drill Slotted\", \" Pad Material\" : \"Carbon Ceramic\",  \"Construction\" : \"Vented Rotor\", \"Diameter\" : \"11.3 in.\", \"Bolt Pattern\": \"6 x 5.31 in.\", \"Finish\" : \"Silver Zinc Plated\",  \"Material\" : \"Carbon Alloy\", \"Includes Brake Pads\" : \"Yes\" }",
                Description = "Upgrading your brakes can increase stopping power, reduce dust and noise. Our Disk Calipers exceed factory specification for the best performance.",
                Inventory = 2,
                LeadTime = 0,
                RecommendationId = 12
            };

            yield return new Product
            {
                SkuNumber = "BAT-0001",
                Title = "12-Volt Calcium Battery",
                Category = categoriesMap["Batteries"],
                CategoryId = categoriesMap["Batteries"].CategoryId,
                Price = 129.99M,
                SalePrice = 129.99M,
                ProductArtUrl = "product_batteries_basic-battery.jpg",
                ProductDetails = "{ \"Type\": \"Calcium\", \"Volts\" : \"12\", \"Weight\" : \"22.9 lbs\", \"Size\" :  \"7.7x5x8.6\", \"Cold Cranking Amps\" : \"510\" }",
                Description = "Calcium is the most common battery type. It is durable and has a long shelf and service life. They also provide high cold cranking amps.",
                Inventory = 9,
                LeadTime = 0,
                RecommendationId = 13
            };

            yield return new Product
            {
                SkuNumber = "BAT-0002",
                Title = "Spiral Coil Battery",
                Category = categoriesMap["Batteries"],
                CategoryId = categoriesMap["Batteries"].CategoryId,
                Price = 154.99M,
                SalePrice = 154.99M,
                ProductArtUrl = "product_batteries_premium-battery.jpg",
                ProductDetails = "{ \"Type\": \"Spiral Coil\", \"Volts\" : \"12\", \"Weight\" : \"20.3 lbs\", \"Size\" :  \"7.4x5.1x8.5\", \"Cold Cranking Amps\" : \"460\" }",
                Description = "Spiral Coil batteries are the preferred option for high performance Vehicles where extra toque is need for starting. They are more resistant to heat and higher charge rates than conventional batteries.",
                Inventory = 3,
                LeadTime = 0,
                RecommendationId = 14
            };

            yield return new Product
            {
                SkuNumber = "BAT-0003",
                Title = "Jumper Leads",
                Category = categoriesMap["Batteries"],
                CategoryId = categoriesMap["Batteries"].CategoryId,
                Price = 16.99M,
                SalePrice = 16.99M,
                ProductArtUrl = "product_batteries_jumper-leads.jpg",
                ProductDetails = "{ \"length\" : \"6ft.\", \"Connection Type\" : \"Alligator Clips\", \"Fit\" : \"Universal\", \"Max Amp's\" : \"750\" }",
                Description = "Battery Jumper Leads have a built in surge protector and a includes a plastic carry case to keep them safe from corrosion.",
                Inventory = 6,
                LeadTime = 0,
                RecommendationId = 15
            };

            yield return new Product
            {
                SkuNumber = "OIL-0001",
                Title = "Filter Set",
                Category = categoriesMap["Oil"],
                CategoryId = categoriesMap["Oil"].CategoryId,
                Price = 28.99M,
                SalePrice = 28.99M,
                ProductArtUrl = "product_oil_filters.jpg",
                ProductDetails = "{ \"Filter Type\" : \"Canister and Cartridge\", \"Thread Size\" : \"0.75-16 in.\", \"Anti-Drainback Valve\" : \"Yes\"}",
                Description = "Ensure that your vehicle's engine has a longer life with our new filter set. Trapping more dirt to ensure old freely circulates through your engine.",
                Inventory = 3,
                LeadTime = 0,
                RecommendationId = 16
            };

            yield return new Product
            {
                SkuNumber = "OIL-0002",
                Title = "Oil and Filter Combo",
                Category = categoriesMap["Oil"],
                CategoryId = categoriesMap["Oil"].CategoryId,
                Price = 34.49M,
                SalePrice = 34.49M,
                ProductArtUrl = "product_oil_oil-filter-combo.jpg",
                ProductDetails = "{ \"Filter Type\" : \"Canister\", \"Thread Size\" : \"0.75-16 in.\", \"Anti-Drainback Valve\" : \"Yes\", \"Size\" : \"1.1 gal.\", \"Synthetic\" : \"No\" }",
                Description = "This Oil and Oil Filter combo is suitable for all types of passenger and light commercial vehicles. Providing affordable performance through excellent lubrication and breakdown resistance.",
                Inventory = 5,
                LeadTime = 0,
                RecommendationId = 17
            };

            yield return new Product
            {
                SkuNumber = "OIL-0003",
                Title = "Synthetic Engine Oil",
                Category = categoriesMap["Oil"],
                CategoryId = categoriesMap["Oil"].CategoryId,
                Price = 36.49M,
                SalePrice = 36.49M,
                ProductArtUrl = "product_oil_premium-oil.jpg",
                ProductDetails = "{ \"Size\" :  \"1.1 Gal.\" , \"Synthetic \" : \"Yes\"}",
                Description = "This Oil is designed to reduce sludge deposits and metal friction throughout your cars engine. Provides performance no matter the condition or temperature.",
                Inventory = 11,
                LeadTime = 0,
                RecommendationId = 18
            };
        }
    }
}
