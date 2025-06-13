using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetFunctions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    DeletedById = table.Column<string>(nullable: true),
                    LastEditTime = table.Column<DateTime>(nullable: false),
                    LastEditById = table.Column<string>(nullable: true),
                    Display = table.Column<bool>(nullable: false),
                    Show = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RoutesJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetFunctions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    ShowName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    Show = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    WorkSpaceId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bancos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bancos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Conceptos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Signo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conceptos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CuotasSociales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ValorCuota = table.Column<decimal>(nullable: false),
                    ImpusoCuota = table.Column<string>(nullable: true),
                    Organismo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuotasSociales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatosEstructura",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Calle = table.Column<string>(nullable: true),
                    Sigla = table.Column<string>(nullable: true),
                    Numero = table.Column<string>(nullable: true),
                    CodigoPostal = table.Column<string>(nullable: true),
                    Localidad = table.Column<string>(nullable: true),
                    Provincia = table.Column<string>(nullable: true),
                    CUIT = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    FAX = table.Column<string>(nullable: true),
                    Sucursal = table.Column<string>(nullable: true),
                    CBU = table.Column<string>(nullable: true),
                    Convenio = table.Column<string>(nullable: true),
                    Entidad = table.Column<string>(nullable: true),
                    NombreOrganismo = table.Column<string>(nullable: true),
                    NombreDependencia = table.Column<string>(nullable: true),
                    URLReportes = table.Column<string>(nullable: true),
                    UsuarioReportes = table.Column<string>(nullable: true),
                    CredencialReportes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosEstructura", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DestinoFondos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
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
                name: "EstadosDeudas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosDeudas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosPrestamos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    AnulablePersona = table.Column<bool>(nullable: false),
                    ConfirmablePersona = table.Column<bool>(nullable: false),
                    Transferido = table.Column<bool>(nullable: false),
                    Color = table.Column<string>(nullable: true),
                    EstadoCGEId = table.Column<int>(nullable: false)
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
                    Descripcion = table.Column<string>(nullable: false),
                    Abreviatura = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genero", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.Id);
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
                name: "InstitucionesFinancieras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitucionesFinancieras", x => x.Id);
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
                name: "MockServicios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodigoBarras = table.Column<string>(nullable: true),
                    CodigoServicioFactura = table.Column<string>(nullable: true),
                    NombreServicio = table.Column<string>(nullable: true),
                    Monto = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockServicios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monedas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monedas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrigenMovimiento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TipoOrigen = table.Column<int>(nullable: false),
                    Descripcion = table.Column<string>(nullable: true),
                    IdAsociado = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrigenMovimiento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Latitud = table.Column<string>(nullable: true),
                    Longitud = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    DescripcionCompleta = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Referencias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NombreCompleto = table.Column<string>(nullable: true),
                    Vinculo = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rubros",
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
                    table.PrimaryKey("PK_Rubros", x => x.Id);
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
                    Tasa = table.Column<decimal>(nullable: false),
                    Inversor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasasInversores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDocumento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoMovimientoBilletera",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Credito = table.Column<bool>(nullable: false),
                    Debito = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoMovimientoBilletera", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoPuesto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoPuesto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposAccesos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAccesos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposClientes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    CantidadActividadesSemanales = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposClientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoServicio",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoServicio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposMovimientos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Credito = table.Column<bool>(nullable: false),
                    Debito = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMovimientos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleFunctions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    RoleId = table.Column<string>(nullable: true),
                    FunctionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleFunctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleFunctions_AspNetFunctions_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "AspNetFunctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetRoleFunctions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organismos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    CuotaId = table.Column<int>(nullable: true),
                    Orden = table.Column<int>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
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
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CUIT = table.Column<long>(nullable: false),
                    EntidadIdCGE = table.Column<int>(nullable: false),
                    TokenCGE = table.Column<string>(nullable: true),
                    PasswordCGE = table.Column<string>(nullable: true),
                    RazonSocial = table.Column<string>(nullable: true),
                    Domicilio = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    Mail = table.Column<string>(nullable: true),
                    ColorFontCarnet = table.Column<string>(nullable: true),
                    ColorCarnet = table.Column<string>(nullable: true),
                    Twitter = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    Instagram = table.Column<string>(nullable: true),
                    WhatsApp = table.Column<string>(nullable: true),
                    FondoMobile = table.Column<byte[]>(nullable: true),
                    GIFLogoMutual = table.Column<byte[]>(nullable: true),
                    LogoMutual = table.Column<byte[]>(nullable: true),
                    ImagenLogin = table.Column<byte[]>(nullable: true),
                    GrupoId = table.Column<int>(nullable: true),
                    Abreviatura = table.Column<string>(nullable: true),
                    ColorFondo = table.Column<string>(nullable: true),
                    ColorBotones = table.Column<string>(nullable: true),
                    ColorLogin = table.Column<string>(nullable: true),
                    FechaBaja = table.Column<DateTime>(nullable: true),
                    UrlMail = table.Column<string>(nullable: true),
                    UsernameMail = table.Column<string>(nullable: true),
                    PasswordMail = table.Column<string>(nullable: true),
                    PuertoMail = table.Column<int>(nullable: false),
                    SSLMail = table.Column<bool>(nullable: false),
                    CasillaMail = table.Column<string>(nullable: true),
                    MailBienvenida = table.Column<bool>(nullable: false),
                    CabeceraMail = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empresas_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Localidad",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Latitud = table.Column<string>(nullable: true),
                    Longitud = table.Column<string>(nullable: true),
                    IdDepartamento = table.Column<int>(nullable: false),
                    IdProvincia = table.Column<int>(nullable: false),
                    Descripcion = table.Column<string>(nullable: true),
                    ProvinciaNombre = table.Column<string>(nullable: true),
                    GuarnicionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Localidad_Guarniciones_GuarnicionId",
                        column: x => x.GuarnicionId,
                        principalTable: "Guarniciones",
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
                    Nombre = table.Column<string>(nullable: true),
                    CapitalMinimo = table.Column<decimal>(nullable: false),
                    CapitalMaximo = table.Column<decimal>(nullable: false),
                    CuotaMinima = table.Column<decimal>(nullable: false),
                    CuotaMaxima = table.Column<decimal>(nullable: false),
                    Intervalo = table.Column<decimal>(nullable: false),
                    CantidadCuotasMinima = table.Column<int>(nullable: false),
                    CantidadCuotasMaxima = table.Column<int>(nullable: false),
                    SistemaFinanciacionId = table.Column<int>(nullable: true),
                    MonedaId = table.Column<int>(nullable: true),
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
                    Nombre = table.Column<string>(nullable: true),
                    Domicilio = table.Column<string>(nullable: true),
                    CUIT = table.Column<string>(nullable: true),
                    TasaActualId = table.Column<int>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
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
                    nombre = table.Column<string>(nullable: true),
                    LimiteCuotas = table.Column<int>(nullable: false),
                    TopePrestamo = table.Column<decimal>(nullable: false),
                    OrganismoId = table.Column<int>(nullable: true)
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
                name: "Campanas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    FechaBaja = table.Column<DateTime>(nullable: true),
                    EmpresaId = table.Column<int>(nullable: true),
                    Observaciones = table.Column<string>(nullable: true),
                    TextoMail = table.Column<string>(nullable: true),
                    ProvinciaId = table.Column<int>(nullable: false),
                    MinimoDisponible = table.Column<decimal>(nullable: false),
                    MaximoDisponible = table.Column<decimal>(nullable: false),
                    UnidadId = table.Column<int>(nullable: false),
                    CantidadPersonas = table.Column<int>(nullable: false),
                    CantidadUnidades = table.Column<int>(nullable: false),
                    CantidadProvincias = table.Column<int>(nullable: false),
                    Imagen = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campanas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campanas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Novedades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Titulo = table.Column<string>(nullable: true),
                    Foto = table.Column<string>(nullable: true),
                    Texto = table.Column<string>(nullable: true),
                    Publica = table.Column<bool>(nullable: false),
                    EmpresaId = table.Column<int>(nullable: true),
                    ColorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Novedades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Novedades_Colores_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Novedades_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    CUIT = table.Column<long>(nullable: false),
                    RazonSocial = table.Column<string>(nullable: true),
                    Domicilio = table.Column<string>(nullable: true),
                    EmpresaId = table.Column<int>(nullable: true),
                    Foto = table.Column<byte[]>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proveedores_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Puestos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    EmpresaId = table.Column<int>(nullable: true),
                    TipoId = table.Column<int>(nullable: true),
                    Coordenadas = table.Column<string>(nullable: true),
                    FechaBaja = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Puestos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puestos_TipoPuesto_TipoId",
                        column: x => x.TipoId,
                        principalTable: "TipoPuesto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmpresaId = table.Column<int>(nullable: true),
                    Activo = table.Column<bool>(nullable: false),
                    FechaBaja = table.Column<DateTime>(nullable: true),
                    Nombre = table.Column<string>(nullable: true),
                    TopeAnualFinde = table.Column<int>(nullable: false),
                    TopeMensualFinde = table.Column<int>(nullable: false),
                    TopePendienteFinde = table.Column<int>(nullable: false),
                    TopeAnualSemana = table.Column<int>(nullable: false),
                    TopeMensualSemana = table.Column<int>(nullable: false),
                    TopePendienteSemana = table.Column<int>(nullable: false),
                    DiasProgramacion = table.Column<int>(nullable: false),
                    TopePendienteGlobal = table.Column<int>(nullable: false),
                    Cupo = table.Column<int>(nullable: false),
                    Icono = table.Column<byte[]>(nullable: true),
                    FechaUnica = table.Column<DateTime>(nullable: true),
                    Observaciones = table.Column<string>(nullable: true),
                    TipoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Servicios_TipoServicio_TipoId",
                        column: x => x.TipoId,
                        principalTable: "TipoServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineasPrestamosPlanes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineaId = table.Column<int>(nullable: true),
                    MontoPrestado = table.Column<decimal>(nullable: false),
                    CantidadCuotas = table.Column<int>(nullable: false),
                    MontoCuota = table.Column<decimal>(nullable: false),
                    CFT = table.Column<decimal>(nullable: false),
                    MargenDisponible = table.Column<decimal>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
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
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TipoPersonaId = table.Column<int>(nullable: true),
                    TipoDocumentoId = table.Column<int>(nullable: true),
                    NroDocumento = table.Column<string>(nullable: true),
                    Apellido = table.Column<string>(nullable: false),
                    Nombres = table.Column<string>(nullable: false),
                    Cuil = table.Column<string>(nullable: true),
                    PaisId = table.Column<int>(nullable: true),
                    FechaNacimiento = table.Column<DateTime>(nullable: true),
                    GeneroId = table.Column<int>(nullable: true),
                    EstadoCivilId = table.Column<int>(nullable: true),
                    CantidadHijos = table.Column<int>(nullable: false),
                    Foto = table.Column<byte[]>(nullable: true),
                    FechaActualizacion = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personas_EstadosCiviles_EstadoCivilId",
                        column: x => x.EstadoCivilId,
                        principalTable: "EstadosCiviles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_Genero_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Genero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_Paises_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Paises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_TipoDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TipoDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_TiposPersonas_TipoPersonaId",
                        column: x => x.TipoPersonaId,
                        principalTable: "TiposPersonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampanasRenglones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CabeceraId = table.Column<int>(nullable: true),
                    DNI = table.Column<long>(nullable: false),
                    Apellido = table.Column<string>(nullable: true),
                    Nombres = table.Column<string>(nullable: true),
                    eMail = table.Column<string>(nullable: true),
                    Celular = table.Column<string>(nullable: true),
                    Disponible = table.Column<decimal>(nullable: false),
                    ImporteMaximo = table.Column<decimal>(nullable: false),
                    UnidadId = table.Column<int>(nullable: false),
                    ProvinciaId = table.Column<int>(nullable: false),
                    Unidad = table.Column<string>(nullable: true),
                    Provincia = table.Column<string>(nullable: true),
                    TipoPersona = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampanasRenglones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampanasRenglones_Campanas_CabeceraId",
                        column: x => x.CabeceraId,
                        principalTable: "Campanas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Foto = table.Column<byte[]>(nullable: true),
                    ProveedorId = table.Column<int>(nullable: true),
                    RubroId = table.Column<int>(nullable: true),
                    Activo = table.Column<bool>(nullable: false),
                    DescripcionAmpliada = table.Column<string>(nullable: true),
                    Precio = table.Column<decimal>(nullable: false),
                    Oferta = table.Column<decimal>(nullable: true),
                    Financiable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Rubros_RubroId",
                        column: x => x.RubroId,
                        principalTable: "Rubros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorRubros",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProveedorId = table.Column<int>(nullable: true),
                    RubroId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorRubros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProveedorRubros_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorRubros_Rubros_RubroId",
                        column: x => x.RubroId,
                        principalTable: "Rubros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PuestosCodigos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PuestoId = table.Column<int>(nullable: true),
                    ValidoDesde = table.Column<DateTime>(nullable: false),
                    ValidoHasta = table.Column<DateTime>(nullable: false),
                    HashQR = table.Column<string>(nullable: true),
                    Imagen = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuestosCodigos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PuestosCodigos_Puestos_PuestoId",
                        column: x => x.PuestoId,
                        principalTable: "Puestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientesServicios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TipoClienteId = table.Column<int>(nullable: true),
                    ServicioId = table.Column<int>(nullable: true),
                    TopeMensual = table.Column<int>(nullable: false),
                    TopeSemanal = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesServicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientesServicios_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientesServicios_TiposClientes_TipoClienteId",
                        column: x => x.TipoClienteId,
                        principalTable: "TiposClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServicioId = table.Column<int>(nullable: true),
                    Nombre = table.Column<string>(nullable: true),
                    Orden = table.Column<int>(nullable: false),
                    DiaSemana = table.Column<int>(nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    FechaBaja = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Horarios_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendedores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonaId = table.Column<int>(nullable: true),
                    Domicilio = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    Mail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendedores_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinanciacionProductos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductoId = table.Column<int>(nullable: true),
                    CantidadCuotas = table.Column<int>(nullable: false),
                    InteresesPorCuota = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanciacionProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanciacionProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Mail = table.Column<string>(nullable: true),
                    DeviceId = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    RecordarPassword = table.Column<bool>(nullable: false),
                    ProveedorId = table.Column<int>(nullable: true),
                    VendedoresId = table.Column<int>(nullable: true),
                    PersonasId = table.Column<int>(nullable: true),
                    Administradores = table.Column<bool>(nullable: false),
                    Token = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Personas_PersonasId",
                        column: x => x.PersonasId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Vendedores_VendedoresId",
                        column: x => x.VendedoresId,
                        principalTable: "Vendedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonaId = table.Column<int>(nullable: true),
                    UsuarioId = table.Column<string>(nullable: true),
                    TipoClienteId = table.Column<int>(nullable: true),
                    EmpresaId = table.Column<int>(nullable: true),
                    RazonSocial = table.Column<string>(nullable: true),
                    NumeroCliente = table.Column<string>(nullable: true),
                    Domicilio = table.Column<string>(nullable: true),
                    LocalidadId = table.Column<int>(nullable: true),
                    ProvinciaId = table.Column<int>(nullable: true),
                    CodigoPostal = table.Column<string>(nullable: true),
                    CBU = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true),
                    Celular = table.Column<string>(nullable: true),
                    FechaIngresoLaboral = table.Column<DateTime>(nullable: false),
                    NumeroLegajoLaboral = table.Column<string>(nullable: true),
                    CategoriaLaboral = table.Column<string>(nullable: true),
                    DestinoLaboral = table.Column<string>(nullable: true),
                    NumeroAsociado = table.Column<int>(nullable: false),
                    CodeudorId = table.Column<int>(nullable: true),
                    PersonaPoliticamenteExpuesta = table.Column<bool>(nullable: false),
                    EsMilitar = table.Column<bool>(nullable: false),
                    FechaIngreso = table.Column<DateTime>(nullable: false),
                    FechaBaja = table.Column<DateTime>(nullable: true),
                    ClienteValidado = table.Column<bool>(nullable: false),
                    RecibirPublicidad = table.Column<bool>(nullable: false),
                    NroDocReferido = table.Column<string>(nullable: true),
                    FotoDNIAnverso = table.Column<byte[]>(nullable: true),
                    FotoDNIReverso = table.Column<byte[]>(nullable: true),
                    FotoSosteniendoDNI = table.Column<byte[]>(nullable: true),
                    LegajoElectronico = table.Column<byte[]>(nullable: true),
                    FirmaOlografica = table.Column<byte[]>(nullable: true),
                    FirmaOlograficaConfirmacion = table.Column<byte[]>(nullable: true),
                    ReferenciaAId = table.Column<int>(nullable: true),
                    ReferenciaBId = table.Column<int>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Clientes_CodeudorId",
                        column: x => x.CodeudorId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Localidad_LocalidadId",
                        column: x => x.LocalidadId,
                        principalTable: "Localidad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Provincia_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Referencias_ReferenciaAId",
                        column: x => x.ReferenciaAId,
                        principalTable: "Referencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Referencias_ReferenciaBId",
                        column: x => x.ReferenciaBId,
                        principalTable: "Referencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_TiposClientes_TipoClienteId",
                        column: x => x.TipoClienteId,
                        principalTable: "TiposClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Billeteras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    Saldo = table.Column<decimal>(nullable: false),
                    QRCobro = table.Column<string>(nullable: true),
                    AliasCVU = table.Column<string>(nullable: true),
                    CVU = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billeteras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Billeteras_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuentaCorriente",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Vencimiento = table.Column<DateTime>(nullable: true),
                    Observaciones = table.Column<string>(nullable: true),
                    Importe = table.Column<decimal>(nullable: false),
                    Saldo = table.Column<decimal>(nullable: false),
                    ClienteId = table.Column<int>(nullable: true),
                    ConceptoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaCorriente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentaCorriente_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentaCorriente_Conceptos_ConceptoId",
                        column: x => x.ConceptoId,
                        principalTable: "Conceptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invitaciones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Apellido = table.Column<string>(nullable: true),
                    Nombres = table.Column<string>(nullable: true),
                    Foto = table.Column<byte[]>(nullable: true),
                    NumeroDocumento = table.Column<long>(nullable: false),
                    ClienteId = table.Column<int>(nullable: true),
                    Desde = table.Column<DateTime>(nullable: false),
                    Hasta = table.Column<DateTime>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    QR = table.Column<string>(nullable: true),
                    Hash = table.Column<string>(nullable: true),
                    Observaciones = table.Column<string>(nullable: true),
                    Patente = table.Column<string>(nullable: true),
                    Baja = table.Column<DateTime>(nullable: true),
                    Estado = table.Column<int>(nullable: false),
                    Completado = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitaciones_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionesPersonas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    FechaHora = table.Column<DateTime>(nullable: false),
                    Titulo = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    TomaConocimiento = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionesPersonas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacionesPersonas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    Domicilio = table.Column<string>(nullable: true),
                    LineaId = table.Column<int>(nullable: true),
                    DestinoFondosId = table.Column<int>(nullable: true),
                    Capital = table.Column<decimal>(nullable: false),
                    EstadoActualId = table.Column<int>(nullable: true),
                    FechaSolicitado = table.Column<DateTime>(nullable: true),
                    FechaAprobacion = table.Column<DateTime>(nullable: true),
                    FechaLiquidacion = table.Column<DateTime>(nullable: true),
                    CantidadCuotas = table.Column<int>(nullable: false),
                    FechaPrimerVencimiento = table.Column<DateTime>(nullable: true),
                    IngresadoPorId = table.Column<string>(nullable: true),
                    AprobadoPorId = table.Column<string>(nullable: true),
                    LiquidadoPorId = table.Column<string>(nullable: true),
                    Observaciones = table.Column<string>(nullable: true),
                    OficialCuentaId = table.Column<string>(nullable: true),
                    CBU = table.Column<string>(nullable: true),
                    FechaPago = table.Column<DateTime>(nullable: true),
                    FormaPagoId = table.Column<int>(nullable: true),
                    FechaAnulacion = table.Column<DateTime>(nullable: true),
                    ObservacionesAnulacion = table.Column<string>(nullable: true),
                    PrestamoCGEId = table.Column<int>(nullable: false),
                    LegajoElectronico = table.Column<byte[]>(nullable: true),
                    FirmaOlografica = table.Column<byte[]>(nullable: true),
                    MontoCuota = table.Column<decimal>(nullable: false),
                    FirmaOlograficaConfirmacion = table.Column<byte[]>(nullable: true),
                    PrestamoNumero = table.Column<int>(nullable: false),
                    Saldo = table.Column<decimal>(nullable: false),
                    CuotasRestantes = table.Column<int>(nullable: false),
                    CapitalEnLetras = table.Column<string>(nullable: true),
                    CuotasEnLetras = table.Column<string>(nullable: true),
                    MontoCuotaEnLetras = table.Column<string>(nullable: true),
                    CFT = table.Column<decimal>(nullable: false),
                    FotoReciboHaber = table.Column<byte[]>(nullable: true),
                    FotoCertificadoDescuento = table.Column<byte[]>(nullable: true),
                    AdjuntoTransferencia = table.Column<byte[]>(nullable: true),
                    ExtensionAdjuntoTransferencia = table.Column<string>(nullable: true),
                    VendedorId = table.Column<int>(nullable: true),
                    Tipos = table.Column<int>(nullable: false)
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
                name: "ProductosClientes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Foto = table.Column<byte[]>(nullable: true),
                    ClienteId = table.Column<int>(nullable: true),
                    RubroId = table.Column<int>(nullable: true),
                    Activo = table.Column<bool>(nullable: false),
                    DescripcionAmpliada = table.Column<string>(nullable: true),
                    Precio = table.Column<decimal>(nullable: false),
                    Oferta = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductosClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductosClientes_Rubros_RubroId",
                        column: x => x.RubroId,
                        principalTable: "Rubros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductosPrecompras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductoId = table.Column<int>(nullable: true),
                    ClienteId = table.Column<int>(nullable: true),
                    FechaPrecompra = table.Column<DateTime>(nullable: false),
                    FechaAnulacion = table.Column<DateTime>(nullable: false),
                    FechaConfirmacion = table.Column<DateTime>(nullable: false),
                    Estado = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosPrecompras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductosPrecompras_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductosPrecompras_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HorarioId = table.Column<int>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Observaciones = table.Column<string>(nullable: true),
                    ClienteId = table.Column<int>(nullable: true),
                    FechaAnulada = table.Column<DateTime>(nullable: true),
                    ClienteAnulaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Clientes_ClienteAnulaId",
                        column: x => x.ClienteAnulaId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Horarios_HorarioId",
                        column: x => x.HorarioId,
                        principalTable: "Horarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UAT",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonaId = table.Column<int>(nullable: true),
                    ClienteId = table.Column<int>(nullable: true),
                    UsuarioId = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    FechaHora = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UAT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UAT_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UAT_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UAT_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactosBilletera",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteContactoId = table.Column<int>(nullable: true),
                    Detalle = table.Column<string>(nullable: true),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    BilleteraId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactosBilletera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactosBilletera_Billeteras_BilleteraId",
                        column: x => x.BilleteraId,
                        principalTable: "Billeteras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactosBilletera_Clientes_ClienteContactoId",
                        column: x => x.ClienteContactoId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuentasBancarias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Numero = table.Column<string>(nullable: true),
                    CBU = table.Column<string>(nullable: true),
                    Titular = table.Column<string>(nullable: true),
                    Alias = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
                    Terceros = table.Column<bool>(nullable: false),
                    BancoId = table.Column<int>(nullable: true),
                    BilleteraId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasBancarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasBancarias_Bancos_BancoId",
                        column: x => x.BancoId,
                        principalTable: "Bancos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasBancarias_Billeteras_BilleteraId",
                        column: x => x.BilleteraId,
                        principalTable: "Billeteras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiciosBilletera",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodigoServicioFactura = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true),
                    BilleteraId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosBilletera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiciosBilletera_Billeteras_BilleteraId",
                        column: x => x.BilleteraId,
                        principalTable: "Billeteras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tarjetas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Numero = table.Column<string>(nullable: true),
                    Titular = table.Column<string>(nullable: true),
                    Vencimiento = table.Column<string>(nullable: true),
                    BancoId = table.Column<int>(nullable: true),
                    InstitucionFinancieraId = table.Column<int>(nullable: true),
                    BilleteraId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarjetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarjetas_Bancos_BancoId",
                        column: x => x.BancoId,
                        principalTable: "Bancos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tarjetas_Billeteras_BilleteraId",
                        column: x => x.BilleteraId,
                        principalTable: "Billeteras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tarjetas_InstitucionesFinancieras_InstitucionFinancieraId",
                        column: x => x.InstitucionFinancieraId,
                        principalTable: "InstitucionesFinancieras",
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
                    Fecha = table.Column<DateTime>(nullable: true),
                    Credito = table.Column<decimal>(nullable: false),
                    Debito = table.Column<decimal>(nullable: false),
                    Saldo = table.Column<decimal>(nullable: false),
                    TipoMovimientoId = table.Column<int>(nullable: true),
                    PrestamoId = table.Column<int>(nullable: true)
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
                    Fecha = table.Column<DateTime>(nullable: false),
                    PrestamoId = table.Column<int>(nullable: true),
                    ResidenteZonaLimistrofe = table.Column<bool>(nullable: false),
                    FrecuenciaAnualCreditos = table.Column<int>(nullable: false),
                    DeclaraBienesInmuebles = table.Column<bool>(nullable: false),
                    DeclaraBienesMueblesRegistrables = table.Column<bool>(nullable: false),
                    CuentaCorrientePesos = table.Column<bool>(nullable: false),
                    CuentaCorrienteDolares = table.Column<bool>(nullable: false),
                    OtrasInversiones = table.Column<bool>(nullable: false)
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
                name: "ProductosClientesContactos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteCompradorId = table.Column<int>(nullable: true),
                    ProductoId = table.Column<int>(nullable: true),
                    FechaContacto = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosClientesContactos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductosClientesContactos_Clientes_ClienteCompradorId",
                        column: x => x.ClienteCompradorId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductosClientesContactos_ProductosClientes_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "ProductosClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accesos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    PuestoId = table.Column<int>(nullable: true),
                    TipoAccesoId = table.Column<int>(nullable: true),
                    Coordenadas = table.Column<string>(nullable: true),
                    ValidadoPorGPS = table.Column<bool>(nullable: false),
                    FechaHora = table.Column<DateTime>(nullable: false),
                    Observaciones = table.Column<string>(nullable: true),
                    UATPuestoId = table.Column<int>(nullable: true),
                    Deuda = table.Column<decimal>(nullable: false),
                    SinCuentaCorriente = table.Column<bool>(nullable: false),
                    EstadoDeudaId = table.Column<int>(nullable: true),
                    Turnos = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accesos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accesos_EstadosDeudas_EstadoDeudaId",
                        column: x => x.EstadoDeudaId,
                        principalTable: "EstadosDeudas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accesos_Puestos_PuestoId",
                        column: x => x.PuestoId,
                        principalTable: "Puestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accesos_TiposAccesos_TipoAccesoId",
                        column: x => x.TipoAccesoId,
                        principalTable: "TiposAccesos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accesos_UAT_UATPuestoId",
                        column: x => x.UATPuestoId,
                        principalTable: "UAT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosBilletera",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrigenAsociadoId = table.Column<int>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    TipoMovimientoId = table.Column<int>(nullable: true),
                    Monto = table.Column<decimal>(nullable: false),
                    QR = table.Column<string>(nullable: true),
                    CBU = table.Column<string>(nullable: true),
                    BilleteraId = table.Column<int>(nullable: true),
                    CuentaBancariaId = table.Column<int>(nullable: true),
                    TarjetaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosBilletera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosBilletera_Billeteras_BilleteraId",
                        column: x => x.BilleteraId,
                        principalTable: "Billeteras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosBilletera_CuentasBancarias_CuentaBancariaId",
                        column: x => x.CuentaBancariaId,
                        principalTable: "CuentasBancarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosBilletera_OrigenMovimiento_OrigenAsociadoId",
                        column: x => x.OrigenAsociadoId,
                        principalTable: "OrigenMovimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosBilletera_Tarjetas_TarjetaId",
                        column: x => x.TarjetaId,
                        principalTable: "Tarjetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosBilletera_TipoMovimientoBilletera_TipoMovimientoId",
                        column: x => x.TipoMovimientoId,
                        principalTable: "TipoMovimientoBilletera",
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
                    ProbabilidadId = table.Column<int>(nullable: true),
                    ConsecuenciaId = table.Column<int>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ComprasProductos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(nullable: true),
                    ProductoId = table.Column<int>(nullable: true),
                    MovimientoId = table.Column<int>(nullable: true),
                    PrestamoId = table.Column<int>(nullable: true),
                    FechaCompra = table.Column<DateTime>(nullable: false),
                    Estado = table.Column<int>(nullable: false),
                    TipoCompra = table.Column<int>(nullable: false),
                    FechaAnulacion = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprasProductos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasProductos_MovimientosBilletera_MovimientoId",
                        column: x => x.MovimientoId,
                        principalTable: "MovimientosBilletera",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasProductos_Prestamos_PrestamoId",
                        column: x => x.PrestamoId,
                        principalTable: "Prestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accesos_ClienteId",
                table: "Accesos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Accesos_EstadoDeudaId",
                table: "Accesos",
                column: "EstadoDeudaId");

            migrationBuilder.CreateIndex(
                name: "IX_Accesos_PuestoId",
                table: "Accesos",
                column: "PuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Accesos_TipoAccesoId",
                table: "Accesos",
                column: "TipoAccesoId");

            migrationBuilder.CreateIndex(
                name: "IX_Accesos_UATPuestoId",
                table: "Accesos",
                column: "UATPuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleFunctions_FunctionId",
                table: "AspNetRoleFunctions",
                column: "FunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleFunctions_RoleId",
                table: "AspNetRoleFunctions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonasId",
                table: "AspNetUsers",
                column: "PersonasId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProveedorId",
                table: "AspNetUsers",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VendedoresId",
                table: "AspNetUsers",
                column: "VendedoresId");

            migrationBuilder.CreateIndex(
                name: "IX_Billeteras_ClienteId",
                table: "Billeteras",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Campanas_EmpresaId",
                table: "Campanas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_CampanasRenglones_CabeceraId",
                table: "CampanasRenglones",
                column: "CabeceraId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CodeudorId",
                table: "Clientes",
                column: "CodeudorId",
                unique: true,
                filter: "[CodeudorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_EmpresaId",
                table: "Clientes",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_LocalidadId",
                table: "Clientes",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_PersonaId",
                table: "Clientes",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ProvinciaId",
                table: "Clientes",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ReferenciaAId",
                table: "Clientes",
                column: "ReferenciaAId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ReferenciaBId",
                table: "Clientes",
                column: "ReferenciaBId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoClienteId",
                table: "Clientes",
                column: "TipoClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesServicios_ServicioId",
                table: "ClientesServicios",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesServicios_TipoClienteId",
                table: "ClientesServicios",
                column: "TipoClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProductos_ClienteId",
                table: "ComprasProductos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProductos_MovimientoId",
                table: "ComprasProductos",
                column: "MovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProductos_PrestamoId",
                table: "ComprasProductos",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasProductos_ProductoId",
                table: "ComprasProductos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactosBilletera_BilleteraId",
                table: "ContactosBilletera",
                column: "BilleteraId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactosBilletera_ClienteContactoId",
                table: "ContactosBilletera",
                column: "ClienteContactoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaCorriente_ClienteId",
                table: "CuentaCorriente",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaCorriente_ConceptoId",
                table: "CuentaCorriente",
                column: "ConceptoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_BancoId",
                table: "CuentasBancarias",
                column: "BancoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_BilleteraId",
                table: "CuentasBancarias",
                column: "BilleteraId");

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
                name: "IX_Empresas_GrupoId",
                table: "Empresas",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanciacionProductos_ProductoId",
                table: "FinanciacionProductos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_ServicioId",
                table: "Horarios",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Inversores_TasaActualId",
                table: "Inversores",
                column: "TasaActualId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_ClienteId",
                table: "Invitaciones",
                column: "ClienteId");

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
                name: "IX_Localidad_GuarnicionId",
                table: "Localidad",
                column: "GuarnicionId");

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
                name: "IX_MovimientosBilletera_BilleteraId",
                table: "MovimientosBilletera",
                column: "BilleteraId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosBilletera_CuentaBancariaId",
                table: "MovimientosBilletera",
                column: "CuentaBancariaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosBilletera_OrigenAsociadoId",
                table: "MovimientosBilletera",
                column: "OrigenAsociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosBilletera_TarjetaId",
                table: "MovimientosBilletera",
                column: "TarjetaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosBilletera_TipoMovimientoId",
                table: "MovimientosBilletera",
                column: "TipoMovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionesPersonas_ClienteId",
                table: "NotificacionesPersonas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Novedades_ColorId",
                table: "Novedades",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Novedades_EmpresaId",
                table: "Novedades",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Organismos_CuotaId",
                table: "Organismos",
                column: "CuotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_EstadoCivilId",
                table: "Personas",
                column: "EstadoCivilId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_GeneroId",
                table: "Personas",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_PaisId",
                table: "Personas",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoDocumentoId",
                table: "Personas",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoPersonaId",
                table: "Personas",
                column: "TipoPersonaId");

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
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_RubroId",
                table: "Productos",
                column: "RubroId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosClientes_ClienteId",
                table: "ProductosClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosClientes_RubroId",
                table: "ProductosClientes",
                column: "RubroId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosClientesContactos_ClienteCompradorId",
                table: "ProductosClientesContactos",
                column: "ClienteCompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosClientesContactos_ProductoId",
                table: "ProductosClientesContactos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosPrecompras_ClienteId",
                table: "ProductosPrecompras",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosPrecompras_ProductoId",
                table: "ProductosPrecompras",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_EmpresaId",
                table: "Proveedores",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorRubros_ProveedorId",
                table: "ProveedorRubros",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorRubros_RubroId",
                table: "ProveedorRubros",
                column: "RubroId");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_EmpresaId",
                table: "Puestos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_TipoId",
                table: "Puestos",
                column: "TipoId");

            migrationBuilder.CreateIndex(
                name: "IX_PuestosCodigos_PuestoId",
                table: "PuestosCodigos",
                column: "PuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClienteAnulaId",
                table: "Reservas",
                column: "ClienteAnulaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClienteId",
                table: "Reservas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HorarioId",
                table: "Reservas",
                column: "HorarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_EmpresaId",
                table: "Servicios",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_TipoId",
                table: "Servicios",
                column: "TipoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosBilletera_BilleteraId",
                table: "ServiciosBilletera",
                column: "BilleteraId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_BancoId",
                table: "Tarjetas",
                column: "BancoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_BilleteraId",
                table: "Tarjetas",
                column: "BilleteraId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_InstitucionFinancieraId",
                table: "Tarjetas",
                column: "InstitucionFinancieraId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposPersonas_OrganismoId",
                table: "TiposPersonas",
                column: "OrganismoId");

            migrationBuilder.CreateIndex(
                name: "IX_UAT_ClienteId",
                table: "UAT",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_UAT_PersonaId",
                table: "UAT",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_UAT_UsuarioId",
                table: "UAT",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendedores_PersonaId",
                table: "Vendedores",
                column: "PersonaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accesos");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetRoleFunctions");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CampanasRenglones");

            migrationBuilder.DropTable(
                name: "ClientesServicios");

            migrationBuilder.DropTable(
                name: "ComprasProductos");

            migrationBuilder.DropTable(
                name: "ContactosBilletera");

            migrationBuilder.DropTable(
                name: "CuentaCorriente");

            migrationBuilder.DropTable(
                name: "CuentasCorrientes");

            migrationBuilder.DropTable(
                name: "DatosEstructura");

            migrationBuilder.DropTable(
                name: "DestinoFondosRubros");

            migrationBuilder.DropTable(
                name: "FinanciacionProductos");

            migrationBuilder.DropTable(
                name: "Inversores");

            migrationBuilder.DropTable(
                name: "Invitaciones");

            migrationBuilder.DropTable(
                name: "LineasPrestamosPlanes");

            migrationBuilder.DropTable(
                name: "LineasPrestamosTiposPersonas");

            migrationBuilder.DropTable(
                name: "MatrizRiesgoRenglones");

            migrationBuilder.DropTable(
                name: "MockServicios");

            migrationBuilder.DropTable(
                name: "NotificacionesPersonas");

            migrationBuilder.DropTable(
                name: "Novedades");

            migrationBuilder.DropTable(
                name: "ProductosClientesContactos");

            migrationBuilder.DropTable(
                name: "ProductosPrecompras");

            migrationBuilder.DropTable(
                name: "ProveedorRubros");

            migrationBuilder.DropTable(
                name: "PuestosCodigos");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "ServiciosBilletera");

            migrationBuilder.DropTable(
                name: "EstadosDeudas");

            migrationBuilder.DropTable(
                name: "TiposAccesos");

            migrationBuilder.DropTable(
                name: "UAT");

            migrationBuilder.DropTable(
                name: "AspNetFunctions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Campanas");

            migrationBuilder.DropTable(
                name: "MovimientosBilletera");

            migrationBuilder.DropTable(
                name: "Conceptos");

            migrationBuilder.DropTable(
                name: "TiposMovimientos");

            migrationBuilder.DropTable(
                name: "TasasInversores");

            migrationBuilder.DropTable(
                name: "MatrizRiesgoCabeceras");

            migrationBuilder.DropTable(
                name: "MatrizConsecuencias");

            migrationBuilder.DropTable(
                name: "MatrizProbabilidades");

            migrationBuilder.DropTable(
                name: "Colores");

            migrationBuilder.DropTable(
                name: "ProductosClientes");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Puestos");

            migrationBuilder.DropTable(
                name: "Horarios");

            migrationBuilder.DropTable(
                name: "CuentasBancarias");

            migrationBuilder.DropTable(
                name: "OrigenMovimiento");

            migrationBuilder.DropTable(
                name: "Tarjetas");

            migrationBuilder.DropTable(
                name: "TipoMovimientoBilletera");

            migrationBuilder.DropTable(
                name: "Prestamos");

            migrationBuilder.DropTable(
                name: "Rubros");

            migrationBuilder.DropTable(
                name: "TipoPuesto");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Bancos");

            migrationBuilder.DropTable(
                name: "Billeteras");

            migrationBuilder.DropTable(
                name: "InstitucionesFinancieras");

            migrationBuilder.DropTable(
                name: "DestinoFondos");

            migrationBuilder.DropTable(
                name: "EstadosPrestamos");

            migrationBuilder.DropTable(
                name: "FormasPago");

            migrationBuilder.DropTable(
                name: "LineasPrestamos");

            migrationBuilder.DropTable(
                name: "TipoServicio");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Monedas");

            migrationBuilder.DropTable(
                name: "SistemasFinanciacion");

            migrationBuilder.DropTable(
                name: "Localidad");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "Referencias");

            migrationBuilder.DropTable(
                name: "TiposClientes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Guarniciones");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Vendedores");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "EstadosCiviles");

            migrationBuilder.DropTable(
                name: "Genero");

            migrationBuilder.DropTable(
                name: "Paises");

            migrationBuilder.DropTable(
                name: "TipoDocumento");

            migrationBuilder.DropTable(
                name: "TiposPersonas");

            migrationBuilder.DropTable(
                name: "Organismos");

            migrationBuilder.DropTable(
                name: "CuotasSociales");
        }
    }
}
