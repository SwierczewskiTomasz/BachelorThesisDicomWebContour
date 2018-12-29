using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contours",
                columns: table => new
                {
                    ContourEntityId = table.Column<Guid>(nullable: false),
                    DicomId = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true),
                    IsManual = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contours", x => x.ContourEntityId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contours");
        }
    }
}
