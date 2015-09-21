// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.SqlServer.Metadata;

namespace PartsUnlimited.Models.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(isNullable: false),
                    ConcurrencyStamp = table.Column<string>(isNullable: true),
                    Name = table.Column<string>(isNullable: true),
                    NormalizedName = table.Column<string>(isNullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(isNullable: false),
                    AccessFailedCount = table.Column<int>(isNullable: false),
                    ConcurrencyStamp = table.Column<string>(isNullable: true),
                    Email = table.Column<string>(isNullable: true),
                    EmailConfirmed = table.Column<bool>(isNullable: false),
                    LockoutEnabled = table.Column<bool>(isNullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(isNullable: true),
                    Name = table.Column<string>(isNullable: true),
                    NormalizedEmail = table.Column<string>(isNullable: true),
                    NormalizedUserName = table.Column<string>(isNullable: true),
                    PasswordHash = table.Column<string>(isNullable: true),
                    PhoneNumber = table.Column<string>(isNullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(isNullable: false),
                    SecurityStamp = table.Column<string>(isNullable: true),
                    TwoFactorEnabled = table.Column<bool>(isNullable: false),
                    UserName = table.Column<string>(isNullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    Description = table.Column<string>(isNullable: true),
                    ImageUrl = table.Column<string>(isNullable: true),
                    Name = table.Column<string>(isNullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    Address = table.Column<string>(isNullable: false),
                    City = table.Column<string>(isNullable: false),
                    Country = table.Column<string>(isNullable: false),
                    Email = table.Column<string>(isNullable: false),
                    Name = table.Column<string>(isNullable: false),
                    OrderDate = table.Column<DateTime>(isNullable: false),
                    Phone = table.Column<string>(isNullable: false),
                    PostalCode = table.Column<string>(isNullable: false),
                    Processed = table.Column<bool>(isNullable: false),
                    State = table.Column<string>(isNullable: false),
                    Total = table.Column<decimal>(isNullable: false),
                    Username = table.Column<string>(isNullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                });
            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    StoreId = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    Name = table.Column<string>(isNullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.StoreId);
                });
            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(isNullable: true),
                    ClaimValue = table.Column<string>(isNullable: true),
                    RoleId = table.Column<string>(isNullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(isNullable: true),
                    ClaimValue = table.Column<string>(isNullable: true),
                    UserId = table.Column<string>(isNullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(isNullable: false),
                    ProviderKey = table.Column<string>(isNullable: false),
                    ProviderDisplayName = table.Column<string>(isNullable: true),
                    UserId = table.Column<string>(isNullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<string>", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(isNullable: false),
                    RoleId = table.Column<string>(isNullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole<string>", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(isNullable: false),
                    Created = table.Column<DateTime>(isNullable: false),
                    Description = table.Column<string>(isNullable: false),
                    Inventory = table.Column<int>(isNullable: false),
                    LeadTime = table.Column<int>(isNullable: false),
                    Price = table.Column<decimal>(isNullable: false),
                    ProductArtUrl = table.Column<string>(isNullable: false),
                    ProductDetails = table.Column<string>(isNullable: false),
                    RecommendationId = table.Column<int>(isNullable: false),
                    SalePrice = table.Column<decimal>(isNullable: false),
                    SkuNumber = table.Column<string>(isNullable: false),
                    Title = table.Column<string>(isNullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId");
                });
            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    CartItemId = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    CartId = table.Column<string>(isNullable: false),
                    Count = table.Column<int>(isNullable: false),
                    DateCreated = table.Column<DateTime>(isNullable: false),
                    ProductId = table.Column<int>(isNullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId");
                });
            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    OrderId = table.Column<int>(isNullable: false),
                    ProductId = table.Column<int>(isNullable: false),
                    Quantity = table.Column<int>(isNullable: false),
                    UnitPrice = table.Column<decimal>(isNullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_OrderDetail_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId");
                });
            migrationBuilder.CreateTable(
                name: "Raincheck",
                columns: table => new
                {
                    RaincheckId = table.Column<int>(isNullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerIdentityStrategy.IdentityColumn),
                    Name = table.Column<string>(isNullable: true),
                    ProductId = table.Column<int>(isNullable: false),
                    Quantity = table.Column<int>(isNullable: false),
                    SalePrice = table.Column<double>(isNullable: false),
                    StoreId = table.Column<int>(isNullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raincheck", x => x.RaincheckId);
                    table.ForeignKey(
                        name: "FK_Raincheck_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_Raincheck_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "StoreId");
                });
            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");
            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("AspNetRoleClaims");
            migrationBuilder.DropTable("AspNetUserClaims");
            migrationBuilder.DropTable("AspNetUserLogins");
            migrationBuilder.DropTable("AspNetUserRoles");
            migrationBuilder.DropTable("CartItem");
            migrationBuilder.DropTable("OrderDetail");
            migrationBuilder.DropTable("Raincheck");
            migrationBuilder.DropTable("AspNetRoles");
            migrationBuilder.DropTable("AspNetUsers");
            migrationBuilder.DropTable("Order");
            migrationBuilder.DropTable("Product");
            migrationBuilder.DropTable("Store");
            migrationBuilder.DropTable("Category");
        }
    }
}
