using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addmarcaspromocionessucursales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MarcaId",
                table: "Sucursales",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MarcaId",
                table: "Promociones",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MarcaId",
                table: "Catalogo",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_MarcaId",
                table: "Sucursales",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_MarcaId",
                table: "Promociones",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogo_MarcaId",
                table: "Catalogo",
                column: "MarcaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogo_Marcas_MarcaId",
                table: "Catalogo",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Promociones_Marcas_MarcaId",
                table: "Promociones",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sucursales_Marcas_MarcaId",
                table: "Sucursales",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalogo_Marcas_MarcaId",
                table: "Catalogo");

            migrationBuilder.DropForeignKey(
                name: "FK_Promociones_Marcas_MarcaId",
                table: "Promociones");

            migrationBuilder.DropForeignKey(
                name: "FK_Sucursales_Marcas_MarcaId",
                table: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_Sucursales_MarcaId",
                table: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_Promociones_MarcaId",
                table: "Promociones");

            migrationBuilder.DropIndex(
                name: "IX_Catalogo_MarcaId",
                table: "Catalogo");

            migrationBuilder.DropColumn(
                name: "MarcaId",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "MarcaId",
                table: "Promociones");

            migrationBuilder.DropColumn(
                name: "MarcaId",
                table: "Catalogo");
        }
    }
}
