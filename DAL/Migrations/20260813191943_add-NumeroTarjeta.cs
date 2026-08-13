using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addNumeroTarjeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NroTarjeta",
                table: "SolicitudDeTarjeta",
                newName: "NumeroTarjeta");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumeroTarjeta",
                table: "SolicitudDeTarjeta",
                newName: "NroTarjeta");
        }
    }
}
