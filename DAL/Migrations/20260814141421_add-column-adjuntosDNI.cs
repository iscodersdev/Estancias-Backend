using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addcolumnadjuntosDNI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "DorsoDNI",
                table: "SolicitudDeTarjeta",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FrenteDNI",
                table: "SolicitudDeTarjeta",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Selfie",
                table: "SolicitudDeTarjeta",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DorsoDNI",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropColumn(
                name: "FrenteDNI",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropColumn(
                name: "Selfie",
                table: "SolicitudDeTarjeta");
        }
    }
}
