using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addtableSolicitudDeTarjeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadoSolicitudDeTarjeta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoSolicitudDeTarjeta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudDeTarjeta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Apellido = table.Column<string>(nullable: true),
                    DNI = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FechaNacimiento = table.Column<DateTime>(nullable: false),
                    Domicilio = table.Column<string>(nullable: true),
                    FechaSolicitud = table.Column<DateTime>(nullable: false),
                    FechaDeRechazoAprobacion = table.Column<DateTime>(nullable: false),
                    EstadoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudDeTarjeta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudDeTarjeta_EstadoSolicitudDeTarjeta_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "EstadoSolicitudDeTarjeta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudDeTarjeta_EstadoId",
                table: "SolicitudDeTarjeta",
                column: "EstadoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudDeTarjeta");

            migrationBuilder.DropTable(
                name: "EstadoSolicitudDeTarjeta");
        }
    }
}
