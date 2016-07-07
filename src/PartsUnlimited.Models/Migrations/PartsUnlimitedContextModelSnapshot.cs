using System;
using Microsoft.EntityFrameworkCore;
using PartsUnlimited.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PartsUnlimited.Models.Migrations
{
    [DbContext(typeof(PartsUnlimitedContext))]
    partial class PartsUnlimitedContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("PartsUnlimited.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
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
                        .HasAnnotation("MaxLength", 70);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 40);

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 40);

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 160);

                    b.Property<DateTime>("OrderDate");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 24);

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 10);

                    b.Property<bool>("Processed");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 40);

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
                        .HasAnnotation("MaxLength", 1024);

                    b.Property<string>("ProductDetails")
                        .IsRequired();

                    b.Property<int>("RecommendationId");

                    b.Property<decimal>("SalePrice");

                    b.Property<string>("SkuNumber")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 160);

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
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("PartsUnlimited.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("PartsUnlimited.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("PartsUnlimited.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.CartItem", b =>
                {
                    b.HasOne("PartsUnlimited.Models.Product")
                        .WithMany()
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.OrderDetail", b =>
                {
                    b.HasOne("PartsUnlimited.Models.Order")
                        .WithMany()
                        .HasForeignKey("OrderId");

                    b.HasOne("PartsUnlimited.Models.Product")
                        .WithMany()
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Product", b =>
                {
                    b.HasOne("PartsUnlimited.Models.Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("PartsUnlimited.Models.Raincheck", b =>
                {
                    b.HasOne("PartsUnlimited.Models.Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("PartsUnlimited.Models.Store")
                        .WithMany()
                        .HasForeignKey("StoreId");
                });
        }
    }
}
