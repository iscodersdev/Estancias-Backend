using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class BannerLinkExterno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LinkExterno",
                table: "Banners",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkExterno",
                table: "Banners");
        }
    }
}
