using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class UsuariosCategorias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuariosCategoriasId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UsuariosCategorias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    CodigoColor = table.Column<string>(nullable: true),
                    Orden = table.Column<int>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosCategorias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UsuariosCategoriasId",
                table: "AspNetUsers",
                column: "UsuariosCategoriasId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UsuariosCategorias_UsuariosCategoriasId",
                table: "AspNetUsers",
                column: "UsuariosCategoriasId",
                principalTable: "UsuariosCategorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UsuariosCategorias_UsuariosCategoriasId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UsuariosCategorias");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UsuariosCategoriasId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UsuariosCategoriasId",
                table: "AspNetUsers");
        }
    }
}
