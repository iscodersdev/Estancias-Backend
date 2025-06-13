using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig23PremiosCanje : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistorialDePuntos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PuntosObtenidos = table.Column<int>(nullable: false),
                    PuntosTotales = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialDePuntos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PuntosClientes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    NroTarjeta = table.Column<string>(nullable: true),
                    Puntos = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntosClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PuntosClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RelacionPuntos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Monto = table.Column<decimal>(nullable: false),
                    Puntos = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelacionPuntos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Premios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Stock = table.Column<int>(nullable: false),
                    StockActual = table.Column<int>(nullable: false),
                    Puntos = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    CategoriaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Premios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Premios_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialCanje",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PremioId = table.Column<int>(nullable: true),
                    ClienteId = table.Column<int>(nullable: true),
                    NroTarjeta = table.Column<string>(nullable: true),
                    PuntosConsumidos = table.Column<int>(nullable: false),
                    PuntosRestantes = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCanje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialCanje_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialCanje_Premios_PremioId",
                        column: x => x.PremioId,
                        principalTable: "Premios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCanje_ClienteId",
                table: "HistorialCanje",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCanje_PremioId",
                table: "HistorialCanje",
                column: "PremioId");

            migrationBuilder.CreateIndex(
                name: "IX_Premios_CategoriaId",
                table: "Premios",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_PuntosClientes_ClienteId",
                table: "PuntosClientes",
                column: "ClienteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialCanje");

            migrationBuilder.DropTable(
                name: "HistorialDePuntos");

            migrationBuilder.DropTable(
                name: "PuntosClientes");

            migrationBuilder.DropTable(
                name: "RelacionPuntos");

            migrationBuilder.DropTable(
                name: "Premios");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
