using DAL.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using EstanciasCore.Services;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class PuntosObtenidosClientesController : Controller
    {
        private readonly EstanciasContext _context;
        private readonly ObtenerPuntosService _obtenerPuntosService;

        public PuntosObtenidosClientesController(EstanciasContext context, ObtenerPuntosService obtenerPuntosService)
        {
            _context = context;
            _obtenerPuntosService = obtenerPuntosService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewBag.SearchTerm = searchTerm;

            var query = _context.PuntosObtenidosClientes
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Personas)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();

                // Actualizar puntos del usuario si lo encontramos antes de la consulta
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
                    
                var resultados = await query.OrderByDescending(p => p.FechaCompra).ToListAsync();
                return View(resultados);
            }

            return View(new List<DAL.Models.PuntosObtenidosClientes>());
        }

        [HttpPost]
        public async Task<IActionResult> Create(string UsuarioId, decimal MontoCompra, long PuntosObtenidos, string Motivo, string SearchTerm)
        {
            if (string.IsNullOrEmpty(UsuarioId))
            {
                return RedirectToAction(nameof(Index), new { searchTerm = SearchTerm });
            }

            var nuevoPunto = new DAL.Models.PuntosObtenidosClientes
            {
                // Set the manually entered values
                MontoCompra = MontoCompra,
                PuntosObtenidos = PuntosObtenidos,
                PuntosDisponibles = PuntosObtenidos, // By default they start as fully available
                
                // Generated auto-values
                IdOperacion = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(), // Mock operation ID
                Compania = string.IsNullOrEmpty(Motivo) ? "Carga Manual" : Motivo,
                CompaniaId = 0,
                FechaCompra = DateTime.Now,
                FechaProcesada = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddYears(1) // 1 year validity
            };

            // Assuming UsuarioId is string in AspNetUsers, but we need to check how the relationship is mapped
            // Usually we can just set the foreign key if it exists, or fetch the user and assign the navigation property
            var user = await _context.Users.FindAsync(UsuarioId);
            if (user != null)
            {
                nuevoPunto.Usuario = user;
                _context.PuntosObtenidosClientes.Add(nuevoPunto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { searchTerm = SearchTerm });
        }
    }
}
