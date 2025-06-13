using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subtitulo",
                table: "Novedades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextoBoton",
                table: "Novedades",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subtitulo",
                table: "Novedades");

            migrationBuilder.DropColumn(
                name: "TextoBoton",
                table: "Novedades");
        }
    }
}
