using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class moveddoubletodecimal2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AskClose",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "AskHigh",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "AskLow",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "AskOpen",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "BidClose",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "BidHigh",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "BidLow",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "BidOpen",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "TickQty",
                table: "Prices");

            migrationBuilder.AddColumn<decimal>(
                name: "Ask",
                table: "Prices",
                type: "decimal(18,5)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Bid",
                table: "Prices",
                type: "decimal(18,5)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ask",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "Bid",
                table: "Prices");

            migrationBuilder.AddColumn<double>(
                name: "AskClose",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AskHigh",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AskLow",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AskOpen",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BidClose",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BidHigh",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BidLow",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BidOpen",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "MarketId",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TickQty",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
