using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class ClienteHistorialPuntos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "HistorialDePuntos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialDePuntos_ClienteId",
                table: "HistorialDePuntos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialDePuntos_Clientes_ClienteId",
                table: "HistorialDePuntos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialDePuntos_Clientes_ClienteId",
                table: "HistorialDePuntos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialDePuntos_ClienteId",
                table: "HistorialDePuntos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "HistorialDePuntos");
        }
    }
}
