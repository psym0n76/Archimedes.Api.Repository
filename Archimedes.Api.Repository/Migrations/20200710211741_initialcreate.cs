using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    Market = table.Column<string>(nullable: true),
                    Granularity = table.Column<string>(nullable: true),
                    BidOpen = table.Column<double>(nullable: false),
                    BidClose = table.Column<double>(nullable: false),
                    BidHigh = table.Column<double>(nullable: false),
                    BidLow = table.Column<double>(nullable: false),
                    AskOpen = table.Column<double>(nullable: false),
                    AskClose = table.Column<double>(nullable: false),
                    AskHigh = table.Column<double>(nullable: false),
                    AskLow = table.Column<double>(nullable: false),
                    TickQty = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Interval = table.Column<int>(nullable: false),
                    TimeFrame = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    MaxDate = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Market = table.Column<string>(nullable: true),
                    Granularity = table.Column<string>(nullable: true),
                    BidOpen = table.Column<double>(nullable: false),
                    BidClose = table.Column<double>(nullable: false),
                    BidHigh = table.Column<double>(nullable: false),
                    BidLow = table.Column<double>(nullable: false),
                    AskOpen = table.Column<double>(nullable: false),
                    AskClose = table.Column<double>(nullable: false),
                    AskHigh = table.Column<double>(nullable: false),
                    AskLow = table.Column<double>(nullable: false),
                    TickQty = table.Column<double>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Direction = table.Column<string>(nullable: true),
                    Granularity = table.Column<string>(nullable: true),
                    Market = table.Column<string>(nullable: true),
                    Strategy = table.Column<string>(nullable: true),
                    OpenPrice = table.Column<decimal>(type: "decimal(5, 5)", nullable: false),
                    ClosePrice = table.Column<decimal>(type: "decimal(5, 5)", nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
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
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Trades");
        }
    }
}
