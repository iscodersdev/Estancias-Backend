using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig27EsVideoBanner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsVideo",
                table: "Banners",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "Banners",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsVideo",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Video",
                table: "Banners");
        }
    }
}
