using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class NotificacionAutomaticas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "NotificacionesPlantillas",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificacionAutomatica",
                table: "NotificacionesPlantillas",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "NotificacionesPlantillas");

            migrationBuilder.DropColumn(
                name: "NotificacionAutomatica",
                table: "NotificacionesPlantillas");
        }
    }
}
