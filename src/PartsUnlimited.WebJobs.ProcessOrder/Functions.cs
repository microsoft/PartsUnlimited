// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PartsUnlimited.Models;

namespace PartsUnlimited.WebJobs.ProcessOrder
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public static void CreateOrderProcessTask([Queue("orders")] CloudQueue orderQueue)
        {
            Console.WriteLine("Starting Create Order Process Task");
            try
            {
                var builder = new ConfigurationBuilder();
                builder.Add(new JsonConfigurationSource { Path = "config.json" });
                var config = builder.Build();
                var connectionString = config["Data:DefaultConnection:ConnectionString"];

                using (var context = new PartsUnlimitedContext(connectionString))
                {
                    var orders = context.Orders.Where(x => !x.Processed).ToList();
                    Console.WriteLine("Found {0} orders to process", orders.Count);

                    foreach (var order in orders)
                    {
                        var productIds = context.OrderDetails.Where(x => x.OrderId == order.OrderId).Select(x => x.ProductId).ToList();
                        var items = context.Products
                            .Where(x => productIds.Contains(x.ProductId))
                            .ToList();

                        var orderItems = items.Select(
                                x => new LegacyOrderItem
                                {
                                    SkuNumber = x.SkuNumber,
                                    Price = x.Price
                                }).ToList();

                        var queueOrder = new LegacyOrder
                        {
                            Address = order.Address,
                            Country = order.Country,
                            City = order.City,
                            Phone = order.Phone,
                            CustomerName = order.Name,
                            OrderDate = order.OrderDate,
                            PostalCode = order.PostalCode,
                            State = order.State,
                            TotalCost = order.Total,
                            Discount = order.Total,
                            Items = orderItems
                        };
                        var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                        var message = JsonConvert.SerializeObject(queueOrder, settings);
                        orderQueue.AddMessageAsync(new CloudQueueMessage(message));
                        order.Processed = true;
                    }
                    context.SaveChanges();
                    Console.WriteLine("Orders successfully processed.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}