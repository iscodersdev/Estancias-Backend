using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class MailService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Proveedor = table.Column<string>(nullable: true),
                    CodigoProveedor = table.Column<string>(nullable: true),
                    ApiKey = table.Column<string>(nullable: true),
                    SmtpUser = table.Column<string>(nullable: true),
                    SmtpPass = table.Column<string>(nullable: true),
                    SmtpHost = table.Column<string>(nullable: true),
                    SmtpPort = table.Column<int>(nullable: false),
                    SenderEmail = table.Column<string>(nullable: true),
                    SenderName = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailConfig", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailConfig");
        }
    }
}
