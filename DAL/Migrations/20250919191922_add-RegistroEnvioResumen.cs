using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addRegistroEnvioResumen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DistribucionResumen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<string>(nullable: true),
                    PeriodoId = table.Column<int>(nullable: true),
                    ResumenTarjetaId = table.Column<int>(nullable: true),
                    CanalesDistribucion = table.Column<string>(nullable: true),
                    Estado = table.Column<string>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistribucionResumen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistribucionResumen_Periodo_PeriodoId",
                        column: x => x.PeriodoId,
                        principalTable: "Periodo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DistribucionResumen_ResumenTarjeta_ResumenTarjetaId",
                        column: x => x.ResumenTarjetaId,
                        principalTable: "ResumenTarjeta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DistribucionResumen_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionResumen_PeriodoId",
                table: "DistribucionResumen",
                column: "PeriodoId");

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionResumen_ResumenTarjetaId",
                table: "DistribucionResumen",
                column: "ResumenTarjetaId");

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionResumen_UsuarioId",
                table: "DistribucionResumen",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistribucionResumen");
        }
    }
}
