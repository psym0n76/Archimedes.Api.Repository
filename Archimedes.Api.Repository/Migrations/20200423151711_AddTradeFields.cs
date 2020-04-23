using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class AddTradeFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Granularity",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Strategy",
                table: "Trades",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Granularity",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Strategy",
                table: "Trades");
        }
    }
}
