// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace PartsUnlimited.Models
{
    public class PartsUnlimitedContext : IdentityDbContext<ApplicationUser>, IPartsUnlimitedContext
    {
        private readonly string _connectionString;

        public PartsUnlimitedContext()
        {
        }

        public PartsUnlimitedContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Raincheck> RainChecks { get; set; }
        public DbSet<Store> Stores { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().Key(a => a.ProductId);
            builder.Entity<Order>().Key(o => o.OrderId);
            builder.Entity<Category>().Key(g => g.CategoryId);
            builder.Entity<CartItem>().Key(c => c.CartItemId);
            builder.Entity<OrderDetail>().Key(o => o.OrderDetailId);
            builder.Entity<Raincheck>().Key(o => o.RaincheckId);
            builder.Entity<Store>().Key(o => o.StoreId);

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(EntityOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
}