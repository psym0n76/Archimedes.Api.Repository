using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class moveddoubletodecimal5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradeType",
                table: "PriceLevels");

            migrationBuilder.AddColumn<string>(
                name: "BuySell",
                table: "PriceLevels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuySell",
                table: "PriceLevels");

            migrationBuilder.AddColumn<string>(
                name: "TradeType",
                table: "PriceLevels",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
