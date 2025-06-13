using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoProductoId",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PagoTarjeta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonaId = table.Column<int>(nullable: true),
                    NroTarjeta = table.Column<string>(nullable: true),
                    FechaVencimiento = table.Column<DateTime>(nullable: false),
                    MontoAdeudado = table.Column<decimal>(nullable: false),
                    FechaPagoProximaCuota = table.Column<DateTime>(nullable: false),
                    EstadoPago = table.Column<int>(nullable: false),
                    ComprobantePago = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagoTarjeta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagoTarjeta_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Promociones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Titulo = table.Column<string>(nullable: true),
                    Subtitulo = table.Column<string>(nullable: true),
                    Foto = table.Column<string>(nullable: true),
                    Texto = table.Column<string>(nullable: true),
                    TextoBoton = table.Column<string>(nullable: true),
                    Publica = table.Column<bool>(nullable: false),
                    EmpresaId = table.Column<int>(nullable: true),
                    ColorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promociones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promociones_Colores_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Promociones_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Talles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Abreviado = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoMovimientoTarjeta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoMovimientoTarjeta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoProducto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProducto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TallesProductos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductoId = table.Column<int>(nullable: true),
                    TallesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TallesProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TallesProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TallesProductos_Talles_TallesId",
                        column: x => x.TallesId,
                        principalTable: "Talles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_TipoProductoId",
                table: "Productos",
                column: "TipoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoTarjeta_PersonaId",
                table: "PagoTarjeta",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_ColorId",
                table: "Promociones",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_EmpresaId",
                table: "Promociones",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_TallesProductos_ProductoId",
                table: "TallesProductos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_TallesProductos_TallesId",
                table: "TallesProductos",
                column: "TallesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_TipoProducto_TipoProductoId",
                table: "Productos",
                column: "TipoProductoId",
                principalTable: "TipoProducto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_TipoProducto_TipoProductoId",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "PagoTarjeta");

            migrationBuilder.DropTable(
                name: "Promociones");

            migrationBuilder.DropTable(
                name: "TallesProductos");

            migrationBuilder.DropTable(
                name: "TipoMovimientoTarjeta");

            migrationBuilder.DropTable(
                name: "TipoProducto");

            migrationBuilder.DropTable(
                name: "Talles");

            migrationBuilder.DropIndex(
                name: "IX_Productos_TipoProductoId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TipoProductoId",
                table: "Productos");
        }
    }
}
