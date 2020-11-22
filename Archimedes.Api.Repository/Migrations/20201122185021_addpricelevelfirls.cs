using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class addpricelevelfirls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookedTrades",
                table: "PriceLevels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLevelBrokenDate",
                table: "PriceLevels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LevelsBroken",
                table: "PriceLevels",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookedTrades",
                table: "PriceLevels");

            migrationBuilder.DropColumn(
                name: "LastLevelBrokenDate",
                table: "PriceLevels");

            migrationBuilder.DropColumn(
                name: "LevelsBroken",
                table: "PriceLevels");
        }
    }
}
