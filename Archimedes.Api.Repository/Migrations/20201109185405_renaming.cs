using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class renaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpenPrice",
                table: "Trades",
                newName: "TargetPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "EntryPrice",
                table: "Trades",
                type: "decimal(5, 5)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryPrice",
                table: "Trades");

            migrationBuilder.RenameColumn(
                name: "TargetPrice",
                table: "Trades",
                newName: "OpenPrice");
        }
    }
}
