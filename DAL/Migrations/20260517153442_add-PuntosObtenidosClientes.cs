using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addPuntosObtenidosClientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PuntosObtenidosClientes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<string>(nullable: true),
                    IdSolicitud = table.Column<long>(nullable: false),
                    IdOperacion = table.Column<string>(nullable: true),
                    MontoCompra = table.Column<decimal>(nullable: false),
                    FechaCompra = table.Column<DateTime>(nullable: false),
                    Compania = table.Column<string>(nullable: true),
                    CompaniaId = table.Column<int>(nullable: false),
                    PuntosObtenidos = table.Column<int>(nullable: false),
                    PuntosDisponibles = table.Column<int>(nullable: false),
                    FechaVencimiento = table.Column<DateTime>(nullable: false),
                    FechaProcesada = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntosObtenidosClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PuntosObtenidosClientes_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PuntosObtenidosClientes_UsuarioId",
                table: "PuntosObtenidosClientes",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PuntosObtenidosClientes");
        }
    }
}
