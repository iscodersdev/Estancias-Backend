using DAL.Data;
using DAL.Mobile;
using DAL.Models;
using EstanciasCore.Interface;
using EstanciasCore.Services;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EstanciasCore.Worker
{
    public class ResumenTarjetaWorker : IHostedService
    {
        private Timer _timer;
        private bool _seEjecutoEsteMes = false;
        private readonly EstanciasContext _db;
        private readonly IDatosTarjetaService _datosServices;
        private readonly ICompositeViewEngine _viewEngine;

        public ResumenTarjetaWorker(EstanciasContext db, IDatosTarjetaService datosServices, ICompositeViewEngine viewEngine)
        {
            _db = db;
            _datosServices = datosServices;
            _viewEngine = viewEngine;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Worker iniciado.");
            // El timer revisará la fecha cada hora.
            _timer = new Timer(RevisarFechaYEjecutar, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private void RevisarFechaYEjecutar(object state)
        {
            var ahora = DateTime.Now;

            // Si cambiamos de mes, reseteamos el flag.
            if (ahora.Day < 26)
            {
                _seEjecutoEsteMes = false;
            }

            // Si es el día 26 y no lo hemos corrido este mes...
            if (ahora.Day == 26 && !_seEjecutoEsteMes)
            {
                Console.WriteLine($"[{DateTime.Now}] Es día 26. Iniciando generación de resúmenes...");

                // --- AQUÍ VA TU LÓGICA DE GENERACIÓN DE PDF ---
                GenerarResumenesPDF();
                // ---------------------------------------------

                _seEjecutoEsteMes = true; // Marcamos como ejecutado para no repetirlo en el día.
                Console.WriteLine("Generación de resúmenes completada.");
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}] No es día de ejecución. Próxima revisión en 1 hora.");
            }
        }

        private async Task GenerarResumenesPDF()
        {
            List<MovimientoTarjetaDTO> comprasAgrupadas = new List<MovimientoTarjetaDTO>();
            DateTime fechaActual = DateTime.Now;
            DatosEstructura datosEstructura = _db.DatosEstructura.FirstOrDefault();
            Periodo periodo = _db.Periodo.Where(p => p.FechaHasta == fechaActual).FirstOrDefault();
            IQueryable<Usuario> usuarios = _db.Usuarios.Where(x => x.Personas!=null).Where(u => (u.Personas.NroTarjeta != null && u.Personas.NroTarjeta != "") &&  (u.Personas.NroDocumento != null && u.Personas.NroDocumento != ""));


            foreach (var itemUsuario in usuarios)
            {
                var datosMovimientos = await _datosServices.ConsultarMovimientos(datosEstructura.UsernameWS, datosEstructura.PasswordWS, itemUsuario.Personas.NroDocumento, Convert.ToInt64(itemUsuario.Personas.NroTarjeta), 100, 0);
                if (datosMovimientos.Detalle.Resultado=="EXITO")
                {
                    comprasAgrupadas = datosMovimientos.Movimientos.Where(x => x.Descripcion=="PAGOS DE CUOTA REGULAR")
                            .GroupBy(m => new { m.Descripcion, m.Fecha })
                            .Select(g => new MovimientoTarjetaDTO
                            {
                                Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                                TipoMovimiento = g.Key.Descripcion,
                                Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                            })
                            .ToList();

                    comprasAgrupadas.AddRange(datosMovimientos.Movimientos.Where(x => x.Descripcion!="PAGOS DE CUOTA REGULAR")
                    .Select(g => new MovimientoTarjetaDTO
                    {
                        Monto = g.Monto.Replace(",", ".").ToString().Replace(".", ","),
                        TipoMovimiento = g.Descripcion,
                        Fecha = g.Fecha.Date.ToString("dd/MM/yyyy")
                    }).ToList());
                }
            }
            Console.WriteLine("...Simulando creación de PDFs...");
            Thread.Sleep(5000); // Simula 5 segundos de trabajo
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Worker detenido.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
