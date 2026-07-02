using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using EstanciasCore.Interface;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class MCuponesController : BaseApiController
    {
        private readonly MercadoPagoServices _mp;
        private readonly IDatosTarjetaService _datosServices;
        private readonly IHostingEnvironment _webHostEnvironment; 
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MTarjetasController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly ObtenerPuntosService _obtenerPuntosService;

        public MCuponesController(EstanciasContext context, MercadoPagoServices mp, IDatosTarjetaService datosServices, IHostingEnvironment webHostEnvironment, IServiceScopeFactory scopeFactory, ILogger<MTarjetasController> logger, IConfiguration configuration, IMailService mailService, ObtenerPuntosService obtenerPuntosService) : base(context)
        {
            _datosServices = datosServices;
            _mp = mp;
            _webHostEnvironment = webHostEnvironment;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
            _mailService = mailService;
            _obtenerPuntosService = obtenerPuntosService;
        }

                

        [HttpPost("TraeCupones")]
        public async Task<ListCuponesDTO> TraeCupones(ListCuponesDTO request)
        {
            try
            {
                var usuario = TraeUsuarioUAT(request.UAT);
                if (usuario == null)
                    return new ListCuponesDTO { Status = 500, UAT = request.UAT, Mensaje = $"no existe UAT de Usuario" };                            

                var cupones = await _context.Premios.Where(x => x.Activo).Select(x => new CuponesDTO()
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    Stock = x.Stock,
                    StockActual = x.StockActual,
                    Puntos = x.Puntos,
                    Activo = x.Activo,
                    CategoriaId = x.Categoria.Id,
                    CategoriaNombre = x.Categoria.Nombre,
                    FechaVencimiento = x.FechaVencimiento.ToString("dd/MM/yyyy"),
                    DiasDeVencimiento = x.DiasDeVencimiento.ToString(),
                    TerminosCondiciones = x.TerminosCondiciones,
                    Fecha = x.Fecha,
                    Imagen = _context.FotosPremios.Where(f => f.Premio.Id == x.Id).Select(f => f.Foto).FirstOrDefault(),
                    Marca = x.Marcas.Nombre
                }).ToListAsync();

                request.UAT = request.UAT;
                request.Status = 200;
                request.Mensaje = "Exito al traer cupones";
                request.Cupones = cupones.OrderBy(x => x.Fecha).ToList();
                //Trae menu habbilitados
                // 1. Obtenemos directamente de la base de datos solo los menús que el usuario puede ver
                var mMenuHabilitados = _context.MenuMobile
                    .Where(item => item.Activo || _context.MenuMobileUsuariosHabilitados
                        .Any(x => x.Usuario.Id == usuario.Id && x.MenuMobile.Id == item.Id)) // Verifica que el usuario tenga asignado ESTE menú específico
                    .Select(item => new MMenuHabilitados
                    {
                        Nombre = item.Nombre,
                        Codigo = item.Codigo
                    })
                    .ToList();

                request.MenuHabilitados = mMenuHabilitados;
                request.Incobrable = usuario.Incobrable;

                return request;
            }
            catch (Exception e)
            {
                request.UAT = request.UAT;
                request.Status = 500;
                request.Mensaje = e.Message;
                return request;
            }
        }

        [HttpPost("CanjearCupon")]
        public async Task<CanjearCuponDTO> CanjearCupon(CanjearCuponDTO request)
        {
            try
            {
                var usuario = TraeUsuarioUAT(request.UAT);
                if (usuario == null)
                    return new CanjearCuponDTO { Status = 500, UAT = request.UAT, Mensaje = "no existe UAT de Usuario" };

                var cupon = await _context.Premios.Where(x => x.Activo && x.Id == request.CuponId).FirstOrDefaultAsync();
                if (cupon == null)
                {
                    request.Status = 404;
                    request.Mensaje = "El cupón seleccionado no existe.";
                    return request;
                }
                if (cupon.StockActual <= 0)
                {
                    request.Status = 500;
                    request.Mensaje = "El cupón seleccionado ya no tiene stock disponible.";
                    return request;
                }
                if (cupon.FechaVencimiento.Date < DateTime.Now.Date)
                {
                    request.Status = 500;
                    request.Mensaje = "El cupón está expirado.";
                    return request;
                }

                DateTime hoy = DateTime.Now;

                await _obtenerPuntosService.ActualizarLotesVencidos(usuario);

                var lotesDisponibles = await _context.PuntosObtenidosClientes
                    .Where(x => x.Usuario.Id == usuario.Id && x.PuntosDisponibles != 0 && x.FechaVencimiento > hoy)
                    .OrderBy(x => x.FechaVencimiento) 
                    .ToListAsync();

                long totalPuntosUsuario = lotesDisponibles.Sum(x => x.PuntosDisponibles);

                if (cupon.Puntos > totalPuntosUsuario)
                {
                    request.Status = 500;
                    request.Mensaje = "Puntos insuficientes para canjear este cupón.";
                    return request;
                }

                long puntosPorDescontar = cupon.Puntos;

                foreach (var lote in lotesDisponibles)
                {
                    if (puntosPorDescontar <= 0) break;

                    if (lote.PuntosDisponibles >= puntosPorDescontar)
                    {
                        lote.PuntosDisponibles -= puntosPorDescontar;
                        puntosPorDescontar = 0;
                    }
                    else
                    {
                        puntosPorDescontar -= lote.PuntosDisponibles;
                        lote.PuntosDisponibles = 0;
                    }
                }

                var canje = new HistorialCanje()
                {
                    Premio = cupon,
                    Cliente = usuario.Clientes,
                    CodigoCupon = CouponGenerator.GenerarCodigoAzar(),
                    FechaVencimientoCupon = cupon.FechaVencimiento,
                    NroTarjeta = usuario.Personas?.NroTarjeta,
                    PuntosConsumidos = cupon.Puntos,
                    PuntosRestantes = totalPuntosUsuario - cupon.Puntos,
                    Activo = true,
                    Fecha = DateTime.Now,
                };
                _context.HistorialCanje.Add(canje);

                cupon.StockActual = cupon.StockActual - 1;
                _context.Premios.Update(cupon);

                var historialPuntos = new HistorialDePuntos()
                {
                    Cliente = usuario.Clientes,
                    Fecha = DateTime.Now,
                    PuntosObtenidos = -cupon.Puntos,
                    PuntosTotales = totalPuntosUsuario - cupon.Puntos
                };
                _context.HistorialDePuntos.Add(historialPuntos);

                await _context.SaveChangesAsync();

                request.Status = 200;
                request.Mensaje = "Cupón validado con éxito";

                return request;
            }
            catch (DbUpdateException e)
            {
                request.Status = 500;
                request.Mensaje = e.InnerException?.Message ?? e.Message;
                return request;
            }
        }

        [HttpPost("TraeCuponesCliente")]
        public async Task<ListCuponesClienteDTO> TraeCuponesCliente(ListCuponesClienteDTO request)
        {
            try
            {
                var usuario = TraeUsuarioUAT(request.UAT);
                if (usuario == null)
                    return new ListCuponesClienteDTO { Status = 500, UAT = request.UAT, Mensaje = $"no existe UAT de Usuario" };
                var hoy = DateTime.Today;
                var cupones = await _context.HistorialCanje.Where(x => x.Cliente.Id == usuario.Clientes.Id).Select(x => new CuponesClienteDTO()
                {
                    Id = x.Id,
                    Nombre = x.Premio.Nombre,
                    Descripcion = x.Premio.Descripcion,
                    TerminosCondiciones = x.Premio.TerminosCondiciones,
                    Activo = x.Activo,
                    CategoriaNombre = x.Premio.Categoria.Nombre,
                    Codigo = x.CodigoCupon,
                    Fecha = x.Fecha,
                    //DiasRestantesVencimiento = (x.FechaVencimientoCupon.Date - DateTime.Now.Date).Days,
                    DiasRestantesVencimiento = EF.Functions.DateDiffDay(hoy, x.Fecha.AddDays(x.Premio.DiasDeVencimiento)),
                    FechaVencimiento = x.Fecha.AddDays(x.Premio.DiasDeVencimiento),
                    Imagen = _context.FotosPremios.Where(f => f.Premio.Id == x.Premio.Id).Select(f => f.Foto).FirstOrDefault(),
                    Marca = x.Premio.Marcas.Nombre
                }).ToListAsync();

                request.UAT = request.UAT;
                request.Status = 200;
                request.Mensaje = "Exito al traer cupones del Cliente";
                request.Cupones = cupones.OrderBy(x => x.Fecha).ToList();

                return request;
            }
            catch (Exception e)
            {
                request.UAT = request.UAT;
                request.Status = 500;
                request.Mensaje = e.Message;
                return request;
            }
        }

        [HttpPost("TraePuntos")]
        public async Task<PuntosDTO> TraePuntos(PuntosDTO request)
        {
            try
            {
                RelacionPuntos relacionPuntos = _context.RelacionPuntos.FirstOrDefault();
                var usuario = TraeUsuarioUAT(request.UAT);
                if (usuario == null)
                    return new PuntosDTO { Status = 500, UAT = request.UAT, Mensaje = "No existe UAT de Usuario" };

                await _obtenerPuntosService.ObtenerPuntos(usuario);

                await _obtenerPuntosService.ActualizarLotesVencidos(usuario);

                DateTime hoy = DateTime.Now;

                long totalPuntosValidos = await _context.PuntosObtenidosClientes
                    .Where(x => x.Usuario.Id == usuario.Id && x.PuntosDisponibles != 0 && x.FechaVencimiento > hoy)
                    .SumAsync(x => x.PuntosDisponibles);

                // 5. Armar y retornar la respuesta exitosa
                request.Puntos = totalPuntosValidos;
                request.BannerPuntosHome = Convert.ToBase64String(relacionPuntos.Imagen);
                request.Status = 200;
                request.Mensaje = "Puntos Actuales Actualizados";

                return request;
            }
            catch (Exception e)
            {
                request.Status = 500;
                request.Mensaje = e.Message;
                return request;
            }
        }

        [HttpPost("HistorialPuntos")]
        public async Task<HistorialPuntosResponseDto> HistorialPuntos(PuntosDTO request)
        {
            var response = new HistorialPuntosResponseDto();

            try
            {
                var usuario = TraeUsuarioUAT(request.UAT);
                if (usuario == null)
                {
                    response.Status = 500;
                    response.Mensaje = "No existe UAT de Usuario";
                    return response;
                }

                DateTime hoy = DateTime.Now;

                var lotes = await _context.PuntosObtenidosClientes
                    .Where(x => x.Usuario.Id == usuario.Id)
                    .OrderByDescending(x => x.FechaCompra)
                    .ToListAsync();

                foreach (var lote in lotes)
                {
                    long puntosUsados = lote.PuntosObtenidos - lote.PuntosDisponibles;
                    long puntosDisponiblesActivos = 0;
                    long puntosVencidos = 0;

                    if (lote.FechaVencimiento <= hoy)
                    {
                        puntosVencidos = lote.PuntosDisponibles;
                    }
                    else
                    {
                        puntosDisponiblesActivos = lote.PuntosDisponibles;
                    }

                    response.Movimientos.Add(new MovimientoPuntosDto
                    {
                        IdSolicitud = lote.IdSolicitud,
                        IdOperacion = lote.IdOperacion,
                        Compania = lote.Compania,
                        MontoCompra = lote.MontoCompra,
                        FechaCompra = lote.FechaCompra,
                        FechaVencimiento = lote.FechaVencimiento,
                        PuntosObtenidos = lote.PuntosObtenidos,
                        PuntosUsados = puntosUsados,
                        PuntosDisponiblesActivos = puntosDisponiblesActivos,
                        PuntosVencidos = puntosVencidos
                    });
                }

                response.Status = 200;
                response.Mensaje = "Historial obtenido correctamente";
                return response;
            }
            catch (Exception e)
            {
                response.Status = 500;
                response.Mensaje = e.Message;
                return response;
            }
        }










        public static class CouponGenerator
        {
            // Usamos caracteres que no se confundan (sin O, 0, I, 1)
            private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            private static readonly Random _random = new Random();

            public static string GenerarCodigoAzar()
            {
                return $"{GenerarBloque(4)}-{GenerarBloque(4)}";
            }

            private static string GenerarBloque(int longitud)
            {
                char[] bloque = new char[longitud];
                for (int i = 0; i < longitud; i++)
                {
                    bloque[i] = Chars[_random.Next(Chars.Length)];
                }
                return new string(bloque);
            }
        }
    }
}
