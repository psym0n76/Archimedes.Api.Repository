using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class lastUpdatedchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Trades",
                newName: "TimeStamp");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Prices",
                newName: "TimeStamp");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Candles",
                newName: "TimeStamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Trades",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Prices",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Candles",
                newName: "Timestamp");
        }
    }
}
