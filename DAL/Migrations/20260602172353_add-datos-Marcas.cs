using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class adddatosMarcas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CBU",
                table: "Marcas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomAliasbre",
                table: "Marcas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatsApp",
                table: "Marcas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CBU",
                table: "Marcas");

            migrationBuilder.DropColumn(
                name: "NomAliasbre",
                table: "Marcas");

            migrationBuilder.DropColumn(
                name: "WhatsApp",
                table: "Marcas");
        }
    }
}
