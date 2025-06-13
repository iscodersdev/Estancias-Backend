using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComprasProductos_Prestamos_PrestamoId",
                table: "ComprasProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_CuentasBancarias_Bancos_BancoId",
                table: "CuentasBancarias");

            migrationBuilder.DropForeignKey(
                name: "FK_CuentasBancarias_Billeteras_BilleteraId",
                table: "CuentasBancarias");

            migrationBuilder.DropForeignKey(
                name: "FK_Localidad_Guarniciones_GuarnicionId",
                table: "Localidad");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosBilletera_CuentasBancarias_CuentaBancariaId",
                table: "MovimientosBilletera");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosBilletera_Tarjetas_TarjetaId",
                table: "MovimientosBilletera");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_EstadosCiviles_EstadoCivilId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Genero_GeneroId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_TiposPersonas_TipoPersonaId",
                table: "Personas");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosBilletera_Billeteras_BilleteraId",
                table: "ServiciosBilletera");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarjetas_Bancos_BancoId",
                table: "Tarjetas");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarjetas_Billeteras_BilleteraId",
                table: "Tarjetas");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarjetas_InstitucionesFinancieras_InstitucionFinancieraId",
                table: "Tarjetas");

            migrationBuilder.DropTable(
                name: "CuentasCorrientes");

            migrationBuilder.DropTable(
                name: "DestinoFondosRubros");

            migrationBuilder.DropTable(
                name: "EstadosCiviles");

            migrationBuilder.DropTable(
                name: "Genero");

            migrationBuilder.DropTable(
                name: "Guarniciones");

            migrationBuilder.DropTable(
                name: "Inversores");

            migrationBuilder.DropTable(
                name: "LineasPrestamosPlanes");

            migrationBuilder.DropTable(
                name: "LineasPrestamosTiposPersonas");

            migrationBuilder.DropTable(
                name: "MatrizRiesgoRenglones");

            migrationBuilder.DropTable(
                name: "TiposMovimientos");

            migrationBuilder.DropTable(
                name: "TasasInversores");

            migrationBuilder.DropTable(
                name: "TiposPersonas");

            migrationBuilder.DropTable(
                name: "MatrizRiesgoCabeceras");

            migrationBuilder.DropTable(
                name: "MatrizConsecuencias");

            migrationBuilder.DropTable(
                name: "MatrizProbabilidades");

            migrationBuilder.DropTable(
                name: "Organismos");

            migrationBuilder.DropTable(
                name: "Prestamos");

            migrationBuilder.DropTable(
                name: "CuotasSociales");

            migrationBuilder.DropTable(
                name: "DestinoFondos");

            migrationBuilder.DropTable(
                name: "EstadosPrestamos");

            migrationBuilder.DropTable(
                name: "FormasPago");

            migrationBuilder.DropTable(
                name: "LineasPrestamos");

            migrationBuilder.DropTable(
                name: "SistemasFinanciacion");

            migrationBuilder.DropIndex(
                name: "IX_Personas_EstadoCivilId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_GeneroId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_TipoPersonaId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Localidad_GuarnicionId",
                table: "Localidad");

            migrationBuilder.DropIndex(
                name: "IX_ComprasProductos_PrestamoId",
                table: "ComprasProductos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tarjetas",
                table: "Tarjetas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiciosBilletera",
                table: "ServiciosBilletera");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstitucionesFinancieras",
                table: "InstitucionesFinancieras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CuentasBancarias",
                table: "CuentasBancarias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bancos",
                table: "Bancos");

            migrationBuilder.DropColumn(
                name: "EstadoCivilId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "GeneroId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "TipoPersonaId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "GuarnicionId",
                table: "Localidad");

            migrationBuilder.DropColumn(
                name: "NombreOrganismo",
                table: "DatosEstructura");

            migrationBuilder.DropColumn(
                name: "PrestamoId",
                table: "ComprasProductos");

            migrationBuilder.RenameTable(
                name: "Tarjetas",
                newName: "Tarjeta");

            migrationBuilder.RenameTable(
                name: "ServiciosBilletera",
                newName: "ServicioBilletera");

            migrationBuilder.RenameTable(
                name: "InstitucionesFinancieras",
                newName: "InstitucionFinanciera");

            migrationBuilder.RenameTable(
                name: "CuentasBancarias",
                newName: "CuentaBancaria");

            migrationBuilder.RenameTable(
                name: "Bancos",
                newName: "Banco");

            migrationBuilder.RenameIndex(
                name: "IX_Tarjetas_InstitucionFinancieraId",
                table: "Tarjeta",
                newName: "IX_Tarjeta_InstitucionFinancieraId");

            migrationBuilder.RenameIndex(
                name: "IX_Tarjetas_BilleteraId",
                table: "Tarjeta",
                newName: "IX_Tarjeta_BilleteraId");

            migrationBuilder.RenameIndex(
                name: "IX_Tarjetas_BancoId",
                table: "Tarjeta",
                newName: "IX_Tarjeta_BancoId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiciosBilletera_BilleteraId",
                table: "ServicioBilletera",
                newName: "IX_ServicioBilletera_BilleteraId");

            migrationBuilder.RenameIndex(
                name: "IX_CuentasBancarias_BilleteraId",
                table: "CuentaBancaria",
                newName: "IX_CuentaBancaria_BilleteraId");

            migrationBuilder.RenameIndex(
                name: "IX_CuentasBancarias_BancoId",
                table: "CuentaBancaria",
                newName: "IX_CuentaBancaria_BancoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tarjeta",
                table: "Tarjeta",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServicioBilletera",
                table: "ServicioBilletera",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstitucionFinanciera",
                table: "InstitucionFinanciera",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CuentaBancaria",
                table: "CuentaBancaria",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banco",
                table: "Banco",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CuentaBancaria_Banco_BancoId",
                table: "CuentaBancaria",
                column: "BancoId",
                principalTable: "Banco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CuentaBancaria_Billeteras_BilleteraId",
                table: "CuentaBancaria",
                column: "BilleteraId",
                principalTable: "Billeteras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosBilletera_CuentaBancaria_CuentaBancariaId",
                table: "MovimientosBilletera",
                column: "CuentaBancariaId",
                principalTable: "CuentaBancaria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosBilletera_Tarjeta_TarjetaId",
                table: "MovimientosBilletera",
                column: "TarjetaId",
                principalTable: "Tarjeta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServicioBilletera_Billeteras_BilleteraId",
                table: "ServicioBilletera",
                column: "BilleteraId",
                principalTable: "Billeteras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarjeta_Banco_BancoId",
                table: "Tarjeta",
                column: "BancoId",
                principalTable: "Banco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarjeta_Billeteras_BilleteraId",
                table: "Tarjeta",
                column: "BilleteraId",
                principalTable: "Billeteras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarjeta_InstitucionFinanciera_InstitucionFinancieraId",
                table: "Tarjeta",
                column: "InstitucionFinancieraId",
                principalTable: "InstitucionFinanciera",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentaBancaria_Banco_BancoId",
                table: "CuentaBancaria");

            migrationBuilder.DropForeignKey(
                name: "FK_CuentaBancaria_Billeteras_BilleteraId",
                table: "CuentaBancaria");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosBilletera_CuentaBancaria_CuentaBancariaId",
                table: "MovimientosBilletera");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosBilletera_Tarjeta_TarjetaId",
                table: "MovimientosBilletera");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicioBilletera_Billeteras_BilleteraId",
                table: "ServicioBilletera");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarjeta_Banco_BancoId",
                table: "Tarjeta");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarjeta_Billeteras_BilleteraId",
                table: "Tarjeta");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarjeta_InstitucionFinanciera_InstitucionFinancieraId",
                table: "Tarjeta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tarjeta",
                table: "Tarjeta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServicioBilletera",
                table: "ServicioBilletera");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstitucionFinanciera",
                table: "InstitucionFinanciera");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CuentaBancaria",
                table: "CuentaBancaria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banco",
                table: "Banco");

            migrationBuilder.RenameTable(
                name: "Tarjeta",
                newName: "Tarjetas");

            migrationBuilder.RenameTable(
                name: "ServicioBilletera",
                newName: "ServiciosBilletera");

            migrationBuilder.RenameTable(
                name: "InstitucionFinanciera",
                newName: "InstitucionesFinancieras");

            migrationBuilder.RenameTable(
                name: "CuentaBancaria",
                newName: "CuentasBancarias");

            migrationBuilder.RenameTable(
                name: "Banco",
                newName: "Bancos");

            migrationBuilder.RenameIndex(
                name: "IX_Tarjeta_InstitucionFinancieraId",
                table: "Tarjetas",
                newName: "IX_Tarjetas_InstitucionFinancieraId");

            migrationBuilder.RenameIndex(
                name: "IX_Tarjeta_BilleteraId",
                table: "Tarjetas",
                newName: "IX_Tarjetas_BilleteraId");

            migrationBuilder.RenameIndex(
                name: "IX_Tarjeta_BancoId",
                table: "Tarjetas",
                newName: "IX_Tarjetas_BancoId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicioBilletera_BilleteraId",
                table: "ServiciosBilletera",
                newName: "IX_ServiciosBilletera_BilleteraId");

            migrationBuilder.RenameIndex(
                name: "IX_CuentaBancaria_BilleteraId",
                table: "CuentasBancarias",
                newName: "IX_CuentasBancarias_BilleteraId");

            migrationBuilder.RenameIndex(
                name: "IX_CuentaBancaria_BancoId",
                table: "CuentasBancarias",
                newName: "IX_CuentasBancarias_BancoId");

            migrationBuilder.AddColumn<int>(
                name: "EstadoCivilId",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneroId",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoPersonaId",
                table: "Personas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GuarnicionId",
                table: "Localidad",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreOrganismo",
                table: "DatosEstructura",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrestamoId",
                table: "ComprasProductos",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tarjetas",
                table: "Tarjetas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiciosBilletera",
                table: "ServiciosBilletera",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstitucionesFinancieras",
                table: "InstitucionesFinancieras",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CuentasBancarias",
                table: "CuentasBancarias",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bancos",
                table: "Bancos",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CuotasSociales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImpusoCuota = table.Column<string>(nullable: true),
                    Organismo = table.Column<int>(nullable: false),
                    ValorCuota = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuotasSociales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DestinoFondos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activo = table.Column<bool>(nullable: false),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinoFondos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCiviles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCiviles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosPrestamos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnulablePersona = table.Column<bool>(nullable: false),
                    Color = table.Column<string>(nullable: true),
                    ConfirmablePersona = table.Column<bool>(nullable: false),
                    EstadoCGEId = table.Column<int>(nullable: false),
                    Nombre = table.Column<string>(nullable: true),
                    Transferido = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosPrestamos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormasPago",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasPago", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genero",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Abreviatura = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genero", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guarniciones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guarniciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatrizConsecuencias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatrizConsecuencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatrizProbabilidades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatrizProbabilidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SistemasFinanciacion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SistemasFinanciacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TasasInversores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Inversor = table.Column<int>(nullable: false),
                    Tasa = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasasInversores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposMovimientos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Credito = table.Column<bool>(nullable: false),
                    Debito = table.Column<bool>(nullable: false),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMovimientos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organismos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activo = table.Column<bool>(nullable: false),
                    CuotaId = table.Column<int>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Orden = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organismos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organismos_CuotasSociales_CuotaId",
                        column: x => x.CuotaId,
                        principalTable: "CuotasSociales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DestinoFondosRubros",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DestinosFondosId = table.Column<int>(nullable: true),
                    RubroId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinoFondosRubros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinoFondosRubros_DestinoFondos_DestinosFondosId",
                        column: x => x.DestinosFondosId,
                        principalTable: "DestinoFondos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DestinoFondosRubros_Rubros_RubroId",
                        column: x => x.RubroId,
                        principalTable: "Rubros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineasPrestamos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CantidadCuotasMaxima = table.Column<int>(nullable: false),
                    CantidadCuotasMinima = table.Column<int>(nullable: false),
                    CapitalMaximo = table.Column<decimal>(nullable: false),
                    CapitalMinimo = table.Column<decimal>(nullable: false),
                    CuotaMaxima = table.Column<decimal>(nullable: false),
                    CuotaMinima = table.Column<decimal>(nullable: false),
                    Intervalo = table.Column<decimal>(nullable: false),
                    MonedaId = table.Column<int>(nullable: true),
                    Nombre = table.Column<string>(nullable: true),
                    SistemaFinanciacionId = table.Column<int>(nullable: true),
                    TipoDescuentoCGEId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasPrestamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasPrestamos_Monedas_MonedaId",
                        column: x => x.MonedaId,
                        principalTable: "Monedas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LineasPrestamos_SistemasFinanciacion_SistemaFinanciacionId",
                        column: x => x.SistemaFinanciacionId,
                        principalTable: "SistemasFinanciacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inversores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activo = table.Column<bool>(nullable: false),
                    CUIT = table.Column<string>(nullable: true),
                    Domicilio = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true),
                    TasaActualId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inversores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inversores_TasasInversores_TasaActualId",
                        column: x => x.TasaActualId,
                        principalTable: "TasasInversores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposPersonas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LimiteCuotas = table.Column<int>(nullable: false),
                    OrganismoId = table.Column<int>(nullable: true),
                    TopePrestamo = table.Column<decimal>(nullable: false),
                    nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPersonas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposPersonas_Organismos_OrganismoId",
                        column: x => x.OrganismoId,
                        principalTable: "Organismos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineasPrestamosPlanes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activo = table.Column<bool>(nullable: false),
                    CFT = table.Column<decimal>(nullable: false),
                    CantidadCuotas = table.Column<int>(nullable: false),
                    LineaId = table.Column<int>(nullable: true),
                    MargenDisponible = table.Column<decimal>(nullable: false),
                    MontoCuota = table.Column<decimal>(nullable: false),
                    MontoPrestado = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasPrestamosPlanes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasPrestamosPlanes_LineasPrestamos_LineaId",
                        column: x => x.LineaId,
                        principalTable: "LineasPrestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdjuntoTransferencia = table.Column<byte[]>(nullable: true),
                    AprobadoPorId = table.Column<string>(nullable: true),
                    CBU = table.Column<string>(nullable: true),
                    CFT = table.Column<decimal>(nullable: false),
                    CantidadCuotas = table.Column<int>(nullable: false),
                    Capital = table.Column<decimal>(nullable: false),
                    CapitalEnLetras = table.Column<string>(nullable: true),
                    ClienteId = table.Column<int>(nullable: true),
                    CuotasEnLetras = table.Column<string>(nullable: true),
                    CuotasRestantes = table.Column<int>(nullable: false),
                    DestinoFondosId = table.Column<int>(nullable: true),
                    Domicilio = table.Column<string>(nullable: true),
                    EstadoActualId = table.Column<int>(nullable: true),
                    ExtensionAdjuntoTransferencia = table.Column<string>(nullable: true),
                    FechaAnulacion = table.Column<DateTime>(nullable: true),
                    FechaAprobacion = table.Column<DateTime>(nullable: true),
                    FechaLiquidacion = table.Column<DateTime>(nullable: true),
                    FechaPago = table.Column<DateTime>(nullable: true),
                    FechaPrimerVencimiento = table.Column<DateTime>(nullable: true),
                    FechaSolicitado = table.Column<DateTime>(nullable: true),
                    FirmaOlografica = table.Column<byte[]>(nullable: true),
                    FirmaOlograficaConfirmacion = table.Column<byte[]>(nullable: true),
                    FormaPagoId = table.Column<int>(nullable: true),
                    FotoCertificadoDescuento = table.Column<byte[]>(nullable: true),
                    FotoReciboHaber = table.Column<byte[]>(nullable: true),
                    IngresadoPorId = table.Column<string>(nullable: true),
                    LegajoElectronico = table.Column<byte[]>(nullable: true),
                    LineaId = table.Column<int>(nullable: true),
                    LiquidadoPorId = table.Column<string>(nullable: true),
                    MontoCuota = table.Column<decimal>(nullable: false),
                    MontoCuotaEnLetras = table.Column<string>(nullable: true),
                    Observaciones = table.Column<string>(nullable: true),
                    ObservacionesAnulacion = table.Column<string>(nullable: true),
                    OficialCuentaId = table.Column<string>(nullable: true),
                    PrestamoCGEId = table.Column<int>(nullable: false),
                    PrestamoNumero = table.Column<int>(nullable: false),
                    Saldo = table.Column<decimal>(nullable: false),
                    Tipos = table.Column<int>(nullable: false),
                    VendedorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prestamos_AspNetUsers_AprobadoPorId",
                        column: x => x.AprobadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_DestinoFondos_DestinoFondosId",
                        column: x => x.DestinoFondosId,
                        principalTable: "DestinoFondos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_EstadosPrestamos_EstadoActualId",
                        column: x => x.EstadoActualId,
                        principalTable: "EstadosPrestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_FormasPago_FormaPagoId",
                        column: x => x.FormaPagoId,
                        principalTable: "FormasPago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_AspNetUsers_IngresadoPorId",
                        column: x => x.IngresadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_LineasPrestamos_LineaId",
                        column: x => x.LineaId,
                        principalTable: "LineasPrestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_AspNetUsers_LiquidadoPorId",
                        column: x => x.LiquidadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_AspNetUsers_OficialCuentaId",
                        column: x => x.OficialCuentaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_Vendedores_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Vendedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineasPrestamosTiposPersonas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineaPrestamoId = table.Column<int>(nullable: true),
                    TipoPersonaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasPrestamosTiposPersonas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasPrestamosTiposPersonas_LineasPrestamos_LineaPrestamoId",
                        column: x => x.LineaPrestamoId,
                        principalTable: "LineasPrestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LineasPrestamosTiposPersonas_TiposPersonas_TipoPersonaId",
                        column: x => x.TipoPersonaId,
                        principalTable: "TiposPersonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuentasCorrientes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    Credito = table.Column<decimal>(nullable: false),
                    Debito = table.Column<decimal>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: true),
                    PrestamoId = table.Column<int>(nullable: true),
                    Saldo = table.Column<decimal>(nullable: false),
                    TipoMovimientoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasCorrientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasCorrientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasCorrientes_Prestamos_PrestamoId",
                        column: x => x.PrestamoId,
                        principalTable: "Prestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasCorrientes_TiposMovimientos_TipoMovimientoId",
                        column: x => x.TipoMovimientoId,
                        principalTable: "TiposMovimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatrizRiesgoCabeceras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    CuentaCorrienteDolares = table.Column<bool>(nullable: false),
                    CuentaCorrientePesos = table.Column<bool>(nullable: false),
                    DeclaraBienesInmuebles = table.Column<bool>(nullable: false),
                    DeclaraBienesMueblesRegistrables = table.Column<bool>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    FrecuenciaAnualCreditos = table.Column<int>(nullable: false),
                    OtrasInversiones = table.Column<bool>(nullable: false),
                    PrestamoId = table.Column<int>(nullable: true),
                    ResidenteZonaLimistrofe = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatrizRiesgoCabeceras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatrizRiesgoCabeceras_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatrizRiesgoCabeceras_Prestamos_PrestamoId",
                        column: x => x.PrestamoId,
                        principalTable: "Prestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatrizRiesgoRenglones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CabeceraId = table.Column<int>(nullable: true),
                    ConsecuenciaId = table.Column<int>(nullable: true),
                    ProbabilidadId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatrizRiesgoRenglones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatrizRiesgoRenglones_MatrizRiesgoCabeceras_CabeceraId",
                        column: x => x.CabeceraId,
                        principalTable: "MatrizRiesgoCabeceras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatrizRiesgoRenglones_MatrizConsecuencias_ConsecuenciaId",
                        column: x => x.ConsecuenciaId,
                        principalTable: "MatrizConsecuencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatrizRiesgoRenglones_MatrizProbabilidades_ProbabilidadId",
                        column: x => x.ProbabilidadId,
                        principalTable: "MatrizProbabilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personas_EstadoCivilId",
                table: "Personas",
                column: "EstadoCivilId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_GeneroId",
                table: "Personas",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoPersonaId",
                table: "Personas",
                column: "TipoPersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Localidad_GuarnicionId",
                table: "Localidad",
                column: "GuarnicionId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProductos_PrestamoId",
                table: "ComprasProductos",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasCorrientes_ClienteId",
                table: "CuentasCorrientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasCorrientes_PrestamoId",
                table: "CuentasCorrientes",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasCorrientes_TipoMovimientoId",
                table: "CuentasCorrientes",
                column: "TipoMovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinoFondosRubros_DestinosFondosId",
                table: "DestinoFondosRubros",
                column: "DestinosFondosId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinoFondosRubros_RubroId",
                table: "DestinoFondosRubros",
                column: "RubroId");

            migrationBuilder.CreateIndex(
                name: "IX_Inversores_TasaActualId",
                table: "Inversores",
                column: "TasaActualId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasPrestamos_MonedaId",
                table: "LineasPrestamos",
                column: "MonedaId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasPrestamos_SistemaFinanciacionId",
                table: "LineasPrestamos",
                column: "SistemaFinanciacionId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasPrestamosPlanes_LineaId",
                table: "LineasPrestamosPlanes",
                column: "LineaId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasPrestamosTiposPersonas_LineaPrestamoId",
                table: "LineasPrestamosTiposPersonas",
                column: "LineaPrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasPrestamosTiposPersonas_TipoPersonaId",
                table: "LineasPrestamosTiposPersonas",
                column: "TipoPersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_MatrizRiesgoCabeceras_ClienteId",
                table: "MatrizRiesgoCabeceras",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_MatrizRiesgoCabeceras_PrestamoId",
                table: "MatrizRiesgoCabeceras",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_MatrizRiesgoRenglones_CabeceraId",
                table: "MatrizRiesgoRenglones",
                column: "CabeceraId");

            migrationBuilder.CreateIndex(
                name: "IX_MatrizRiesgoRenglones_ConsecuenciaId",
                table: "MatrizRiesgoRenglones",
                column: "ConsecuenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_MatrizRiesgoRenglones_ProbabilidadId",
                table: "MatrizRiesgoRenglones",
                column: "ProbabilidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Organismos_CuotaId",
                table: "Organismos",
                column: "CuotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_AprobadoPorId",
                table: "Prestamos",
                column: "AprobadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_ClienteId",
                table: "Prestamos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_DestinoFondosId",
                table: "Prestamos",
                column: "DestinoFondosId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_EstadoActualId",
                table: "Prestamos",
                column: "EstadoActualId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_FormaPagoId",
                table: "Prestamos",
                column: "FormaPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IngresadoPorId",
                table: "Prestamos",
                column: "IngresadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_LineaId",
                table: "Prestamos",
                column: "LineaId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_LiquidadoPorId",
                table: "Prestamos",
                column: "LiquidadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_OficialCuentaId",
                table: "Prestamos",
                column: "OficialCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_VendedorId",
                table: "Prestamos",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposPersonas_OrganismoId",
                table: "TiposPersonas",
                column: "OrganismoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComprasProductos_Prestamos_PrestamoId",
                table: "ComprasProductos",
                column: "PrestamoId",
                principalTable: "Prestamos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasBancarias_Bancos_BancoId",
                table: "CuentasBancarias",
                column: "BancoId",
                principalTable: "Bancos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasBancarias_Billeteras_BilleteraId",
                table: "CuentasBancarias",
                column: "BilleteraId",
                principalTable: "Billeteras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Localidad_Guarniciones_GuarnicionId",
                table: "Localidad",
                column: "GuarnicionId",
                principalTable: "Guarniciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosBilletera_CuentasBancarias_CuentaBancariaId",
                table: "MovimientosBilletera",
                column: "CuentaBancariaId",
                principalTable: "CuentasBancarias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosBilletera_Tarjetas_TarjetaId",
                table: "MovimientosBilletera",
                column: "TarjetaId",
                principalTable: "Tarjetas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_EstadosCiviles_EstadoCivilId",
                table: "Personas",
                column: "EstadoCivilId",
                principalTable: "EstadosCiviles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Genero_GeneroId",
                table: "Personas",
                column: "GeneroId",
                principalTable: "Genero",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_TiposPersonas_TipoPersonaId",
                table: "Personas",
                column: "TipoPersonaId",
                principalTable: "TiposPersonas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosBilletera_Billeteras_BilleteraId",
                table: "ServiciosBilletera",
                column: "BilleteraId",
                principalTable: "Billeteras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarjetas_Bancos_BancoId",
                table: "Tarjetas",
                column: "BancoId",
                principalTable: "Bancos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarjetas_Billeteras_BilleteraId",
                table: "Tarjetas",
                column: "BilleteraId",
                principalTable: "Billeteras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarjetas_InstitucionesFinancieras_InstitucionFinancieraId",
                table: "Tarjetas",
                column: "InstitucionFinancieraId",
                principalTable: "InstitucionesFinancieras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
