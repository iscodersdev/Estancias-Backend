using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig30MercadoPago : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConciliacionDePago",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<string>(nullable: true),
                    MercadoPagoId = table.Column<string>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Monto = table.Column<decimal>(nullable: false),
                    Descripcion = table.Column<string>(nullable: true),
                    Estado = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConciliacionDePago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConciliacionDePago_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConciliacionDePago_UsuarioId",
                table: "ConciliacionDePago",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConciliacionDePago");
        }
    }
}
