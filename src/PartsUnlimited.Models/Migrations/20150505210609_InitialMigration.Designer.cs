using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using PartsUnlimited.Models;

namespace PartsUnlimited.Models.Migrations
{
    [ContextType(typeof(PartsUnlimitedContext))]
    partial class InitialMigration
    {
        public override string Id
        {
            get { return "20150505210609_InitialMigration"; }
        }
        
        public override string ProductVersion
        {
            get { return "7.0.0-beta4-12943"; }
        }
        
        public override IModel Target
        {
            get
            {
                var builder = new BasicModelBuilder()
                    .Annotation("SqlServer:ValueGeneration", "Identity");
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                    {
                        b.Property<string>("ConcurrencyStamp")
                            .ConcurrencyToken()
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("Name")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<string>("NormalizedName")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("Id");
                        b.Annotation("Relational:TableName", "AspNetRoles");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("ClaimType")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("ClaimValue")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("RoleId")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("Id");
                        b.Annotation("Relational:TableName", "AspNetRoleClaims");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("ClaimType")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("ClaimValue")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("UserId")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("Id");
                        b.Annotation("Relational:TableName", "AspNetUserClaims");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("LoginProvider")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("ProviderDisplayName")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("ProviderKey")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<string>("UserId")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("LoginProvider", "ProviderKey");
                        b.Annotation("Relational:TableName", "AspNetUserLogins");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("RoleId")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("UserId")
                            .Annotation("OriginalValueIndex", 1);
                        b.Key("UserId", "RoleId");
                        b.Annotation("Relational:TableName", "AspNetUserRoles");
                    });
                
                builder.Entity("PartsUnlimited.Models.ApplicationUser", b =>
                    {
                        b.Property<int>("AccessFailedCount")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("ConcurrencyStamp")
                            .ConcurrencyToken()
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("Email")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<bool>("EmailConfirmed")
                            .Annotation("OriginalValueIndex", 3);
                        b.Property<string>("Id")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 4);
                        b.Property<bool>("LockoutEnabled")
                            .Annotation("OriginalValueIndex", 5);
                        b.Property<DateTimeOffset?>("LockoutEnd")
                            .Annotation("OriginalValueIndex", 6);
                        b.Property<string>("Name")
                            .Annotation("OriginalValueIndex", 7);
                        b.Property<string>("NormalizedEmail")
                            .Annotation("OriginalValueIndex", 8);
                        b.Property<string>("NormalizedUserName")
                            .Annotation("OriginalValueIndex", 9);
                        b.Property<string>("PasswordHash")
                            .Annotation("OriginalValueIndex", 10);
                        b.Property<string>("PhoneNumber")
                            .Annotation("OriginalValueIndex", 11);
                        b.Property<bool>("PhoneNumberConfirmed")
                            .Annotation("OriginalValueIndex", 12);
                        b.Property<string>("SecurityStamp")
                            .Annotation("OriginalValueIndex", 13);
                        b.Property<bool>("TwoFactorEnabled")
                            .Annotation("OriginalValueIndex", 14);
                        b.Property<string>("UserName")
                            .Annotation("OriginalValueIndex", 15);
                        b.Key("Id");
                        b.Annotation("Relational:TableName", "AspNetUsers");
                    });
                
                builder.Entity("PartsUnlimited.Models.CartItem", b =>
                    {
                        b.Property<string>("CartId")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<int>("CartItemId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 1)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<int>("Count")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<DateTime>("DateCreated")
                            .Annotation("OriginalValueIndex", 3);
                        b.Property<int>("ProductId")
                            .Annotation("OriginalValueIndex", 4);
                        b.Key("CartItemId");
                    });
                
                builder.Entity("PartsUnlimited.Models.Category", b =>
                    {
                        b.Property<int>("CategoryId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 0)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("Description")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("ImageUrl")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<string>("Name")
                            .Annotation("OriginalValueIndex", 3);
                        b.Key("CategoryId");
                    });
                
                builder.Entity("PartsUnlimited.Models.Order", b =>
                    {
                        b.Property<string>("Address")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<string>("City")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("Country")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<string>("Email")
                            .Annotation("OriginalValueIndex", 3);
                        b.Property<string>("Name")
                            .Annotation("OriginalValueIndex", 4);
                        b.Property<DateTime>("OrderDate")
                            .Annotation("OriginalValueIndex", 5);
                        b.Property<int>("OrderId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 6)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<string>("Phone")
                            .Annotation("OriginalValueIndex", 7);
                        b.Property<string>("PostalCode")
                            .Annotation("OriginalValueIndex", 8);
                        b.Property<bool>("Processed")
                            .Annotation("OriginalValueIndex", 9);
                        b.Property<string>("State")
                            .Annotation("OriginalValueIndex", 10);
                        b.Property<decimal>("Total")
                            .Annotation("OriginalValueIndex", 11);
                        b.Property<string>("Username")
                            .Annotation("OriginalValueIndex", 12);
                        b.Key("OrderId");
                    });
                
                builder.Entity("PartsUnlimited.Models.OrderDetail", b =>
                    {
                        b.Property<int>("OrderDetailId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 0)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<int>("OrderId")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("ProductId")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<int>("Quantity")
                            .Annotation("OriginalValueIndex", 3);
                        b.Property<decimal>("UnitPrice")
                            .Annotation("OriginalValueIndex", 4);
                        b.Key("OrderDetailId");
                    });
                
                builder.Entity("PartsUnlimited.Models.Product", b =>
                    {
                        b.Property<int>("CategoryId")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<DateTime>("Created")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<string>("Description")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<int>("Inventory")
                            .Annotation("OriginalValueIndex", 3);
                        b.Property<int>("LeadTime")
                            .Annotation("OriginalValueIndex", 4);
                        b.Property<decimal>("Price")
                            .Annotation("OriginalValueIndex", 5);
                        b.Property<string>("ProductArtUrl")
                            .Annotation("OriginalValueIndex", 6);
                        b.Property<string>("ProductDetails")
                            .Annotation("OriginalValueIndex", 7);
                        b.Property<int>("ProductId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 8)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<int>("RecommendationId")
                            .Annotation("OriginalValueIndex", 9);
                        b.Property<decimal>("SalePrice")
                            .Annotation("OriginalValueIndex", 10);
                        b.Property<string>("SkuNumber")
                            .Annotation("OriginalValueIndex", 11);
                        b.Property<string>("Title")
                            .Annotation("OriginalValueIndex", 12);
                        b.Key("ProductId");
                    });
                
                builder.Entity("PartsUnlimited.Models.Raincheck", b =>
                    {
                        b.Property<string>("Name")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<int>("ProductId")
                            .Annotation("OriginalValueIndex", 1);
                        b.Property<int>("Quantity")
                            .Annotation("OriginalValueIndex", 2);
                        b.Property<int>("RaincheckId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 3)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Property<double>("SalePrice")
                            .Annotation("OriginalValueIndex", 4);
                        b.Property<int>("StoreId")
                            .Annotation("OriginalValueIndex", 5);
                        b.Key("RaincheckId");
                    });
                
                builder.Entity("PartsUnlimited.Models.Store", b =>
                    {
                        b.Property<string>("Name")
                            .Annotation("OriginalValueIndex", 0);
                        b.Property<int>("StoreId")
                            .GenerateValueOnAdd()
                            .Annotation("OriginalValueIndex", 1)
                            .Annotation("SqlServer:ValueGeneration", "Default");
                        b.Key("StoreId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", "RoleId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("PartsUnlimited.Models.ApplicationUser", "UserId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("PartsUnlimited.Models.ApplicationUser", "UserId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", "RoleId");
                        b.ForeignKey("PartsUnlimited.Models.ApplicationUser", "UserId");
                    });
                
                builder.Entity("PartsUnlimited.Models.CartItem", b =>
                    {
                        b.ForeignKey("PartsUnlimited.Models.Product", "ProductId");
                    });
                
                builder.Entity("PartsUnlimited.Models.OrderDetail", b =>
                    {
                        b.ForeignKey("PartsUnlimited.Models.Order", "OrderId");
                        b.ForeignKey("PartsUnlimited.Models.Product", "ProductId");
                    });
                
                builder.Entity("PartsUnlimited.Models.Product", b =>
                    {
                        b.ForeignKey("PartsUnlimited.Models.Category", "CategoryId");
                    });
                
                builder.Entity("PartsUnlimited.Models.Raincheck", b =>
                    {
                        b.ForeignKey("PartsUnlimited.Models.Product", "ProductId");
                        b.ForeignKey("PartsUnlimited.Models.Store", "StoreId");
                    });
                
                return builder.Model;
            }
        }
    }
}
