using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addTerminosCondiciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiasDeVencimiento",
                table: "Premios");

            migrationBuilder.AddColumn<string>(
                name: "TerminosCondiciones",
                table: "Premios",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TerminosCondiciones",
                table: "Premios");

            migrationBuilder.AddColumn<int>(
                name: "DiasDeVencimiento",
                table: "Premios",
                nullable: false,
                defaultValue: 0);
        }
    }
}
