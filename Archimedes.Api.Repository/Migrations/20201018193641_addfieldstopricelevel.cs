using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class addfieldstopricelevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "PriceLevels");

            migrationBuilder.DropColumn(
                name: "PriceRange",
                table: "PriceLevels");

            migrationBuilder.AddColumn<double>(
                name: "AskPrice",
                table: "PriceLevels",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AskPriceRange",
                table: "PriceLevels",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BidPrice",
                table: "PriceLevels",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BidPriceRange",
                table: "PriceLevels",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Strategy",
                table: "PriceLevels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AskPrice",
                table: "PriceLevels");

            migrationBuilder.DropColumn(
                name: "AskPriceRange",
                table: "PriceLevels");

            migrationBuilder.DropColumn(
                name: "BidPrice",
                table: "PriceLevels");

            migrationBuilder.DropColumn(
                name: "BidPriceRange",
                table: "PriceLevels");

            migrationBuilder.DropColumn(
                name: "Strategy",
                table: "PriceLevels");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "PriceLevels",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceRange",
                table: "PriceLevels",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
