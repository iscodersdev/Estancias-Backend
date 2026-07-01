using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addTableMenuMobile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuMobile",
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
                    table.PrimaryKey("PK_MenuMobile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuMobileUsuariosHabilitados",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MenuMobileId = table.Column<int>(nullable: true),
                    UsuarioId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuMobileUsuariosHabilitados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuMobileUsuariosHabilitados_MenuMobile_MenuMobileId",
                        column: x => x.MenuMobileId,
                        principalTable: "MenuMobile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuMobileUsuariosHabilitados_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuMobileUsuariosHabilitados_MenuMobileId",
                table: "MenuMobileUsuariosHabilitados",
                column: "MenuMobileId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuMobileUsuariosHabilitados_UsuarioId",
                table: "MenuMobileUsuariosHabilitados",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuMobileUsuariosHabilitados");

            migrationBuilder.DropTable(
                name: "MenuMobile");
        }
    }
}
