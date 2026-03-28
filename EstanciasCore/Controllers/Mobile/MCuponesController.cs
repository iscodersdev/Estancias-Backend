using DAL.Data;
using DAL.DTOs;
using DAL.DTOs.API;
using DAL.DTOs.Reportes;
using DAL.DTOs.Servicios;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.API.Filters;
using EstanciasCore.Interface;
using EstanciasCore.Services;
using iText.Html2pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMailService _mailService;

        public MCuponesController(EstanciasContext context, MercadoPagoServices mp, IDatosTarjetaService datosServices, IHostingEnvironment webHostEnvironment, IServiceScopeFactory scopeFactory, ILogger<MTarjetasController> logger, IConfiguration configuration, ICompositeViewEngine viewEngine, IServiceProvider serviceProvider, IMailService mailService) : base(context)
        {
            _datosServices = datosServices;
            _mp = mp;
            _webHostEnvironment = webHostEnvironment;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
            _viewEngine=viewEngine;
            _serviceProvider=serviceProvider;
            _mailService = mailService;
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
                    Fecha = x.Fecha,
                    Imagen = _context.FotosPremios.Where(f => f.Premio.Id == x.Id).Select(f => f.Foto).FirstOrDefault()
                }).ToListAsync();

                request.UAT = request.UAT;
                request.Status = 200;
                request.Mensaje = "Exito al traer cupones";
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

        [HttpPost("CanjearCupon")]
        public async Task<CanjearCuponDTO> CanjearCupon(CanjearCuponDTO request)
        {
            try
            {
                var usuario = TraeUsuarioUAT(request.UAT);
                if (usuario == null)
                    return new CanjearCuponDTO { Status = 500, UAT = request.UAT, Mensaje = $"no existe UAT de Usuario" };

                var puntosCliente = _context.PuntosClientes.Where(x => x.Cliente.Id == usuario.Clientes.Id).FirstOrDefault();
                var cupon = await _context.Premios.Where(x => x.Activo && x.Id==request.CuponId).FirstOrDefaultAsync();
                if (cupon.StockActual<=0)
                {
                    request.Status = 500;
                    request.Mensaje = "El cupón seleccionado ya no tiene stock disponible.";
                    return request;
                }
                if (cupon.FechaVencimiento.Date<DateTime.Now.Date)
                {
                    request.Status = 500;
                    request.Mensaje = "El cupón está expirado.";
                    return request;
                }
                if (cupon.Activo==false)
                {
                    request.Status = 500;
                    request.Mensaje = "El cupón no se encuentra activo.";
                    return request;
                }
                if (cupon.Puntos>puntosCliente.Puntos)
                {
                    request.Status = 500;
                    request.Mensaje = "Puntos insuficientes para canjear este cupón.";
                    return request;
                }

                var canje = new HistorialCanje()
                {
                    Premio = cupon,
                    Cliente = usuario.Clientes,
                    CodigoCupon = CouponGenerator.GenerarCodigoAzar(),
                    FechaVencimientoCupon = cupon.FechaVencimiento,
                    NroTarjeta = usuario.Personas.NroTarjeta,
                    PuntosConsumidos = cupon.Puntos,
                    PuntosRestantes = puntosCliente.Puntos - cupon.Puntos,
                    Activo = true,
                    Fecha = DateTime.Now,
                };
                _context.HistorialCanje.Add(canje);
                cupon.StockActual = cupon.StockActual - 1;
                puntosCliente.Puntos = puntosCliente.Puntos - cupon.Puntos;
                _context.PuntosClientes.Update(puntosCliente);
                _context.Premios.Update(cupon);

                var historialPuntos = new HistorialDePuntos()
                {
                    Cliente = usuario.Clientes,
                    Fecha = DateTime.Now,
                    PuntosObtenidos = -cupon.Puntos,
                    PuntosTotales = puntosCliente.Puntos
                };
                _context.HistorialDePuntos.Add(historialPuntos);
                await _context.SaveChangesAsync();                

                request.UAT = request.UAT;
                request.Status = 200;
                request.Mensaje = "Cupón validado con éxito";

                return request;
            }
            catch (DbUpdateException e)
            {
                request.UAT = request.UAT;
                request.Status = 500;
                request.Mensaje = e.InnerException?.Message;
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

                var cupones = await _context.HistorialCanje.Select(x => new CuponesClienteDTO()
                {
                    Id = x.Id,
                    Nombre = x.Premio.Nombre,
                    Descripcion = x.Premio.Descripcion,
                    Activo = x.Activo,
                    CategoriaNombre = x.Premio.Categoria.Nombre,
                    Codigo = x.CodigoCupon,
                    Fecha = x.Fecha,
                    DiasRestantesVencimiento = (x.FechaVencimientoCupon.Date - DateTime.Now.Date).Days,
                    FechaVencimiento = x.FechaVencimientoCupon,
                    Imagen = _context.FotosPremios.Where(f => f.Premio.Id == x.Premio.Id).Select(f => f.Foto).FirstOrDefault()
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
                var usuario = TraeUsuarioUAT(request.UAT);
                if (usuario == null)
                    return new PuntosDTO { Status = 500, UAT = request.UAT, Mensaje = $"no existe UAT de Usuario" };

                var puntosCliente = await _context.PuntosClientes.Where(x => x.Cliente.Id == usuario.Clientes.Id).FirstOrDefaultAsync();

                request.Puntos = puntosCliente!=null?puntosCliente.Puntos:0;
                request.UAT = request.UAT;
                request.Status = 200;
                request.Mensaje = "Puntos Actuales";

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
