using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class UpdateTableMovimientoTarjeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientoTarjeta_MovimientoTarjetaCuotas_MovimientoTarjetaCuotasId",
                table: "MovimientoTarjeta");

            migrationBuilder.DropTable(
                name: "MovimientoTarjetaCuotas");

            migrationBuilder.DropIndex(
                name: "IX_MovimientoTarjeta_MovimientoTarjetaCuotasId",
                table: "MovimientoTarjeta");

            migrationBuilder.DropColumn(
                name: "CantidadCuotas",
                table: "MovimientoTarjeta");

            migrationBuilder.DropColumn(
                name: "MovimientoTarjetaCuotasId",
                table: "MovimientoTarjeta");

            migrationBuilder.RenameColumn(
                name: "TipoMovimiento",
                table: "MovimientoTarjeta",
                newName: "NroCuota");

            migrationBuilder.RenameColumn(
                name: "MontoTotal",
                table: "MovimientoTarjeta",
                newName: "Monto");

            migrationBuilder.AlterColumn<string>(
                name: "NroSolicitud",
                table: "MovimientoTarjeta",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "MovimientoTarjeta",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "MovimientoTarjeta");

            migrationBuilder.RenameColumn(
                name: "NroCuota",
                table: "MovimientoTarjeta",
                newName: "TipoMovimiento");

            migrationBuilder.RenameColumn(
                name: "Monto",
                table: "MovimientoTarjeta",
                newName: "MontoTotal");

            migrationBuilder.AlterColumn<int>(
                name: "NroSolicitud",
                table: "MovimientoTarjeta",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CantidadCuotas",
                table: "MovimientoTarjeta",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovimientoTarjetaCuotasId",
                table: "MovimientoTarjeta",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MovimientoTarjetaCuotas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<string>(nullable: true),
                    Monto = table.Column<decimal>(nullable: false),
                    NroCuota = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoTarjetaCuotas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoTarjeta_MovimientoTarjetaCuotasId",
                table: "MovimientoTarjeta",
                column: "MovimientoTarjetaCuotasId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientoTarjeta_MovimientoTarjetaCuotas_MovimientoTarjetaCuotasId",
                table: "MovimientoTarjeta",
                column: "MovimientoTarjetaCuotasId",
                principalTable: "MovimientoTarjetaCuotas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
