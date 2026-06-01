using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addmarcas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Imagen",
                table: "RelacionPuntos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiasDeVencimiento",
                table: "Premios",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MarcasId",
                table: "Premios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MarcasId",
                table: "Banners",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Imagen = table.Column<byte[]>(nullable: true),
                    Orden = table.Column<int>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Premios_MarcasId",
                table: "Premios",
                column: "MarcasId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_MarcasId",
                table: "Banners",
                column: "MarcasId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Marcas_MarcasId",
                table: "Banners",
                column: "MarcasId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Premios_Marcas_MarcasId",
                table: "Premios",
                column: "MarcasId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Marcas_MarcasId",
                table: "Banners");

            migrationBuilder.DropForeignKey(
                name: "FK_Premios_Marcas_MarcasId",
                table: "Premios");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropIndex(
                name: "IX_Premios_MarcasId",
                table: "Premios");

            migrationBuilder.DropIndex(
                name: "IX_Banners_MarcasId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Imagen",
                table: "RelacionPuntos");

            migrationBuilder.DropColumn(
                name: "DiasDeVencimiento",
                table: "Premios");

            migrationBuilder.DropColumn(
                name: "MarcasId",
                table: "Premios");

            migrationBuilder.DropColumn(
                name: "MarcasId",
                table: "Banners");
        }
    }
}
