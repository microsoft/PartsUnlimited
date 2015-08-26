// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using PartsUnlimited.Models;

namespace PartsUnlimited.Models.Migrations
{
    [ContextType(typeof(PartsUnlimitedContext))]
    partial class PartsUnlimitedContextModelSnapshot : ModelSnapshot
    {
        public override void BuildModel(ModelBuilder builder)
        {
            builder
                .Annotation("SqlServer:ValueGeneration", "Identity");
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();
                    
                    b.Property<string>("Name");
                    
                    b.Property<string>("NormalizedName");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetRoles");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("ClaimType");
                    
                    b.Property<string>("ClaimValue");
                    
                    b.Property<string>("RoleId");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetRoleClaims");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("ClaimType");
                    
                    b.Property<string>("ClaimValue");
                    
                    b.Property<string>("UserId");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetUserClaims");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ProviderKey")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ProviderDisplayName");
                    
                    b.Property<string>("UserId");
                    
                    b.Key("LoginProvider", "ProviderKey");
                    
                    b.Annotation("Relational:TableName", "AspNetUserLogins");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");
                    
                    b.Property<string>("RoleId");
                    
                    b.Key("UserId", "RoleId");
                    
                    b.Annotation("Relational:TableName", "AspNetUserRoles");
                });
            
            builder.Entity("PartsUnlimited.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .GenerateValueOnAdd();
                    
                    b.Property<int>("AccessFailedCount");
                    
                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();
                    
                    b.Property<string>("Email");
                    
                    b.Property<bool>("EmailConfirmed");
                    
                    b.Property<bool>("LockoutEnabled");
                    
                    b.Property<DateTimeOffset?>("LockoutEnd");
                    
                    b.Property<string>("Name");
                    
                    b.Property<string>("NormalizedEmail");
                    
                    b.Property<string>("NormalizedUserName");
                    
                    b.Property<string>("PasswordHash");
                    
                    b.Property<string>("PhoneNumber");
                    
                    b.Property<bool>("PhoneNumberConfirmed");
                    
                    b.Property<string>("SecurityStamp");
                    
                    b.Property<bool>("TwoFactorEnabled");
                    
                    b.Property<string>("UserName");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetUsers");
                });
            
            builder.Entity("PartsUnlimited.Models.CartItem", b =>
                {
                    b.Property<int>("CartItemId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("CartId");
                    
                    b.Property<int>("Count");
                    
                    b.Property<DateTime>("DateCreated");
                    
                    b.Property<int>("ProductId");
                    
                    b.Key("CartItemId");
                });
            
            builder.Entity("PartsUnlimited.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Description");
                    
                    b.Property<string>("ImageUrl");
                    
                    b.Property<string>("Name");
                    
                    b.Key("CategoryId");
                });
            
            builder.Entity("PartsUnlimited.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Address");
                    
                    b.Property<string>("City");
                    
                    b.Property<string>("Country");
                    
                    b.Property<string>("Email");
                    
                    b.Property<string>("Name");
                    
                    b.Property<DateTime>("OrderDate");
                    
                    b.Property<string>("Phone");
                    
                    b.Property<string>("PostalCode");
                    
                    b.Property<bool>("Processed");
                    
                    b.Property<string>("State");
                    
                    b.Property<decimal>("Total");
                    
                    b.Property<string>("Username");
                    
                    b.Key("OrderId");
                });
            
            builder.Entity("PartsUnlimited.Models.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<int>("OrderId");
                    
                    b.Property<int>("ProductId");
                    
                    b.Property<int>("Quantity");
                    
                    b.Property<decimal>("UnitPrice");
                    
                    b.Key("OrderDetailId");
                });
            
            builder.Entity("PartsUnlimited.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<int>("CategoryId");
                    
                    b.Property<DateTime>("Created");
                    
                    b.Property<string>("Description");
                    
                    b.Property<int>("Inventory");
                    
                    b.Property<int>("LeadTime");
                    
                    b.Property<decimal>("Price");
                    
                    b.Property<string>("ProductArtUrl");
                    
                    b.Property<string>("ProductDetails");
                    
                    b.Property<int>("RecommendationId");
                    
                    b.Property<decimal>("SalePrice");
                    
                    b.Property<string>("SkuNumber");
                    
                    b.Property<string>("Title");
                    
                    b.Key("ProductId");
                });
            
            builder.Entity("PartsUnlimited.Models.Raincheck", b =>
                {
                    b.Property<int>("RaincheckId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Name");
                    
                    b.Property<int>("ProductId");
                    
                    b.Property<int>("Quantity");
                    
                    b.Property<double>("SalePrice");
                    
                    b.Property<int>("StoreId");
                    
                    b.Key("RaincheckId");
                });
            
            builder.Entity("PartsUnlimited.Models.Store", b =>
                {
                    b.Property<int>("StoreId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Name");
                    
                    b.Key("StoreId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Reference("PartsUnlimited.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Reference("PartsUnlimited.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");
                    
                    b.Reference("PartsUnlimited.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
            
            builder.Entity("PartsUnlimited.Models.CartItem", b =>
                {
                    b.Reference("PartsUnlimited.Models.Product")
                        .InverseCollection()
                        .ForeignKey("ProductId");
                });
            
            builder.Entity("PartsUnlimited.Models.OrderDetail", b =>
                {
                    b.Reference("PartsUnlimited.Models.Order")
                        .InverseCollection()
                        .ForeignKey("OrderId");
                    
                    b.Reference("PartsUnlimited.Models.Product")
                        .InverseCollection()
                        .ForeignKey("ProductId");
                });
            
            builder.Entity("PartsUnlimited.Models.Product", b =>
                {
                    b.Reference("PartsUnlimited.Models.Category")
                        .InverseCollection()
                        .ForeignKey("CategoryId");
                });
            
            builder.Entity("PartsUnlimited.Models.Raincheck", b =>
                {
                    b.Reference("PartsUnlimited.Models.Product")
                        .InverseCollection()
                        .ForeignKey("ProductId");
                    
                    b.Reference("PartsUnlimited.Models.Store")
                        .InverseCollection()
                        .ForeignKey("StoreId");
                });
        }
    }
}
