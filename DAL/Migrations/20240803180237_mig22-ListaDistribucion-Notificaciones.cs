using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig22ListaDistribucionNotificaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnvioNotificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Titulo = table.Column<string>(nullable: true),
                    Texto = table.Column<string>(nullable: true),
                    Foto = table.Column<byte[]>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Envio = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvioNotificaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListaDistribucion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaDistribucion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnvioNotificacionesDestinatarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NotificacionId = table.Column<int>(nullable: true),
                    DestinatarioId = table.Column<string>(nullable: true),
                    Envio = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvioNotificacionesDestinatarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvioNotificacionesDestinatarios_AspNetUsers_DestinatarioId",
                        column: x => x.DestinatarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnvioNotificacionesDestinatarios_EnvioNotificaciones_NotificacionId",
                        column: x => x.NotificacionId,
                        principalTable: "EnvioNotificaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DistribucionDestinatarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ListaDistribucionId = table.Column<int>(nullable: true),
                    DestinatarioId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistribucionDestinatarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistribucionDestinatarios_AspNetUsers_DestinatarioId",
                        column: x => x.DestinatarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DistribucionDestinatarios_ListaDistribucion_ListaDistribucionId",
                        column: x => x.ListaDistribucionId,
                        principalTable: "ListaDistribucion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionDestinatarios_DestinatarioId",
                table: "DistribucionDestinatarios",
                column: "DestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionDestinatarios_ListaDistribucionId",
                table: "DistribucionDestinatarios",
                column: "ListaDistribucionId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioNotificacionesDestinatarios_DestinatarioId",
                table: "EnvioNotificacionesDestinatarios",
                column: "DestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioNotificacionesDestinatarios_NotificacionId",
                table: "EnvioNotificacionesDestinatarios",
                column: "NotificacionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistribucionDestinatarios");

            migrationBuilder.DropTable(
                name: "EnvioNotificacionesDestinatarios");

            migrationBuilder.DropTable(
                name: "ListaDistribucion");

            migrationBuilder.DropTable(
                name: "EnvioNotificaciones");
        }
    }
}
