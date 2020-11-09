using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class addtradesdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direction",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Granularity",
                table: "Trades");

            migrationBuilder.AddColumn<string>(
                name: "BuySell",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "Trades",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuySell",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "Trades");

            migrationBuilder.AddColumn<string>(
                name: "Direction",
                table: "Trades",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Granularity",
                table: "Trades",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
