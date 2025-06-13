using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordWS",
                table: "DatosEstructura",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsernameWS",
                table: "DatosEstructura",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordWS",
                table: "DatosEstructura");

            migrationBuilder.DropColumn(
                name: "UsernameWS",
                table: "DatosEstructura");
        }
    }
}
