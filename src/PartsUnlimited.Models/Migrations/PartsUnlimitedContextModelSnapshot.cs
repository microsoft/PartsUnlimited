// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using PartsUnlimited.Models;
using Microsoft.Data.Entity.SqlServer.Metadata;

namespace PartsUnlimited.Models.Migrations
{
    [DbContext(typeof(PartsUnlimitedContext))]
    partial class PartsUnlimitedContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta7-15540")
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();

                    b.Property<string>("Name")
                        .Annotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .Annotation("MaxLength", 256);

                    b.Key("Id");

                    b.Index("NormalizedName")
                        .Annotation("Relational:Name", "RoleNameIndex");

                    b.Annotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId");

                    b.Key("Id");

                    b.Annotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId");

                    b.Key("Id");

                    b.Annotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId");

                    b.Key("LoginProvider", "ProviderKey");

                    b.Annotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.Key("UserId", "RoleId");

                    b.Annotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("PartsUnlimited.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();

                    b.Property<string>("Email")
                        .Annotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .Annotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .Annotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .Annotation("MaxLength", 256);

                    b.Key("Id");

                    b.Index("NormalizedEmail")
                        .Annotation("Relational:Name", "EmailIndex");

                    b.Index("NormalizedUserName")
                        .Annotation("Relational:Name", "UserNameIndex");

                    b.Annotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("PartsUnlimited.Models.CartItem", b =>
                {
                    b.Property<int>("CartItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CartId")
                        .Required();

                    b.Property<int>("Count");

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("ProductId");

                    b.Key("CartItemId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Name")
                        .Required();

                    b.Key("CategoryId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .Required()
                        .Annotation("MaxLength", 70);

                    b.Property<string>("City")
                        .Required()
                        .Annotation("MaxLength", 40);

                    b.Property<string>("Country")
                        .Required()
                        .Annotation("MaxLength", 40);

                    b.Property<string>("Email")
                        .Required();

                    b.Property<string>("Name")
                        .Required()
                        .Annotation("MaxLength", 160);

                    b.Property<DateTime>("OrderDate");

                    b.Property<string>("Phone")
                        .Required()
                        .Annotation("MaxLength", 24);

                    b.Property<string>("PostalCode")
                        .Required()
                        .Annotation("MaxLength", 10);

                    b.Property<bool>("Processed");

                    b.Property<string>("State")
                        .Required()
                        .Annotation("MaxLength", 40);

                    b.Property<decimal>("Total");

                    b.Property<string>("Username")
                        .Required();

                    b.Key("OrderId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OrderId");

                    b.Property<int>("ProductId");

                    b.Property<int>("Quantity");

                    b.Property<decimal>("UnitPrice");

                    b.Key("OrderDetailId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .Required();

                    b.Property<int>("Inventory");

                    b.Property<int>("LeadTime");

                    b.Property<decimal>("Price");

                    b.Property<string>("ProductArtUrl")
                        .Required()
                        .Annotation("MaxLength", 1024);

                    b.Property<string>("ProductDetails")
                        .Required();

                    b.Property<int>("RecommendationId");

                    b.Property<decimal>("SalePrice");

                    b.Property<string>("SkuNumber")
                        .Required();

                    b.Property<string>("Title")
                        .Required()
                        .Annotation("MaxLength", 160);

                    b.Key("ProductId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Raincheck", b =>
                {
                    b.Property<int>("RaincheckId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("ProductId");

                    b.Property<int>("Quantity");

                    b.Property<double>("SalePrice");

                    b.Property<int>("StoreId");

                    b.Key("RaincheckId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Store", b =>
                {
                    b.Property<int>("StoreId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Key("StoreId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Reference("PartsUnlimited.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Reference("PartsUnlimited.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");

                    b.Reference("PartsUnlimited.Models.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.CartItem", b =>
                {
                    b.Reference("PartsUnlimited.Models.Product")
                        .InverseCollection()
                        .ForeignKey("ProductId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.OrderDetail", b =>
                {
                    b.Reference("PartsUnlimited.Models.Order")
                        .InverseCollection()
                        .ForeignKey("OrderId");

                    b.Reference("PartsUnlimited.Models.Product")
                        .InverseCollection()
                        .ForeignKey("ProductId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Product", b =>
                {
                    b.Reference("PartsUnlimited.Models.Category")
                        .InverseCollection()
                        .ForeignKey("CategoryId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Raincheck", b =>
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
