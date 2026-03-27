using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class HistorialDeCupones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiasDeVencimiento",
                table: "Premios",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimiento",
                table: "Premios",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "HistorialCanje",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CodigoCupon",
                table: "HistorialCanje",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimientoCupon",
                table: "HistorialCanje",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiasDeVencimiento",
                table: "Premios");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                table: "Premios");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "HistorialCanje");

            migrationBuilder.DropColumn(
                name: "CodigoCupon",
                table: "HistorialCanje");

            migrationBuilder.DropColumn(
                name: "FechaVencimientoCupon",
                table: "HistorialCanje");
        }
    }
}
