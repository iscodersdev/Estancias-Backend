using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addcolumFechaDePago : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDePago",
                table: "PagoTarjeta",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoInformado",
                table: "PagoTarjeta",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaDePago",
                table: "PagoTarjeta");

            migrationBuilder.DropColumn(
                name: "MontoInformado",
                table: "PagoTarjeta");
        }
    }
}
