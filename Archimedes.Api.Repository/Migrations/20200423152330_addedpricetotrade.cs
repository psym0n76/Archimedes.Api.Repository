using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class addedpricetotrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ClosePrice",
                table: "Trades",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenPrice",
                table: "Trades",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosePrice",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "OpenPrice",
                table: "Trades");
        }
    }
}
