using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class moveddoubletodecimal4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TargetPrice",
                table: "Trades",
                type: "decimal(18, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EntryPrice",
                table: "Trades",
                type: "decimal(18, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClosePrice",
                table: "Trades",
                type: "decimal(18, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5, 5)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TargetPrice",
                table: "Trades",
                type: "decimal(5, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EntryPrice",
                table: "Trades",
                type: "decimal(5, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClosePrice",
                table: "Trades",
                type: "decimal(5, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 5)");
        }
    }
}
