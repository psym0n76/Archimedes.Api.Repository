using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Archimedes.Api.Repository.Migrations
{
    public partial class addmarket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Interval = table.Column<int>(nullable: false),
                    TimeFrame = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    MaxDate = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Markets");
        }
    }
}
