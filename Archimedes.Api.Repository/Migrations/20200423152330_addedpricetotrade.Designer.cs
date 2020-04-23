﻿// <auto-generated />
using System;
using Archimedes.Api.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Archimedes.Api.Repository.Migrations
{
    [DbContext(typeof(ArchimedesContext))]
    [Migration("20200423152330_addedpricetotrade")]
    partial class addedpricetotrade
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Archimedes.Api.Repository.Candle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("AskClose")
                        .HasColumnType("float");

                    b.Property<double>("AskHigh")
                        .HasColumnType("float");

                    b.Property<double>("AskLow")
                        .HasColumnType("float");

                    b.Property<double>("AskOpen")
                        .HasColumnType("float");

                    b.Property<double>("BidClose")
                        .HasColumnType("float");

                    b.Property<double>("BidHigh")
                        .HasColumnType("float");

                    b.Property<double>("BidLow")
                        .HasColumnType("float");

                    b.Property<double>("BidOpen")
                        .HasColumnType("float");

                    b.Property<DateTime>("DateFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateTo")
                        .HasColumnType("datetime2");

                    b.Property<string>("Granularity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TickQty")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Candles");
                });

            modelBuilder.Entity("Archimedes.Api.Repository.Price", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("AskClose")
                        .HasColumnType("float");

                    b.Property<double>("AskHigh")
                        .HasColumnType("float");

                    b.Property<double>("AskLow")
                        .HasColumnType("float");

                    b.Property<double>("AskOpen")
                        .HasColumnType("float");

                    b.Property<double>("BidClose")
                        .HasColumnType("float");

                    b.Property<double>("BidHigh")
                        .HasColumnType("float");

                    b.Property<double>("BidLow")
                        .HasColumnType("float");

                    b.Property<double>("BidOpen")
                        .HasColumnType("float");

                    b.Property<string>("Granularity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TickQty")
                        .HasColumnType("float");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("Archimedes.Api.Repository.Trade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("ClosePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Direction")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Granularity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("OpenPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Strategy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Trades");
                });
#pragma warning restore 612, 618
        }
    }
}
