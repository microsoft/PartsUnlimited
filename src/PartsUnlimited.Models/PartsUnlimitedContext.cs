// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PartsUnlimited.Models
{
    public class PartsUnlimitedContext : IdentityDbContext<ApplicationUser>, IPartsUnlimitedContext
    {
        private readonly string _connectionString;

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
            builder.Entity<Product>().Ignore(a => a.ProductDetailList).HasKey(a => a.ProductId);
            builder.Entity<Order>().HasKey(o => o.OrderId);
            builder.Entity<Category>().HasKey(g => g.CategoryId);
            builder.Entity<CartItem>().HasKey(c => c.CartItemId);
            builder.Entity<OrderDetail>().HasKey(o => o.OrderDetailId);
            builder.Entity<Raincheck>().HasKey(o => o.RaincheckId);
            builder.Entity<Store>().HasKey(o => o.StoreId);

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }else
            {
                System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(_connectionString);
                optionsBuilder.UseInMemoryDatabase("Test");
            }
        }
    }
}