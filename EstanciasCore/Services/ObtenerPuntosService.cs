using DAL.Data;
using DAL.DTOs.ApiCpeCreditos;
using DAL.DTOs.Reportes;
using DAL.DTOs.Servicios;
using DAL.Mobile;
using DAL.Models;
using EstanciasCore.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;  
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static EstanciasCore.Services.DatosTarjetaService;
namespace EstanciasCore.Services
{
    public class ObtenerPuntosService
    {
        private EstanciasContext _context { get; set; }
        private readonly IDatosTarjetaService _datosTarjetaService;
        private readonly ILogger<ObtenerPuntosService> _logger;
        private readonly HttpClient _httpClient;
        private readonly DateTime FiltroFecha = new DateTime(2026, 7, 1);
        private static readonly (int Id, string Nombre)[] Tiendas = new[]
        {
            (141, "KEVINGSTON FORMOSA"),
            (121, "WANAMA"),
            (92, "PORTA SANTA"),
            (150, "GRISINO"),
            (140, "PENGUIN"),
            (96, "CASA CHRISTIE"),
            (94, "LEGACY PORTAL"),
            (180, "KEVINGSTON CENTRO"),
            (175, "VOLKA"),
            (183, "VOLKA SAENZ PEÑA"),
            (176, "CATIVELLI"),
            (991, "SANTA CARMELA"),
            (992, "MELOCOTON"),
            (4, "ESTANCIAS VICTORIA"),
            (193, "LODS"),
            (146, "SARA REY"),
            (77, "PATO PAMPA"),
            (880, "ASUNCION"),
            (100, "ARIA")
        };

        public ObtenerPuntosService(IConfiguration configuration, EstanciasContext context, IDatosTarjetaService datosTarjetaService, ILogger<ObtenerPuntosService> logger)
        {
            _context=context;
            _datosTarjetaService = datosTarjetaService;
            _logger = logger;
            _httpClient = new HttpClient();
        }


        public async Task ObtenerPuntos(Usuario user)
        {
            RelacionPuntos relacionPuntos = _context.RelacionPuntos.Where(x=>x.Activo==true).FirstOrDefault();

            if (user.Personas == null) return;

            if (user.Personas.PersonaIdCpeCreditos == null)
            {
                var responsePersona = await _datosTarjetaService.ObtenerPersona(user.Personas.NroDocumento);
                if (responsePersona?.Persona != null)
                {
                    user.Personas.PersonaIdCpeCreditos = Convert.ToInt32(responsePersona.Persona.Id);
                }
                else
                {
                    return;
                }
            }

            var ontenerCreditosResponse = await _datosTarjetaService.ObtenerCreditos((int)user.Personas.PersonaIdCpeCreditos);

            if (ontenerCreditosResponse?.Credito != null)
            {
                var comprasRegistradas = _context.PuntosObtenidosClientes
                    .Where(p => p.Usuario.Id == user.Id)
                    .Select(x => x.IdOperacion)
                    .ToList();

                var recorrerCreditos = ontenerCreditosResponse.Credito
                    .Where(c => common.ConvertirFecha(c.Fecha) >= FiltroFecha && !comprasRegistradas.Contains(c.Operacion))
                    .ToList();

                var insertarPuntos = new List<PuntosObtenidosClientes>();

                foreach (var c in recorrerCreditos)
                {
                    DateTime fechaCompra = common.ConvertirFecha(c.Fecha);
                    decimal monto = Convert.ToDecimal(c.CapitalPedido);

                    var responseCompania = await _datosTarjetaService.ObtenerOperacionDetalles(c.Operacion);

                    int companiaId = responseCompania != null ? Convert.ToInt32(responseCompania.CodigoCompania) : 0;

                    // SI EL ID ESTÁ EN EL ARRAY, NO TIENE QUE SUMAR PUNTOS (Sigue de largo)
                    if (Tiendas.Any(t => t.Id == companiaId))
                    {
                        _logger.LogInformation($"Compañía {companiaId} excluida. No suma puntos.");
                        continue; // Salta al siguiente crédito sin hacer el .Add
                    }

                    // Si NO está en el array, pasa el filtro y se procesa normalmente:
                    var nuevoPuntoCliente = new PuntosObtenidosClientes
                    {
                        Usuario = user,
                        IdSolicitud = c.IdSolicitud,
                        IdOperacion = c.Operacion,
                        MontoCompra = monto,
                        FechaCompra = fechaCompra,
                        Compania = responseCompania?.Compania ?? "Desconocida",
                        CompaniaId = companiaId,
                        PuntosObtenidos = CalcularPuntos(monto, relacionPuntos),
                        PuntosDisponibles = CalcularPuntosVencidos(monto, relacionPuntos),
                        FechaVencimiento = CalcularFechaVencimiento(fechaCompra),
                        FechaProcesada = DateTime.Now
                    };

                    insertarPuntos.Add(nuevoPuntoCliente);
                }

                if (insertarPuntos.Any())
                {
                    _context.PuntosObtenidosClientes.AddRange(insertarPuntos);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task ActualizarLotesVencidos(Usuario user)
        {
            DateTime hoy = DateTime.Now;

            // 1. Expirar los lotes positivos que ya vencieron
            var lotesExpirados = await _context.PuntosObtenidosClientes
                .Where(x => x.Usuario.Id == user.Id && x.FechaVencimiento <= hoy && x.PuntosDisponibles > 0)
                .ToListAsync();

            if (lotesExpirados.Any())
            {
                foreach (var lote in lotesExpirados)
                {
                    lote.PuntosDisponibles = 0;
                }

                await _context.SaveChangesAsync();
            }

            // 2. Revisar si la suma total de puntos (positivos + negativos) da 0 o negativo
            var lotesActivos = await _context.PuntosObtenidosClientes
                .Where(x => x.Usuario.Id == user.Id && x.PuntosDisponibles != 0)
                .ToListAsync();

            if (lotesActivos.Any())
            {
                long saldoTotal = lotesActivos.Sum(x => x.PuntosDisponibles);

                // Si el saldo es 0 o negativo, limpiamos absolutamente todos los lotes para "perdonar" la deuda
                // y empezar de cero, tal como define la regla de negocio.
                if (saldoTotal <= 0)
                {
                    foreach (var lote in lotesActivos)
                    {
                        lote.PuntosDisponibles = 0;
                    }
                    await _context.SaveChangesAsync();
                }
            }
        }


        private int CalcularPuntos(decimal monto, RelacionPuntos relacionPuntos)
        {
            if (monto <= 0) return 0;

            decimal montoPorPunto = relacionPuntos.Monto;

            return (int)Math.Floor(monto / montoPorPunto);
        }

        private int CalcularPuntosVencidos(decimal monto, RelacionPuntos relacionPuntos)
        {
            return CalcularPuntos(monto, relacionPuntos);
        }

        private DateTime CalcularFechaVencimiento(DateTime fechaCompra)
        {
            DateTime fechaConAnios = fechaCompra.AddYears(2);
            DateTime vencimiento = new DateTime(fechaConAnios.Year, fechaConAnios.Month, 1).AddMonths(1);
            return vencimiento;
        }

    }
}