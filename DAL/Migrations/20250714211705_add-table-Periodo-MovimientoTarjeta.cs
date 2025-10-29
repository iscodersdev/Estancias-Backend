using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addtablePeriodoMovimientoTarjeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovimientoTarjetaCuotas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NroCuota = table.Column<int>(nullable: false),
                    Fecha = table.Column<string>(nullable: true),
                    Monto = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoTarjetaCuotas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Periodo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    FechaDesde = table.Column<DateTime>(nullable: false),
                    FechaHasta = table.Column<DateTime>(nullable: false),
                    FechaVencimiento = table.Column<DateTime>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periodo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovimientoTarjeta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NroSolicitud = table.Column<int>(nullable: false),
                    NombreComercio = table.Column<string>(nullable: true),
                    CantidadCuotas = table.Column<int>(nullable: false),
                    MontoTotal = table.Column<decimal>(nullable: false),
                    TipoMovimiento = table.Column<string>(nullable: true),
                    MovimientoTarjetaCuotasId = table.Column<int>(nullable: true),
                    PeriodoId = table.Column<int>(nullable: true),
                    UsuarioId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoTarjeta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientoTarjeta_MovimientoTarjetaCuotas_MovimientoTarjetaCuotasId",
                        column: x => x.MovimientoTarjetaCuotasId,
                        principalTable: "MovimientoTarjetaCuotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientoTarjeta_Periodo_PeriodoId",
                        column: x => x.PeriodoId,
                        principalTable: "Periodo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientoTarjeta_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoTarjeta_MovimientoTarjetaCuotasId",
                table: "MovimientoTarjeta",
                column: "MovimientoTarjetaCuotasId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoTarjeta_PeriodoId",
                table: "MovimientoTarjeta",
                column: "PeriodoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoTarjeta_UsuarioId",
                table: "MovimientoTarjeta",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientoTarjeta");

            migrationBuilder.DropTable(
                name: "MovimientoTarjetaCuotas");

            migrationBuilder.DropTable(
                name: "Periodo");
        }
    }
}
