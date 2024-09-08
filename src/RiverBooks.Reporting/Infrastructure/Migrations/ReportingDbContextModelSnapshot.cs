﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RiverBooks.Reporting.Infrastructure.Data;

#nullable disable

namespace RiverBooks.Reporting.Infrastructure.Migrations
{
    [DbContext(typeof(ReportingDbContext))]
    partial class ReportingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Reporting")
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RiverBooks.Reporting.Domain.BookSale", b =>
                {
                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("SoldAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalSales")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<int>("UnitsSold")
                        .HasColumnType("int");

                    b.HasKey("OrderId", "BookId");

                    b.HasIndex("SoldAtUtc");

                    b.ToTable("BookSale", "Reporting");
                });

            modelBuilder.Entity("RiverBooks.SharedKernel.Events.TransactionalOutboxEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Attempts")
                        .HasColumnType("int");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTimeOffset>("OccurredUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<DateTimeOffset?>("ProcessedUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("Success")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("OccurredUtc")
                        .HasFilter("[Success] = 0 AND [Attempts] < 3");

                    b.ToTable("OutboxEvents", "Reporting");
                });
#pragma warning restore 612, 618
        }
    }
}
