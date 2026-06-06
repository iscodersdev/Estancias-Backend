using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using EstanciasCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Endpoints
{
    [Area("Core")]
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [Route("endpoint/puntos-obtenidos-clientes")]
    public class PuntosObtenidosClientesEndpointController : Controller
    {
        private readonly EstanciasContext _context;
        private readonly ObtenerPuntosService _obtenerPuntosService;

        public PuntosObtenidosClientesEndpointController(
            EstanciasContext context,
            ObtenerPuntosService obtenerPuntosService)
        {
            _context = context;
            _obtenerPuntosService = obtenerPuntosService;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar(string searchTerm)
        {
            var query = _context.PuntosObtenidosClientes
                .Include(p => p.Usuario)
                    .ThenInclude(u => u.Personas)
                .AsQueryable();

            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new
                {
                    Items = new PuntosObtenidosClientesDTO[] { },
                    TotalRegistros = 0,
                    SearchTerm = searchTerm
                });
            }

            searchTerm = searchTerm.Trim().ToLower();

            var usuario = await _context.Users
                .Include(u => u.Personas)
                .FirstOrDefaultAsync(u =>
                    (u.UserName != null && u.UserName.ToLower().Contains(searchTerm)) ||
                    (u.Personas != null && u.Personas.NroDocumento != null && u.Personas.NroDocumento.Contains(searchTerm)) ||
                    (u.Personas != null && u.Personas.NroTarjeta != null && u.Personas.NroTarjeta.Contains(searchTerm)));

            if (usuario != null)
            {
                await _obtenerPuntosService.ObtenerPuntos(usuario);
                await _obtenerPuntosService.ActualizarLotesVencidos(usuario);
            }

            query = query.Where(p =>
                (p.Usuario.UserName != null && p.Usuario.UserName.ToLower().Contains(searchTerm)) ||
                (p.Usuario.Personas != null && p.Usuario.Personas.NroDocumento != null && p.Usuario.Personas.NroDocumento.Contains(searchTerm)) ||
                (p.Usuario.Personas != null && p.Usuario.Personas.NroTarjeta != null && p.Usuario.Personas.NroTarjeta.Contains(searchTerm)));

            var resultados = await query
                .OrderByDescending(p => p.FechaCompra)
                .Select(p => new PuntosObtenidosClientesDTO
                {
                    Id = p.Id,

                    UsuarioId = p.Usuario != null ? p.Usuario.Id : null,
                    UserName = p.Usuario != null ? p.Usuario.UserName : null,

                    NroDocumento = p.Usuario != null && p.Usuario.Personas != null
                        ? p.Usuario.Personas.NroDocumento
                        : null,

                    NroTarjeta = p.Usuario != null && p.Usuario.Personas != null
                        ? p.Usuario.Personas.NroTarjeta
                        : null,

                    IdSolicitud = p.IdSolicitud,
                    IdOperacion = p.IdOperacion,

                    MontoCompra = p.MontoCompra,
                    FechaCompra = p.FechaCompra,

                    Compania = p.Compania,
                    CompaniaId = p.CompaniaId,

                    PuntosObtenidos = p.PuntosObtenidos,
                    PuntosDisponibles = p.PuntosDisponibles,

                    FechaVencimiento = p.FechaVencimiento,
                    FechaProcesada = p.FechaProcesada
                })
                .ToListAsync();

            return Json(new
            {
                Items = resultados,
                TotalRegistros = resultados.Count,
                SearchTerm = searchTerm
            });
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] CrearPuntosObtenidosClientesDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "No se recibieron datos."
                });
            }

            if (string.IsNullOrEmpty(dto.UsuarioId))
            {
                return Json(new
                {
                    Success = false,
                    Message = "Debe indicar un usuario.",
                    SearchTerm = dto.SearchTerm
                });
            }

            var user = await _context.Users.FindAsync(dto.UsuarioId);

            if (user == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "No se encontró el usuario indicado.",
                    SearchTerm = dto.SearchTerm
                });
            }

            var nuevoPunto = new PuntosObtenidosClientes
            {
                Usuario = user,

                MontoCompra = dto.MontoCompra,
                PuntosObtenidos = dto.PuntosObtenidos,
                PuntosDisponibles = dto.PuntosObtenidos,

                IdOperacion = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),

                Compania = string.IsNullOrEmpty(dto.Motivo)
                    ? "Carga Manual"
                    : dto.Motivo,

                CompaniaId = 0,

                FechaCompra = DateTime.Now,
                FechaProcesada = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddYears(1)
            };

            _context.PuntosObtenidosClientes.Add(nuevoPunto);
            await _context.SaveChangesAsync();

            return Json(new
            {
                Success = true,
                Message = "Puntos cargados correctamente.",
                Id = nuevoPunto.Id,
                SearchTerm = dto.SearchTerm
            });
        }
    }
}