using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Notificaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificacionesPlantillas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Titulo = table.Column<string>(nullable: true),
                    Mensaje = table.Column<string>(nullable: true),
                    ImagenUrl = table.Column<string>(nullable: true),
                    DeepLink = table.Column<string>(nullable: true),
                    PreferLargeImage = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionesPlantillas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoNotificacionesProcedimientos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Codigo = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoNotificacionesProcedimientos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Codigo = table.Column<string>(nullable: true),
                    FechaEjecucion = table.Column<DateTime>(nullable: false),
                    FechaUltimaEjecucion = table.Column<DateTime>(nullable: false),
                    Variable1 = table.Column<int>(nullable: false),
                    Variable2 = table.Column<int>(nullable: false),
                    Variable3 = table.Column<int>(nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    TipoNotificacionesProcedimientosId = table.Column<int>(nullable: true),
                    NotificacionesPlantillasId = table.Column<int>(nullable: true),
                    ListaDistribucionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_ListaDistribucion_ListaDistribucionId",
                        column: x => x.ListaDistribucionId,
                        principalTable: "ListaDistribucion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notificaciones_NotificacionesPlantillas_NotificacionesPlantillasId",
                        column: x => x.NotificacionesPlantillasId,
                        principalTable: "NotificacionesPlantillas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notificaciones_TipoNotificacionesProcedimientos_TipoNotificacionesProcedimientosId",
                        column: x => x.TipoNotificacionesProcedimientosId,
                        principalTable: "TipoNotificacionesProcedimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_ListaDistribucionId",
                table: "Notificaciones",
                column: "ListaDistribucionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_NotificacionesPlantillasId",
                table: "Notificaciones",
                column: "NotificacionesPlantillasId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_TipoNotificacionesProcedimientosId",
                table: "Notificaciones",
                column: "TipoNotificacionesProcedimientosId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "NotificacionesPlantillas");

            migrationBuilder.DropTable(
                name: "TipoNotificacionesProcedimientos");
        }
    }
}
