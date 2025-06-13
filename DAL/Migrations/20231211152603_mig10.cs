using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PromocionFija",
                table: "Promociones",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PromocionesQR",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    PromocionesId = table.Column<int>(nullable: true),
                    Titulo = table.Column<string>(nullable: true),
                    Subtitulo = table.Column<string>(nullable: true),
                    Foto = table.Column<string>(nullable: true),
                    Texto = table.Column<string>(nullable: true),
                    QR = table.Column<string>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromocionesQR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromocionesQR_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromocionesQR_Promociones_PromocionesId",
                        column: x => x.PromocionesId,
                        principalTable: "Promociones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromocionesQR_ClienteId",
                table: "PromocionesQR",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PromocionesQR_PromocionesId",
                table: "PromocionesQR",
                column: "PromocionesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromocionesQR");

            migrationBuilder.DropColumn(
                name: "PromocionFija",
                table: "Promociones");
        }
    }
}
