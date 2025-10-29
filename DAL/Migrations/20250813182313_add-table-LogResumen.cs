using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addtableLogResumen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResumenTarjeta_Periodo_PeriodoId",
                table: "ResumenTarjeta");

            migrationBuilder.RenameColumn(
                name: "RegistrosNuevos",
                table: "LogProcedimientos",
                newName: "RegistrosCreados");

            migrationBuilder.RenameColumn(
                name: "RegistrosActualizados",
                table: "LogProcedimientos",
                newName: "RegistrosConErrores");

            migrationBuilder.AlterColumn<int>(
                name: "PeriodoId",
                table: "ResumenTarjeta",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiaEjecucion",
                table: "Procedimientos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LogResumenesTarjetas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<string>(nullable: true),
                    Mensaje = table.Column<string>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    LogProcedimientosId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogResumenesTarjetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogResumenesTarjetas_LogProcedimientos_LogProcedimientosId",
                        column: x => x.LogProcedimientosId,
                        principalTable: "LogProcedimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogResumenesTarjetas_LogProcedimientosId",
                table: "LogResumenesTarjetas",
                column: "LogProcedimientosId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResumenTarjeta_Periodo_PeriodoId",
                table: "ResumenTarjeta",
                column: "PeriodoId",
                principalTable: "Periodo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResumenTarjeta_Periodo_PeriodoId",
                table: "ResumenTarjeta");

            migrationBuilder.DropTable(
                name: "LogResumenesTarjetas");

            migrationBuilder.DropColumn(
                name: "DiaEjecucion",
                table: "Procedimientos");

            migrationBuilder.RenameColumn(
                name: "RegistrosCreados",
                table: "LogProcedimientos",
                newName: "RegistrosNuevos");

            migrationBuilder.RenameColumn(
                name: "RegistrosConErrores",
                table: "LogProcedimientos",
                newName: "RegistrosActualizados");

            migrationBuilder.AlterColumn<int>(
                name: "PeriodoId",
                table: "ResumenTarjeta",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ResumenTarjeta_Periodo_PeriodoId",
                table: "ResumenTarjeta",
                column: "PeriodoId",
                principalTable: "Periodo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
