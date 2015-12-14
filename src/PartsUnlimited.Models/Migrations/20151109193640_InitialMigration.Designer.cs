using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using PartsUnlimited.Models;

namespace PartsUnlimited.Models.Migrations
{
    [DbContext(typeof(PartsUnlimitedContext))]
    [Migration("20151109193640_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta8-15964")
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .Annotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .Annotation("MaxLength", 256);

                    b.HasKey("Id");

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

                    b.HasKey("Id");

                    b.Annotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.Annotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.Annotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.Annotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("PartsUnlimited.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

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

                    b.HasKey("Id");

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
                        .IsRequired();

                    b.Property<int>("Count");

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("ProductId");

                    b.HasKey("CartItemId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("CategoryId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired()
                        .Annotation("MaxLength", 70);

                    b.Property<string>("City")
                        .IsRequired()
                        .Annotation("MaxLength", 40);

                    b.Property<string>("Country")
                        .IsRequired()
                        .Annotation("MaxLength", 40);

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired()
                        .Annotation("MaxLength", 160);

                    b.Property<DateTime>("OrderDate");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .Annotation("MaxLength", 24);

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .Annotation("MaxLength", 10);

                    b.Property<bool>("Processed");

                    b.Property<string>("State")
                        .IsRequired()
                        .Annotation("MaxLength", 40);

                    b.Property<decimal>("Total");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("OrderId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OrderId");

                    b.Property<int>("ProductId");

                    b.Property<int>("Quantity");

                    b.Property<decimal>("UnitPrice");

                    b.HasKey("OrderDetailId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int>("Inventory");

                    b.Property<int>("LeadTime");

                    b.Property<decimal>("Price");

                    b.Property<string>("ProductArtUrl")
                        .IsRequired()
                        .Annotation("MaxLength", 1024);

                    b.Property<string>("ProductDetails")
                        .IsRequired();

                    b.Property<int>("RecommendationId");

                    b.Property<decimal>("SalePrice");

                    b.Property<string>("SkuNumber")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired()
                        .Annotation("MaxLength", 160);

                    b.HasKey("ProductId");
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

                    b.HasKey("RaincheckId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Store", b =>
                {
                    b.Property<int>("StoreId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("StoreId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .ForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("PartsUnlimited.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("PartsUnlimited.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .ForeignKey("RoleId");

                    b.HasOne("PartsUnlimited.Models.ApplicationUser")
                        .WithMany()
                        .ForeignKey("UserId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.CartItem", b =>
                {
                    b.HasOne("PartsUnlimited.Models.Product")
                        .WithMany()
                        .ForeignKey("ProductId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.OrderDetail", b =>
                {
                    b.HasOne("PartsUnlimited.Models.Order")
                        .WithMany()
                        .ForeignKey("OrderId");

                    b.HasOne("PartsUnlimited.Models.Product")
                        .WithMany()
                        .ForeignKey("ProductId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Product", b =>
                {
                    b.HasOne("PartsUnlimited.Models.Category")
                        .WithMany()
                        .ForeignKey("CategoryId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Order", b =>
            {
                b.HasOne("PartsUnlimited.Models.Promo")
                    .WithMany()
                    .ForeignKey("PromoId");
            });

            modelBuilder.Entity("PartsUnlimited.Models.Raincheck", b =>
                {
                    b.HasOne("PartsUnlimited.Models.Product")
                        .WithMany()
                        .ForeignKey("ProductId");

                    b.HasOne("PartsUnlimited.Models.Store")
                        .WithMany()
                        .ForeignKey("StoreId");
                });
        }
    }
}
