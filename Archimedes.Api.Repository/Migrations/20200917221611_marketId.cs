using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class marketId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MarketId",
                table: "Prices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarketId",
                table: "Candles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "Candles");
        }
    }
}
