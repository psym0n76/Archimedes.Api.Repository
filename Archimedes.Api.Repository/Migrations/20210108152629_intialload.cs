using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class intialload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Market = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarketId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Granularity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BidOpen = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    BidClose = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    BidHigh = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    BidLow = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    AskOpen = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    AskClose = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    AskHigh = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    AskLow = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TickQty = table.Column<double>(type: "float", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interval = table.Column<int>(type: "int", nullable: false),
                    TimeFrame = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Granularity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    MinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Market = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Granularity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    BuySell = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandleType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strategy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BidPrice = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    BidPriceRange = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    AskPrice = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    AskPriceRange = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    LevelsBroken = table.Column<int>(type: "int", nullable: false),
                    LevelBroken = table.Column<bool>(type: "bit", nullable: false),
                    LevelBrokenDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LevelExpired = table.Column<bool>(type: "bit", nullable: false),
                    LevelExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CandlesElapsedLevelBroken = table.Column<int>(type: "int", nullable: false),
                    OutsideRange = table.Column<bool>(type: "bit", nullable: false),
                    OutsideRangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Trade = table.Column<bool>(type: "bit", nullable: false),
                    Trades = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Market = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Granularity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bid = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    Ask = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Strategy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Market = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Granularity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Count = table.Column<double>(type: "float", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeGroupId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Market = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strategy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceLevelTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BuySell = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetPrice = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    EntryPrice = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    ClosePrice = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    RiskReward = table.Column<double>(type: "float", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candles");

            migrationBuilder.DropTable(
                name: "Markets");

            migrationBuilder.DropTable(
                name: "PriceLevels");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Strategy");

            migrationBuilder.DropTable(
                name: "Trades");
        }
    }
}
