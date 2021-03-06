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
    [Migration("20210110125927_addexternalmarketID")]
    partial class addexternalmarketID
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Archimedes.Api.Repository.Candle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<decimal>("AskClose")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("AskHigh")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("AskLow")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("AskOpen")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("BidClose")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("BidHigh")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("BidLow")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("BidOpen")
                        .HasColumnType("decimal(18,5)");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Granularity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MarketId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TickQty")
                        .HasColumnType("float");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Candles");
                });

            modelBuilder.Entity("Archimedes.Api.Repository.Market", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int>("ExternalMarketId")
                        .HasColumnType("int");

                    b.Property<string>("Granularity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Interval")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("MaxDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("MinDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("TimeFrame")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Markets");
                });

            modelBuilder.Entity("Archimedes.Api.Repository.Price", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<decimal>("Ask")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("Bid")
                        .HasColumnType("decimal(18,5)");

                    b.Property<string>("Granularity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("Archimedes.Api.Repository.PriceLevel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<decimal>("AskPrice")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("AskPriceRange")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("BidPrice")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("BidPriceRange")
                        .HasColumnType("decimal(18,5)");

                    b.Property<string>("BuySell")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CandleType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CandlesElapsedLevelBroken")
                        .HasColumnType("int");

                    b.Property<string>("Granularity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("LevelBroken")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LevelBrokenDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("LevelExpired")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LevelExpiredDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("LevelsBroken")
                        .HasColumnType("int");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("OutsideRange")
                        .HasColumnType("bit");

                    b.Property<DateTime>("OutsideRangeDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Strategy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Trade")
                        .HasColumnType("bit");

                    b.Property<int>("Trades")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PriceLevels");
                });

            modelBuilder.Entity("Archimedes.Api.Repository.Strategy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<double>("Count")
                        .HasColumnType("float");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Granularity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Strategy");
                });

            modelBuilder.Entity("Archimedes.Api.Repository.Trade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("BuySell")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ClosePrice")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("EntryPrice")
                        .HasColumnType("decimal(18,5)");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,5)");

                    b.Property<DateTime>("PriceLevelTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<double>("RiskReward")
                        .HasColumnType("float");

                    b.Property<string>("Strategy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Success")
                        .HasColumnType("bit");

                    b.Property<decimal>("TargetPrice")
                        .HasColumnType("decimal(18,5)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("TradeGroupId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Trades");
                });
#pragma warning restore 612, 618
        }
    }
}
