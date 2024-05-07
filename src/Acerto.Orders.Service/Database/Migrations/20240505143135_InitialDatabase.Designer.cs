﻿// <auto-generated />
using System;
using Acerto.Orders.Service.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Acerto.Orders.Service.Database.Migrations
{
    [DbContext(typeof(OrdersDbContext))]
    [Migration("20240505143135_InitialDatabase")]
    partial class InitialDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Acerto.Orders.Service.Database.Models.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_orders");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("Acerto.Orders.Service.Database.Models.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Item")
                        .HasColumnType("jsonb")
                        .HasColumnName("item");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.HasKey("Id")
                        .HasName("pk_order_items");

                    b.HasIndex("OrderId", "ProductId")
                        .IsUnique()
                        .HasDatabaseName("ix_order_items_order_id_product_id");

                    b.ToTable("order_items", null, t =>
                        {
                            t.HasCheckConstraint("order_items_quantity_greather_than_or_equal_1", "quantity >= 1");
                        });
                });

            modelBuilder.Entity("Acerto.Orders.Service.Database.Models.OrderStatusChange", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<DateTime>("StatusDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("status_date");

                    b.Property<string>("StatusReason")
                        .HasColumnType("text")
                        .HasColumnName("status_reason");

                    b.HasKey("Id")
                        .HasName("pk_order_status_changes");

                    b.HasIndex("OrderId")
                        .HasDatabaseName("ix_order_status_changes_order_id");

                    b.ToTable("order_status_changes", (string)null);
                });

            modelBuilder.Entity("Acerto.Orders.Service.Database.Models.OrderItem", b =>
                {
                    b.HasOne("Acerto.Orders.Service.Database.Models.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_items_order_order_id");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Acerto.Orders.Service.Database.Models.OrderStatusChange", b =>
                {
                    b.HasOne("Acerto.Orders.Service.Database.Models.Order", "Order")
                        .WithMany("OrderStatusChanges")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_status_changes_orders_order_id");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Acerto.Orders.Service.Database.Models.Order", b =>
                {
                    b.Navigation("OrderItems");

                    b.Navigation("OrderStatusChanges");
                });
#pragma warning restore 612, 618
        }
    }
}
