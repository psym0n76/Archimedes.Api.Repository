using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class newpricelevelfirelds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OutsideOfRange",
                table: "PriceLevels",
                newName: "OutsideRange");

            migrationBuilder.RenameColumn(
                name: "LastLevelBrokenDate",
                table: "PriceLevels",
                newName: "OutsideRangeDate");

            migrationBuilder.RenameColumn(
                name: "BookedTrades",
                table: "PriceLevels",
                newName: "Trades");

            migrationBuilder.AddColumn<DateTime>(
                name: "PriceLevelTimestamp",
                table: "Trades",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CandlesElapsedLevelBroken",
                table: "PriceLevels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LevelBrokenDate",
                table: "PriceLevels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceLevelTimestamp",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "CandlesElapsedLevelBroken",
                table: "PriceLevels");

            migrationBuilder.DropColumn(
                name: "LevelBrokenDate",
                table: "PriceLevels");

            migrationBuilder.RenameColumn(
                name: "Trades",
                table: "PriceLevels",
                newName: "BookedTrades");

            migrationBuilder.RenameColumn(
                name: "OutsideRangeDate",
                table: "PriceLevels",
                newName: "LastLevelBrokenDate");

            migrationBuilder.RenameColumn(
                name: "OutsideRange",
                table: "PriceLevels",
                newName: "OutsideOfRange");
        }
    }
}
