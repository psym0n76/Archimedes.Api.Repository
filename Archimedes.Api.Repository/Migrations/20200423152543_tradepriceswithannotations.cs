using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class tradepriceswithannotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "OpenPrice",
                table: "Trades",
                type: "decimal(5, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClosePrice",
                table: "Trades",
                type: "decimal(5, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "OpenPrice",
                table: "Trades",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClosePrice",
                table: "Trades",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 5)");
        }
    }
}
