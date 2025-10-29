using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addtableResumenTarjeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResumenTarjeta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NroComprobante = table.Column<string>(nullable: true),
                    Monto = table.Column<decimal>(nullable: false),
                    MontoAdeudado = table.Column<decimal>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    PeriodoId = table.Column<int>(nullable: true),
                    UsuarioId = table.Column<string>(nullable: true),
                    Adjunto = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumenTarjeta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumenTarjeta_Periodo_PeriodoId",
                        column: x => x.PeriodoId,
                        principalTable: "Periodo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResumenTarjeta_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResumenTarjeta_PeriodoId",
                table: "ResumenTarjeta",
                column: "PeriodoId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumenTarjeta_UsuarioId",
                table: "ResumenTarjeta",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResumenTarjeta");
        }
    }
}
