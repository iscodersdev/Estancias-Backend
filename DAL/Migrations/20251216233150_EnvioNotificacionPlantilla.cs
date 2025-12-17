using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class EnvioNotificacionPlantilla : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificacionesPlantillasId",
                table: "EnvioNotificaciones",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnvioNotificaciones_NotificacionesPlantillasId",
                table: "EnvioNotificaciones",
                column: "NotificacionesPlantillasId");

            migrationBuilder.AddForeignKey(
                name: "FK_EnvioNotificaciones_NotificacionesPlantillas_NotificacionesPlantillasId",
                table: "EnvioNotificaciones",
                column: "NotificacionesPlantillasId",
                principalTable: "NotificacionesPlantillas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnvioNotificaciones_NotificacionesPlantillas_NotificacionesPlantillasId",
                table: "EnvioNotificaciones");

            migrationBuilder.DropIndex(
                name: "IX_EnvioNotificaciones_NotificacionesPlantillasId",
                table: "EnvioNotificaciones");

            migrationBuilder.DropColumn(
                name: "NotificacionesPlantillasId",
                table: "EnvioNotificaciones");
        }
    }
}
