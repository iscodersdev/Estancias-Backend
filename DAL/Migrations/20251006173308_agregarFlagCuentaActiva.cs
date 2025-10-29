using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class agregarFlagCuentaActiva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activo",
                table: "AspNetUsers");
        }
    }
}
