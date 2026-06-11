using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addsucursalmarca : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sucursales_Marcas_MarcaId",
                table: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_Sucursales_MarcaId",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "MarcaId",
                table: "Sucursales");

            migrationBuilder.CreateTable(
                name: "SucursalesMarcas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SucursalesId = table.Column<int>(nullable: true),
                    MarcaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SucursalesMarcas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SucursalesMarcas_Marcas_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SucursalesMarcas_Sucursales_SucursalesId",
                        column: x => x.SucursalesId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SucursalesMarcas_MarcaId",
                table: "SucursalesMarcas",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_SucursalesMarcas_SucursalesId",
                table: "SucursalesMarcas",
                column: "SucursalesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SucursalesMarcas");

            migrationBuilder.AddColumn<int>(
                name: "MarcaId",
                table: "Sucursales",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_MarcaId",
                table: "Sucursales",
                column: "MarcaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sucursales_Marcas_MarcaId",
                table: "Sucursales",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
