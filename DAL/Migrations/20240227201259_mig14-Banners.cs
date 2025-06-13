using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig14Banners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Titulo = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    Subtitulo = table.Column<string>(nullable: true),
                    Foto = table.Column<string>(nullable: true),
                    Texto = table.Column<string>(nullable: true),
                    TextoBoton = table.Column<string>(nullable: true),
                    BannerFijo = table.Column<bool>(nullable: false),
                    Publico = table.Column<bool>(nullable: false),
                    EmpresaId = table.Column<int>(nullable: true),
                    ColorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banners_Colores_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Banners_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banners_ColorId",
                table: "Banners",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_EmpresaId",
                table: "Banners",
                column: "EmpresaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");
        }
    }
}
