using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig25FechaDesdeHastaBanner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDesde",
                table: "Banners",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHasta",
                table: "Banners",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Vencimiento",
                table: "Banners",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaDesde",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "FechaHasta",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Vencimiento",
                table: "Banners");
        }
    }
}
