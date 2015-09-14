// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace PartsUnlimited.Models.Migrations
{
    public partial class InitialMigration : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column(type: "nvarchar(max)", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });
            migration.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column(type: "nvarchar(max)", nullable: true),
                    Email = table.Column(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column(type: "bit", nullable: false),
                    LockoutEnabled = table.Column(type: "bit", nullable: false),
                    LockoutEnd = table.Column(type: "datetimeoffset", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column(type: "bit", nullable: false),
                    SecurityStamp = table.Column(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column(type: "bit", nullable: false),
                    UserName = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });
            migration.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Description = table.Column(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column(type: "nvarchar(max)", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });
            migration.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Address = table.Column(type: "nvarchar(max)", nullable: true),
                    City = table.Column(type: "nvarchar(max)", nullable: true),
                    Country = table.Column(type: "nvarchar(max)", nullable: true),
                    Email = table.Column(type: "nvarchar(max)", nullable: true),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    OrderDate = table.Column(type: "datetime2", nullable: false),
                    Phone = table.Column(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column(type: "nvarchar(max)", nullable: true),
                    Processed = table.Column(type: "bit", nullable: false),
                    State = table.Column(type: "nvarchar(max)", nullable: true),
                    Total = table.Column(type: "decimal(18, 2)", nullable: false),
                    Username = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                });
            migration.CreateTable(
                name: "Store",
                columns: table => new
                {
                    StoreId = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Name = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.StoreId);
                });
            migration.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    ClaimType = table.Column(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                        columns: x => x.RoleId,
                        referencedTable: "AspNetRoles",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    ClaimType = table.Column(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                        columns: x => x.UserId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<string>", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                        columns: x => x.UserId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole<string>", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                        columns: x => x.RoleId,
                        referencedTable: "AspNetRoles",
                        referencedColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                        columns: x => x.UserId,
                        referencedTable: "AspNetUsers",
                        referencedColumn: "Id");
                });
            migration.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductId = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    CategoryId = table.Column(type: "int", nullable: false),
                    Created = table.Column(type: "datetime2", nullable: false),
                    Description = table.Column(type: "nvarchar(max)", nullable: true),
                    Inventory = table.Column(type: "int", nullable: false),
                    LeadTime = table.Column(type: "int", nullable: false),
                    Price = table.Column(type: "decimal(18, 2)", nullable: false),
                    ProductArtUrl = table.Column(type: "nvarchar(max)", nullable: true),
                    ProductDetails = table.Column(type: "nvarchar(max)", nullable: true),
                    RecommendationId = table.Column(type: "int", nullable: false),
                    SalePrice = table.Column(type: "decimal(18, 2)", nullable: false),
                    SkuNumber = table.Column(type: "nvarchar(max)", nullable: true),
                    Title = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_Category_CategoryId",
                        columns: x => x.CategoryId,
                        referencedTable: "Category",
                        referencedColumn: "CategoryId");
                });
            migration.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    CartItemId = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    CartId = table.Column(type: "nvarchar(max)", nullable: true),
                    Count = table.Column(type: "int", nullable: false),
                    DateCreated = table.Column(type: "datetime2", nullable: false),
                    ProductId = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItem_Product_ProductId",
                        columns: x => x.ProductId,
                        referencedTable: "Product",
                        referencedColumn: "ProductId");
                });
            migration.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailId = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    OrderId = table.Column(type: "int", nullable: false),
                    ProductId = table.Column(type: "int", nullable: false),
                    Quantity = table.Column(type: "int", nullable: false),
                    UnitPrice = table.Column(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_OrderId",
                        columns: x => x.OrderId,
                        referencedTable: "Order",
                        referencedColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_OrderDetail_Product_ProductId",
                        columns: x => x.ProductId,
                        referencedTable: "Product",
                        referencedColumn: "ProductId");
                });
            migration.CreateTable(
                name: "Raincheck",
                columns: table => new
                {
                    RaincheckId = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Name = table.Column(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column(type: "int", nullable: false),
                    Quantity = table.Column(type: "int", nullable: false),
                    SalePrice = table.Column(type: "float", nullable: false),
                    StoreId = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raincheck", x => x.RaincheckId);
                    table.ForeignKey(
                        name: "FK_Raincheck_Product_ProductId",
                        columns: x => x.ProductId,
                        referencedTable: "Product",
                        referencedColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_Raincheck_Store_StoreId",
                        columns: x => x.StoreId,
                        referencedTable: "Store",
                        referencedColumn: "StoreId");
                });
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("AspNetRoles");
            migration.DropTable("AspNetRoleClaims");
            migration.DropTable("AspNetUserClaims");
            migration.DropTable("AspNetUserLogins");
            migration.DropTable("AspNetUserRoles");
            migration.DropTable("AspNetUsers");
            migration.DropTable("CartItem");
            migration.DropTable("Category");
            migration.DropTable("Order");
            migration.DropTable("OrderDetail");
            migration.DropTable("Product");
            migration.DropTable("Raincheck");
            migration.DropTable("Store");
        }
    }
}
