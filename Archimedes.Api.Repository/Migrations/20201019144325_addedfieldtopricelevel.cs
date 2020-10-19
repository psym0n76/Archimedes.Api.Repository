using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class addedfieldtopricelevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CandleType",
                table: "PriceLevels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandleType",
                table: "PriceLevels");
        }
    }
}
