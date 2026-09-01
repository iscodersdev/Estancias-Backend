using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addcolumndomicilioSolicitarTarjeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Domicilio",
                table: "SolicitudDeTarjeta",
                newName: "PisoDepto");

            migrationBuilder.AddColumn<string>(
                name: "Altura",
                table: "SolicitudDeTarjeta",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Calle",
                table: "SolicitudDeTarjeta",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoPostal",
                table: "SolicitudDeTarjeta",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocalidadId",
                table: "SolicitudDeTarjeta",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinciaId",
                table: "SolicitudDeTarjeta",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudDeTarjeta_LocalidadId",
                table: "SolicitudDeTarjeta",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudDeTarjeta_ProvinciaId",
                table: "SolicitudDeTarjeta",
                column: "ProvinciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudDeTarjeta_Localidad_LocalidadId",
                table: "SolicitudDeTarjeta",
                column: "LocalidadId",
                principalTable: "Localidad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudDeTarjeta_Provincia_ProvinciaId",
                table: "SolicitudDeTarjeta",
                column: "ProvinciaId",
                principalTable: "Provincia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudDeTarjeta_Localidad_LocalidadId",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudDeTarjeta_Provincia_ProvinciaId",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudDeTarjeta_LocalidadId",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudDeTarjeta_ProvinciaId",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropColumn(
                name: "Altura",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropColumn(
                name: "Calle",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropColumn(
                name: "CodigoPostal",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropColumn(
                name: "LocalidadId",
                table: "SolicitudDeTarjeta");

            migrationBuilder.DropColumn(
                name: "ProvinciaId",
                table: "SolicitudDeTarjeta");

            migrationBuilder.RenameColumn(
                name: "PisoDepto",
                table: "SolicitudDeTarjeta",
                newName: "Domicilio");
        }
    }
}
