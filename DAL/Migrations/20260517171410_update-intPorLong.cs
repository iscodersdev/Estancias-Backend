using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class updateintPorLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PuntosObtenidos",
                table: "PuntosObtenidosClientes",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PuntosDisponibles",
                table: "PuntosObtenidosClientes",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PuntosObtenidos",
                table: "PuntosObtenidosClientes",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PuntosDisponibles",
                table: "PuntosObtenidosClientes",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
