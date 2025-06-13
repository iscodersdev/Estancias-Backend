using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FechaVencimiento",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NroTarjeta",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TokenEnviado",
                table: "Personas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidandoTarjeta",
                table: "Personas",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "NroTarjeta",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "TokenEnviado",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "ValidandoTarjeta",
                table: "Personas");
        }
    }
}
