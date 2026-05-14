using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class update_puntoslong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Puntos",
                table: "RelacionPuntos",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "Puntos",
                table: "PuntosClientes",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "Puntos",
                table: "Premios",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PuntosTotales",
                table: "HistorialDePuntos",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PuntosObtenidos",
                table: "HistorialDePuntos",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PuntosRestantes",
                table: "HistorialCanje",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "PuntosConsumidos",
                table: "HistorialCanje",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Puntos",
                table: "RelacionPuntos",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "Puntos",
                table: "PuntosClientes",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "Puntos",
                table: "Premios",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PuntosTotales",
                table: "HistorialDePuntos",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PuntosObtenidos",
                table: "HistorialDePuntos",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PuntosRestantes",
                table: "HistorialCanje",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "PuntosConsumidos",
                table: "HistorialCanje",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
