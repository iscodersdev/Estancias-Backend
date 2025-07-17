using DAL.Data;
using DAL.DTOs;
using DAL.DTOs.API;
using DAL.Mobile;
using DAL.Models;
using DAL.Models.Core;
using EstanciasCore.API.Filters;
using EstanciasCore.Areas.Administracion.ViewModels;
using EstanciasCore.Interface;
using EstanciasCore.Services;
using Google.Protobuf.WellKnownTypes;
using iText.Html2pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Org.BouncyCastle.Ocsp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static EstanciasCore.Services.MercadoPagoServices;
using PagoTarjetaDTO = DAL.Mobile.PagoTarjetaDTO;

namespace EstanciasCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class MTarjetasController : BaseApiController
    {
        private readonly MercadoPagoServices _mp;
        private readonly IDatosTarjetaService _datosServices;

        public MTarjetasController(EstanciasContext context, MercadoPagoServices mp, IDatosTarjetaService datosServices) : base(context)
        {
            _datosServices = datosServices;
            _mp = mp;
        }

        [HttpPost("Alta")]
        public async Task<IActionResult> Alta([FromBody] DAL.DTOs.API.AltaTarjetaDTO altaTarjetaDTO)
        {
            try
            {
                var usuario = TraeUsuarioUAT(altaTarjetaDTO.UAT);
                var billetera = _context.Billeteras.Where(b => b.Cliente.Usuario.Id == usuario.Id).FirstOrDefault();
                var tarjeta = new Tarjeta { Titular = altaTarjetaDTO.Titular, Numero = altaTarjetaDTO.Numero, Vencimiento = altaTarjetaDTO.Vencimiento };
                billetera.Tarjetas.Add(tarjeta);
                _context.Update(billetera);
                await _context.SaveChangesAsync();

                return new JsonResult(new RespuestaAPI { Status = 200, UAT = altaTarjetaDTO.UAT, Mensaje = "Tarjeta creada con exito" });
            }
            catch (Exception e)
            {
                Log.Error($"Error en creacion de tarjeta - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = altaTarjetaDTO.UAT, Mensaje = $"Error en creacion de terjeta" });
            }

        }

        [HttpPost("MovimientoTarjeta")]
        public IActionResult MovimientoTarjeta([FromBody] ListaMovimientoTarjetaDTO movimientostarjetaDTOS)
        {
            try
            {
                var usuario = TraeUsuarioUAT(movimientostarjetaDTOS.UAT);
                if (usuario == null)
                    return new JsonResult(new RespuestaAPI { Status = 500, UAT = movimientostarjetaDTOS.UAT, Mensaje = $"no existe UAT de Usuario" });

                var procedimiento = _context.Procedimientos.Where(x => x.Codigo=="SynchronizeMovementIndividual").FirstOrDefault();
                if (procedimiento.Activo)
                {
                    _datosServices.ActualizarMovimientosAsync(usuario);
                }

                DatosEstructura empresa = _context.DatosEstructura.FirstOrDefault();
                var tipomov = _context.TipoMovimientoTarjeta.Where(x => x.Id == movimientostarjetaDTOS.tipomovimiento).FirstOrDefault();

                var movimientos = _datosServices.ConsultarMovimientos(empresa.UsernameWS, empresa.PasswordWS, usuario.Personas.NroDocumento, movimientostarjetaDTOS.NroTarjeta, movimientostarjetaDTOS.CantMovimientos, movimientostarjetaDTOS.tipomovimiento).Result;

                if (movimientos.Detalle.Resultado == "EXITO")
                {
                    List<MovimientoTarjetaDTO> comprasAgrupadas = new List<MovimientoTarjetaDTO>();
                    if (movimientostarjetaDTOS.tipomovimiento == 0) //Todos los movimientos
                    {
                        comprasAgrupadas = movimientos.Movimientos.Where(x => x.Descripcion=="PAGOS DE CUOTA REGULAR")
                        .GroupBy(m => new { m.Descripcion, m.Fecha })
                        .Select(g => new MovimientoTarjetaDTO
                        {
                            Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                            TipoMovimiento = g.Key.Descripcion,
                            Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                        })
                        .ToList();

                        comprasAgrupadas.AddRange(movimientos.Movimientos.Where(x => x.Descripcion!="PAGOS DE CUOTA REGULAR")
                        .Select(g => new MovimientoTarjetaDTO
                        {
                            Monto = g.Monto.Replace(",", ".").ToString().Replace(".", ","),
                            TipoMovimiento = g.Descripcion,
                            Fecha = g.Fecha.Date.ToString("dd/MM/yyyy")
                        }).ToList());
                    }
                    else
                    {
                        comprasAgrupadas = movimientos.Movimientos.Where(x => x.Descripcion == tipomov.Nombre)
                       .GroupBy(m => new { m.Descripcion, m.Fecha })
                       .Select(g => new MovimientoTarjetaDTO
                       {
                           Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                           TipoMovimiento = g.Key.Descripcion,
                           Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                       })
                       .ToList();
                    }


                    List<ListDetalleCuotaDTO> movimientosDetalles = new List<ListDetalleCuotaDTO>();

                    //var movi = movimientos.DetallesSolicitud.Where(x=>x.DetallesCuota.Where(e=>e.Fecha))

                    foreach (var item in movimientos.DetallesSolicitud)
                    {
                        foreach (var itemMovimiento in item.DetallesCuota)
                        {
                            if (movimientosDetalles.Any(x => x.Fecha == itemMovimiento.Fecha))
                            {
                                var detalle = movimientosDetalles.Where(x => x.Fecha == itemMovimiento.Fecha).First();
                                detalle.Monto = (Convert.ToDecimal(detalle.Monto) + Convert.ToDecimal(itemMovimiento.Monto)).ToString();
                            }
                            else
                            {
                                movimientosDetalles.Add(new ListDetalleCuotaDTO()
                                {
                                    Fecha = itemMovimiento.Fecha,
                                    Monto = itemMovimiento.Monto
                                });
                            }
                        }
                    }

                    string saldoVencidoAcumulado = "0.0";
                    string sumaProximoVencimientoAcumulado = "0.0";
                    bool cuotaVencida = false;
                    string fechaSiguienteCuota = "";
                    DateTime? fechaProximoPago = null;

                    foreach (var item in movimientosDetalles.OrderBy(x => ConvertirFecha(x.Fecha)))
                    {

                        if (VerificarVencimiento(item.Fecha))
                        {
                            SetearCultureInfoUS();
                            var aux1 = Convert.ToDecimal(item.Monto);
                            var aux2 = Convert.ToDecimal(saldoVencidoAcumulado);
                            saldoVencidoAcumulado = (aux1+aux2).ToString();
                            cuotaVencida = true;
                        }
                        else
                        {
                            if (fechaSiguienteCuota=="")
                                fechaSiguienteCuota = item.Fecha;

                            if ((ConvertirFecha(item.Fecha).Month==(ConvertirFecha(fechaSiguienteCuota).Month)) && (ConvertirFecha(item.Fecha).Year==ConvertirFecha(fechaSiguienteCuota).Year))
                            {
                                if (fechaProximoPago==null)
                                    fechaProximoPago=ConvertirFecha(item.Fecha);

                                SetearCultureInfoUS();
                                var monto2 = Convert.ToDecimal(item.Monto);
                                var monto1 = Convert.ToDecimal(sumaProximoVencimientoAcumulado);
                                sumaProximoVencimientoAcumulado = (monto1+monto2).ToString();
                            }
                        }

                    }
                    SetearCultureInfoUS();
                    sumaProximoVencimientoAcumulado = (Convert.ToDecimal(sumaProximoVencimientoAcumulado)+Convert.ToDecimal(saldoVencidoAcumulado)).ToString();

                    bool ContieneLeyenda = false;
                    string Leyenda = "";
                    var LeyendaTexto = _context.LeyendaTipoMovimiento.FirstOrDefault();

                    if (comprasAgrupadas.Where(x => x.TipoMovimiento == LeyendaTexto.NombreMovimiento).Count()>0 && LeyendaTexto.Activo==true)
                    {
                        ContieneLeyenda = true;
                        Leyenda = LeyendaTexto.TextoLeyenda;
                    }


                    List<MovimientoTarjetaDTO> MovimientosTarjeta = movimientos.Movimientos
                        .Select(mov => new MovimientoTarjetaDTO
                        {
                            TipoMovimiento = mov.Descripcion,
                            Monto = mov.Monto,
                            Fecha = mov.Fecha.ToString("dd/MM/yyyy")
                        })
                        .ToList();

                    int cantidadDeMovimientos = MovimientosTarjeta.Count();
                    if (cantidadDeMovimientos>movimientostarjetaDTOS.CantMovimientos)
                    {
                        cantidadDeMovimientos = Convert.ToInt32(movimientostarjetaDTOS.CantMovimientos);
                    }


                    if (movimientostarjetaDTOS.NroTarjeta.ToString().TrimStart('0')=="500033395")
                    {
                        saldoVencidoAcumulado = "0.0";
                    }

                    var MovientosOrdenadosPorFecha = comprasAgrupadas.OrderByDescending(x => ConvertirFecha(x.Fecha)).Take(Convert.ToInt32(movimientostarjetaDTOS.CantMovimientos)).ToList();

                    return new JsonResult(
                        new ListaMovimientoTarjetaDTO
                        {
                            Status = 200,
                            UAT = movimientostarjetaDTOS.UAT,
                            Mensaje = "Movimiento obtenidos",
                            NroTarjeta = movimientostarjetaDTOS.NroTarjeta,
                            NroDocumento = Convert.ToInt32(usuario.Personas.NroDocumento),
                            Direccion = movimientos.Detalle.Direccion,
                            MontoAdeudado = saldoVencidoAcumulado.Replace(".", ","),
                            ProximaFechaPago = fechaProximoPago?.ToString("dd/MM/yyyy"),
                            CuotaVencida = cuotaVencida,
                            TotalProximaCuota = sumaProximoVencimientoAcumulado.Replace(".", ","),
                            CantMovimientos = movimientostarjetaDTOS.CantMovimientos,
                            Resultado = movimientos.Detalle.Resultado,
                            Nombre = movimientos.Detalle.Nombre,
                            FechaPagoProximaCuota = fechaProximoPago?.ToString("dd/MM/yyyy"),
                            MovimientosTarjeta = MovientosOrdenadosPorFecha,
                            MontoDisponible = movimientos.Detalle.MontoDisponible.Replace(".", ","),
                            ContieneLeyenda = ContieneLeyenda,
                            Leyenda = Leyenda,
                            Telefono = empresa.Telefono
                        });
                }
                else
                    return new JsonResult(new ListaMovimientoTarjetaDTO { Status = 500, UAT = movimientostarjetaDTOS.UAT, Mensaje = "No existe datos de la tarjeta" });

            }
            catch (Exception e)
            {
                Log.Error($"Error en creacion de tarjeta - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = movimientostarjetaDTOS.UAT, Mensaje = $"Error en creacion de tarjeta" + e });
            }

        }


        [HttpPost("MovimientoTarjetaNew")]
        public IActionResult MovimientoTarjetaNew([FromBody] ListaMovimientoTarjetaDTO movimientostarjetaDTOS)
        {
            try
            {
                var usuario = TraeUsuarioUAT(movimientostarjetaDTOS.UAT);
                if (usuario == null)
                    return new JsonResult(new RespuestaAPI { Status = 500, UAT = movimientostarjetaDTOS.UAT, Mensaje = $"no existe UAT de Usuario" });

                //var procedimiento = _context.Procedimientos.Where(x => x.Codigo=="SynchronizeMovementIndividual").FirstOrDefault();
                //if (procedimiento.Activo)
                //{
                //    _datosServices.ActualizarMovimientosAsync(usuario);
                //}

                DatosEstructura empresa = _context.DatosEstructura.FirstOrDefault();
                var tipomov = _context.TipoMovimientoTarjeta.Where(x => x.Id == movimientostarjetaDTOS.tipomovimiento).FirstOrDefault();

                // var movimientos = _datosServices.ConsultarMovimientos(empresa.UsernameWS, empresa.PasswordWS, usuario.Personas.NroDocumento, movimientostarjetaDTOS.NroTarjeta, movimientostarjetaDTOS.CantMovimientos, movimientostarjetaDTOS.tipomovimiento).Result;
                var movimientos = _datosServices.LoginApiLoanAsync(empresa.UsernameWS, empresa.PasswordWS);

                
                return new JsonResult(new ListaMovimientoTarjetaDTO { Status = 500, UAT = movimientostarjetaDTOS.UAT, Mensaje = "No existe datos de la tarjeta" });

            }
            catch (Exception e)
            {
                Log.Error($"Error en creacion de tarjeta - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = movimientostarjetaDTOS.UAT, Mensaje = $"Error en creacion de tarjeta" + e });
            }

        }

        [HttpPost("TraePeriodos")]
        public async Task<IActionResult> TraePeriodos(TraePeriodosDTO body)
        {
            TraePeriodosDTO traePeriodosDTO = new TraePeriodosDTO() { UAT = body.UAT };

            var usuario = TraeUsuarioUAT(body.UAT);
            if (usuario == null)
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = traePeriodosDTO.UAT, Mensaje = $"no existe UAT de Usuario" });

            var periodos = await _context.Periodo.Select(x=> new PeriodoDTO() { FechaDesde = x.FechaDesde, FechaHasta = x.FechaHasta, Id = x.Id, Nombre = x.Descripcion }).ToListAsync();
            if (periodos == null || !periodos.Any())
            {
                return NotFound("No se encontraron periodos.");
            }
            traePeriodosDTO.Periodos = periodos.OrderByDescending(x => x.FechaDesde).ToList();
            traePeriodosDTO.Status = 200;
            traePeriodosDTO.Mensaje = "Periodos obtenidos con exito";
            return new JsonResult(traePeriodosDTO);
        }

        [HttpPost("DescargarResumen")]
        public async Task<IActionResult> DescargarResumen(MovimientosTarjetaDTO body)
        {
            var usuario = TraeUsuarioUAT(body.UAT);
            if (usuario == null)
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = body.UAT, Mensaje = $"no existe UAT de Usuario" });

            var datosParaElResumen = await _datosServices.PrepararDatosDTO(body.PeriodoId, usuario.Id);
            if (datosParaElResumen == null)
            {
                return NotFound(new { error = "No se encontraron datos para generar el resumen." });
            }

            string html = await _datosServices.RenderViewToStringAsync("ResumenBancarioTemplate", datosParaElResumen);
            byte[] pdfBytes;
            using (var memoryStream = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(html, memoryStream);
                pdfBytes = memoryStream.ToArray();
            }

            // --- 3. Devuelve un objeto JSON con los datos del archivo ---
            string nombreArchivo = $"Resumen_{datosParaElResumen.Periodo.Descripcion.Replace(" ", "_")}.pdf";

            return Ok(new
            {
                FileName = nombreArchivo,
                MimeType = "application/pdf",
                FileContents = pdfBytes // Esto se enviará como un string Base64
            });
        }

        [HttpPost("SolicitarPagoTarjeta")]
        public IActionResult SolicitarPagoTarjeta([FromBody] PagoTarjetaDTO pagotarjetaDTO)
        {
            try
            {
                ListaMovimientoTarjetaDTO movimientostarjetaDTOS = new ListaMovimientoTarjetaDTO();
                var usuario = TraeUsuarioUAT(pagotarjetaDTO.UAT);
                if (usuario == null)
                    return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"no existe UAT de Usuario" });

                DatosEstructura empresa = _context.DatosEstructura.FirstOrDefault();
                var movtarj = new MovPersona();

                var tipomov = _context.TipoMovimientoTarjeta.FirstOrDefault();
                var nuevosMovimientos = movtarj.ConsultarMovimientosTarjetas2(empresa.UsernameWS, empresa.PasswordWS, usuario.Personas.NroDocumento, Convert.ToInt64(pagotarjetaDTO.NroTarjeta), 100, 0);                var pers = _context.Personas.Where(x => x.NroTarjeta == usuario.Personas.NroTarjeta).FirstOrDefault();


                if (nuevosMovimientos.Detalle.Resultado == "EXITO")
                {
                    List<MovimientoTarjetaDTO> resultadoNuevos = new List<MovimientoTarjetaDTO>();
                    if (movimientostarjetaDTOS.tipomovimiento == 0)
                    {
                        resultadoNuevos = nuevosMovimientos.Movimientos
                        .GroupBy(m => new { m.Descripcion, m.Fecha })
                        .Select(g => new MovimientoTarjetaDTO
                        {
                            Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                            TipoMovimiento = g.Key.Descripcion,
                            Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                        })
                        .ToList();
                    }
                    else
                    {
                        resultadoNuevos = nuevosMovimientos.Movimientos.Where(x => x.Descripcion == tipomov.Nombre)
                        .GroupBy(m => new { m.Descripcion, m.Fecha })
                        .Select(g => new MovimientoTarjetaDTO
                        {
                            Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                            TipoMovimiento = g.Key.Descripcion,
                            Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                        })
                        .ToList();
                    }

                    List<ListDetalleCuotaDTO> listDetalle = new List<ListDetalleCuotaDTO>();

                    foreach (var item in nuevosMovimientos.DetallesSolicitud)
                    {
                        foreach (var itemMovimiento in item.DetallesCuota)
                        {
                            if (listDetalle.Any(x => x.Fecha == itemMovimiento.Fecha))
                            {
                                var detalle = listDetalle.Where(x => x.Fecha == itemMovimiento.Fecha).First();
                                detalle.Monto = (Convert.ToDecimal(detalle.Monto) + Convert.ToDecimal(itemMovimiento.Monto)).ToString();
                            }
                            else
                            {
                                listDetalle.Add(new ListDetalleCuotaDTO()
                                {
                                    Fecha = itemMovimiento.Fecha,
                                    Monto = itemMovimiento.Monto
                                });
                            }
                        }
                    }

                    string deudaTotal = "0.0";
                    string saldoVencidoAcumulado = "0.0";
                    string sumaProximoVencimientoAcumulado = "0.0";
                    string sumaTotalAcumulado = "0.0";
                    bool cuotaVencida = false;
                    string fechaSiguienteCuota = "";
                    DateTime? fechaProximoPago = null;
                    foreach (var item in listDetalle.OrderBy(x => x.Fecha))
                    {
                        if (VerificarVencimiento(item.Fecha))
                        {
                            SetearCultureInfoUS();
                            var aux1 = Convert.ToDecimal(item.Monto);
                            var aux2 = Convert.ToDecimal(saldoVencidoAcumulado);
                            saldoVencidoAcumulado = (aux1+aux2).ToString();
                            cuotaVencida = true;
                        }
                        else
                        {
                            
                            if (fechaSiguienteCuota=="")
                                fechaSiguienteCuota = item.Fecha;

                            if ((ConvertirFecha(item.Fecha).Month==(ConvertirFecha(fechaSiguienteCuota).Month)) && (ConvertirFecha(item.Fecha).Year==ConvertirFecha(fechaSiguienteCuota).Year))
                            {
                                if (fechaProximoPago==null)
                                    fechaProximoPago=ConvertirFecha(item.Fecha);

                                SetearCultureInfoUS();
                                var monto2 = Convert.ToDecimal(item.Monto);
                                var monto1 = Convert.ToDecimal(sumaProximoVencimientoAcumulado);
                                sumaProximoVencimientoAcumulado = (monto1+monto2).ToString();
                            }
                            SetearCultureInfoUS();
                            var monto3 = Convert.ToDecimal(item.Monto);
                            var monto4 = Convert.ToDecimal(deudaTotal);
                            deudaTotal = (monto3+monto4).ToString();
                        }

                    }
                    SetearCultureInfoUS();
                    sumaProximoVencimientoAcumulado = (Convert.ToDecimal(sumaProximoVencimientoAcumulado)+Convert.ToDecimal(saldoVencidoAcumulado)).ToString();
                    sumaTotalAcumulado = (Convert.ToDecimal(deudaTotal)+Convert.ToDecimal(saldoVencidoAcumulado)).ToString();

                    bool ContieneLeyenda = false;
                    string Leyenda = "";
                    var LeyendaTexto = _context.LeyendaTipoMovimiento.FirstOrDefault();

                    if (resultadoNuevos.Where(x => x.TipoMovimiento == LeyendaTexto.NombreMovimiento).Count()>0 && LeyendaTexto.Activo==true)
                    {
                        ContieneLeyenda = true;
                        Leyenda = LeyendaTexto.TextoLeyenda;
                    }

                    List<MovimientoTarjetaDTO> MovimientosTarjeta = nuevosMovimientos.Movimientos
                        .Select(mov => new MovimientoTarjetaDTO
                        {
                            TipoMovimiento = mov.Descripcion,
                            Monto = mov.Monto,
                            Fecha = mov.Fecha.ToString("dd/MM/yyyy")
                        })
                        .ToList();

                        int cantidadDeMovimientos = MovimientosTarjeta.Count();
                        if (cantidadDeMovimientos>movimientostarjetaDTOS.CantMovimientos)
                        {
                            cantidadDeMovimientos = Convert.ToInt32(movimientostarjetaDTOS.CantMovimientos);
                        }


                        var pagoTarjeta = new PagoTarjeta
                        {
                            NroTarjeta = pagotarjetaDTO.NroTarjeta,
                            //MontoAdeudado = Convert.ToDecimal(sumaTotalAcumulado.Replace(".", ",")),
                            MontoAdeudado = Convert.ToDecimal(sumaProximoVencimientoAcumulado),
                            FechaVencimiento = fechaProximoPago==null? DateTime.MinValue : (DateTime)fechaProximoPago,                         
                            Persona = pers, 
                            EstadoPago = EstadoPago.Pendiente,  
                            FechaComprobante = DateTime.Now, 
                            FechaPagoProximaCuota = fechaProximoPago==null ? DateTime.MinValue : (DateTime)fechaProximoPago,
                        };
                        //_context.PagoTarjeta.Add(pagoTarjeta);
                        //_context.SaveChanges();
                        pagotarjetaDTO.id = pagoTarjeta.Id;


                        return new JsonResult(new PagoTarjetaDTO { Status = 200, UAT = pagotarjetaDTO.UAT, Mensaje = "Solicitud de pago de tarjeta", NroTarjeta = pagotarjetaDTO.NroTarjeta.ToString(), Persona = pers.Id,  MontoAdeudado = pagoTarjeta.MontoAdeudado.ToString().Replace(".", ","), EstadoPago = EstadoPago.Pendiente,  FechaPagoProximaCuota = pagoTarjeta.FechaPagoProximaCuota?.ToString("dd/MM/yyyy"), FechaVencimiento=pagoTarjeta.FechaVencimiento?.ToString("dd/MM/yyyy"), FechaComprobante=pagoTarjeta.FechaComprobante?.ToString("dd/MM/yyyy"), id=pagotarjetaDTO.id, alias = empresa.Alias ,CBU=empresa.CBU });
                    }
                    else
                        return new JsonResult(new PagoTarjetaDTO { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = "Error a Solicitar pago de tarjeta" });

            }
            catch (Exception e)
            {
                Log.Error($"Error en creacion de tarjeta - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"Error en creacion de terjeta" });
            }

        }

        [HttpPost("SubirComprobantePagoTarjeta")]
        public IActionResult SubirComprobantePagoTarjeta([FromBody] PagoTarjetaDTO pagotarjetaDTO)
        {
            //var pagotarjetaPrueba = _context.PagoTarjeta.Where(x => x.Id == 1).FirstOrDefault();
            //pagotarjetaDTO.ComprobantePago = pagotarjetaPrueba.ComprobantePago;
            try
            {
                if (pagotarjetaDTO.ComprobantePago!=null)
                //if (true)
                {                
                    var usuario = TraeUsuarioUAT(pagotarjetaDTO.UAT);
                    if (usuario == null)
                        return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"no existe UAT de Usuario" });

                    ListaMovimientoTarjetaDTO movimientostarjetaDTOS = new ListaMovimientoTarjetaDTO();

                    DatosEstructura empresa = _context.DatosEstructura.FirstOrDefault();
                    var movtarj = new MovPersona();

                    var tipomov = _context.TipoMovimientoTarjeta.FirstOrDefault();
                    var nuevosMovimientos = movtarj.ConsultarMovimientosTarjetas2(empresa.UsernameWS, empresa.PasswordWS, usuario.Personas.NroDocumento, Convert.ToInt64(pagotarjetaDTO.NroTarjeta), 100, 0); 
                    var pers = _context.Personas.Where(x => x.NroTarjeta == usuario.Personas.NroTarjeta).FirstOrDefault();


                    if (nuevosMovimientos.Detalle.Resultado == "EXITO")
                    {
                        List<MovimientoTarjetaDTO> resultadoNuevos = new List<MovimientoTarjetaDTO>();
                        if (movimientostarjetaDTOS.tipomovimiento == 0)
                        {
                            resultadoNuevos = nuevosMovimientos.Movimientos
                            .GroupBy(m => new { m.Descripcion, m.Fecha })
                            .Select(g => new MovimientoTarjetaDTO
                            {
                                Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                                TipoMovimiento = g.Key.Descripcion,
                                Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                            })
                            .ToList();
                        }
                        else
                        {
                            resultadoNuevos = nuevosMovimientos.Movimientos.Where(x => x.Descripcion == tipomov.Nombre)
                            .GroupBy(m => new { m.Descripcion, m.Fecha })
                            .Select(g => new MovimientoTarjetaDTO
                            {
                                Monto =  (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ","))==null ? g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", "."))).ToString().Replace(".", ",") : (g.Sum(m => Convert.ToDecimal(m.Monto.Replace(",", ".")) + Convert.ToDecimal(m.Recargo.Replace(",", "."))).ToString().Replace(".", ",")),
                                TipoMovimiento = g.Key.Descripcion,
                                Fecha = g.Key.Fecha.Date.ToString("dd/MM/yyyy")
                            })
                            .ToList();
                        }

                        List<ListDetalleCuotaDTO> listDetalle = new List<ListDetalleCuotaDTO>();

                        foreach (var item in nuevosMovimientos.DetallesSolicitud)
                        {
                            foreach (var itemMovimiento in item.DetallesCuota)
                            {
                                if (listDetalle.Any(x => x.Fecha == itemMovimiento.Fecha))
                                {
                                    var detalle = listDetalle.Where(x => x.Fecha == itemMovimiento.Fecha).First();
                                    detalle.Monto = (Convert.ToDecimal(detalle.Monto) + Convert.ToDecimal(itemMovimiento.Monto)).ToString();
                                }
                                else
                                {
                                    listDetalle.Add(new ListDetalleCuotaDTO()
                                    {
                                        Fecha = itemMovimiento.Fecha,
                                        Monto = itemMovimiento.Monto
                                    });
                                }
                            }
                        }

                        string deudaTotal = "0.0";
                        string saldoVencidoAcumulado = "0.0";
                        string sumaProximoVencimientoAcumulado = "0.0";
                        string sumaTotalAcumulado = "0.0";
                        bool cuotaVencida = false;
                        string fechaSiguienteCuota = "";
                        DateTime? fechaProximoPago = null;
                        foreach (var item in listDetalle.OrderBy(x => x.Fecha))
                        {
                            if (VerificarVencimiento(item.Fecha))
                            {
                                SetearCultureInfoUS();
                                var aux1 = Convert.ToDecimal(item.Monto);
                                var aux2 = Convert.ToDecimal(saldoVencidoAcumulado);
                                saldoVencidoAcumulado = (aux1+aux2).ToString();
                                cuotaVencida = true;
                            }
                            else
                            {

                                if (fechaSiguienteCuota=="")
                                    fechaSiguienteCuota = item.Fecha;

                                if ((ConvertirFecha(item.Fecha).Month==(ConvertirFecha(fechaSiguienteCuota).Month)) && (ConvertirFecha(item.Fecha).Year==ConvertirFecha(fechaSiguienteCuota).Year))
                                {
                                    if (fechaProximoPago==null)
                                        fechaProximoPago=ConvertirFecha(item.Fecha);

                                    SetearCultureInfoUS();
                                    var monto2 = Convert.ToDecimal(item.Monto);
                                    var monto1 = Convert.ToDecimal(sumaProximoVencimientoAcumulado);
                                    sumaProximoVencimientoAcumulado = (monto1+monto2).ToString();
                                }
                                SetearCultureInfoUS();
                                var monto3 = Convert.ToDecimal(item.Monto);
                                var monto4 = Convert.ToDecimal(deudaTotal);
                                deudaTotal = (monto3+monto4).ToString();
                            }

                        }
                        SetearCultureInfoUS();
                        sumaProximoVencimientoAcumulado = (Convert.ToDecimal(sumaProximoVencimientoAcumulado)+Convert.ToDecimal(saldoVencidoAcumulado)).ToString();
                        sumaTotalAcumulado = (Convert.ToDecimal(deudaTotal)+Convert.ToDecimal(saldoVencidoAcumulado)).ToString();

                        bool ContieneLeyenda = false;
                        string Leyenda = "";
                        var LeyendaTexto = _context.LeyendaTipoMovimiento.FirstOrDefault();

                        if (resultadoNuevos.Where(x => x.TipoMovimiento == LeyendaTexto.NombreMovimiento).Count()>0 && LeyendaTexto.Activo==true)
                        {
                            ContieneLeyenda = true;
                            Leyenda = LeyendaTexto.TextoLeyenda;
                        }

                        List<MovimientoTarjetaDTO> MovimientosTarjeta = nuevosMovimientos.Movimientos
                            .Select(mov => new MovimientoTarjetaDTO
                            {
                                TipoMovimiento = mov.Descripcion,
                                Monto = mov.Monto,
                                Fecha = mov.Fecha.ToString("dd/MM/yyyy")
                            })
                            .ToList();

                        int cantidadDeMovimientos = MovimientosTarjeta.Count();
                        if (cantidadDeMovimientos>movimientostarjetaDTOS.CantMovimientos)
                        {
                            cantidadDeMovimientos = Convert.ToInt32(movimientostarjetaDTOS.CantMovimientos);
                        }

                        var pagoTarjeta = new PagoTarjeta
                        {
                            NroTarjeta = pagotarjetaDTO.NroTarjeta,
                            //MontoAdeudado = Convert.ToDecimal(sumaTotalAcumulado.Replace(".", ",")),
                            MontoAdeudado = Convert.ToDecimal(sumaProximoVencimientoAcumulado),
                            FechaVencimiento = fechaProximoPago==null ? DateTime.MinValue : (DateTime)fechaProximoPago,
                            Persona = pers,
                            EstadoPago = EstadoPago.Pagado,
                            FechaComprobante = DateTime.Now,
                            FechaPagoProximaCuota = fechaProximoPago==null ? DateTime.MinValue : (DateTime)fechaProximoPago,
                            ComprobantePago = pagotarjetaDTO.ComprobantePago
                        };
                        _context.PagoTarjeta.Add(pagoTarjeta);
                        _context.SaveChanges();
                        return new JsonResult(new PagoTarjetaDTO { Status = 200, UAT = pagotarjetaDTO.UAT, Mensaje = "Comprobante cargado con exito" });
                    }
                    else
                    {
                        return new JsonResult(new PagoTarjetaDTO { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = "Error al traer el Saldo Pendiente." });
                    }
                }
                else
                {
                    return new JsonResult(new PagoTarjetaDTO { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = "No hay comprobante cargado" });
                }                    
                
                //else
                //    return new JsonResult(new PagoTarjetaDTO { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = "No tiene pagos pendientes para subir archivo" });

            }
            catch (Exception e)
            {
                Log.Error($"Error en creacion de tarjeta - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"Error al subir comprobante"});
            }

        }

		[HttpPost("ObtenerComprobantes")]
		public IActionResult ObtenerComprobantes([FromBody] ComprobantesDTO pagotarjetaDTO)
		{
			try
			{
				var usuario = TraeUsuarioUAT(pagotarjetaDTO.UAT);
                var personaId = 0;
				if (usuario == null)
					return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"no existe UAT de Usuario" });

                if (usuario.Personas!=null)
                {
					personaId=usuario.Personas.Id;
				}else if (usuario.Clientes!=null)
                {
                    if (usuario.Clientes.Persona!=null)
                    {
						personaId=usuario.Clientes.Persona.Id;
					}
                }
                else
                {
					return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"Error al encontrar los comprobantes" });
				}


				var comprobantes = _context.PagoTarjeta.Where(x => x.Persona.Id == personaId).ToList();

                if (comprobantes.Count()>0)
                {
                    pagotarjetaDTO.ListComprobantes = new List<ListComprobantesDTO>();

                    foreach (var item in comprobantes)
                    {
                        pagotarjetaDTO.ListComprobantes.Add(new ListComprobantesDTO()
                        {
                            NroTarjeta = item.NroTarjeta,
                            FechaVencimiento = item.FechaVencimiento.ToString(),
                            MontoAdeudado = item.MontoAdeudado.ToString(),
                            FechaPagoProximaCuota = item.FechaPagoProximaCuota.ToString(),
                            EstadoPago = item.EstadoPago,
                            EstadoPagoDescripcion = item.EstadoPago.GetType().GetField(item.EstadoPago.ToString()).Name,
                            ComprobantePago = item.ComprobantePago,
                            FechaComprobante = item.FechaComprobante.ToString(),
                        });
                        pagotarjetaDTO.Mensaje = "Listado de Comprobantes";
                        pagotarjetaDTO.Status = 200;
                        pagotarjetaDTO.PersonaId = personaId;

					}
					return new JsonResult(pagotarjetaDTO);
				}
                else
                {
                    return new JsonResult(new PagoTarjetaDTO { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = "No hay comprobante cargado" });

                }

			}
			catch (Exception e)
			{
				Log.Error($"Error en creacion de tarjeta - {e.Message}");
				return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"Error al obtener los comprobantes" });
			}

		}

		[HttpPost("TraeDetalleSolicitudPago")]
        public IActionResult TraeDetalleSolicitudPago([FromBody] PagoTarjetaDTO pagotarjetaDTO)
        {
            try
            {
                var usuario = TraeUsuarioUAT(pagotarjetaDTO.UAT);
                if (usuario == null)
                    return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"no existe UAT de Usuario" });

                var pagotarjeta = _context.PagoTarjeta.Where(x => x.Id == pagotarjetaDTO.id && x.EstadoPago == EstadoPago.Pagado).FirstOrDefault();

                if (pagotarjeta != null)
                {
                    if (pagotarjeta.ComprobantePago != null)
                    {
                        
                        return new JsonResult(new PagoTarjetaDTO { Status = 200, UAT = pagotarjetaDTO.UAT, Mensaje = "Comprobante cargado con exito" , ComprobantePago = pagotarjeta.ComprobantePago, MontoAdeudado = pagotarjeta.MontoAdeudado.ToString(), FechaVencimiento= pagotarjeta.FechaVencimiento.ToString(), FechaPagoProximaCuota =pagotarjeta.FechaPagoProximaCuota.ToString(), EstadoPago = pagotarjeta.EstadoPago , Persona = pagotarjeta.Persona.Id , NroTarjeta = pagotarjeta.NroTarjeta});
                    }
                    else
                    {
                        return new JsonResult(new PagoTarjetaDTO { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = "No hay comprobante cargado" });

                    }
                }
                else
                    return new JsonResult(new PagoTarjetaDTO { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = "No tiene pagos pendientes para subir archivo" });

            }
            catch (Exception e)
            {
                Log.Error($"Error en creacion de tarjeta - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"Error al subir comprobante" });
            }

        }
     
        [HttpPost("RegistrarPago")]
        public async Task<IActionResult> RegistrarPago([FromBody] MConciliacionDePagoDTO pagotarjetaDTO)
        {
            try
            {
                var usuario = TraeUsuarioUAT(pagotarjetaDTO.UAT);
                if (usuario == null)
                    return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"no existe UAT de Usuario" });

                Payment payment = await _mp.GetPago(pagotarjetaDTO.MercadoPagoId);
                if (payment==null)
                {
                    return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"No se pudo guardar el pago" });

                }
                ConciliacionDePago conciliacionDePago = new ConciliacionDePago
                {
                    Fecha = DateTime.Now,
                    Monto = Convert.ToDecimal(payment.TransactionAmount),
                    Usuario = usuario,
                    MercadoPagoId = pagotarjetaDTO.MercadoPagoId.ToString(),
                    Descripcion = payment.Description
                };
                conciliacionDePago.SetEstado(payment.Status);

                _context.ConciliacionDePago.Add(conciliacionDePago);
                _context.SaveChanges();
                return new JsonResult(new RespuestaAPI { Status = 200, UAT = pagotarjetaDTO.UAT, Mensaje = $"Se guardo el Pago correctamente" });
            }
            catch (Exception e)
            {
                Log.Error($"Error en creacion de tarjeta - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = pagotarjetaDTO.UAT, Mensaje = $"Error al obtener los comprobantes" });
            }
        }       


        private bool VerificarVencimiento(string fecha)
        {
            SetearCultureInfoES();
            // Obtener la fecha actual
            DateTime fechaActual = DateTime.Now;
            //DateTime fechaActual = DateTime.Today;            			

			DateTime fechaIngresada;

            var periodoActual = _context.Periodo
                .FirstOrDefault(p => fechaActual >= p.FechaDesde && fechaActual <= p.FechaHasta);


            if (DateTime.TryParse(fecha, out fechaIngresada))
			{

                if (fechaIngresada<=periodoActual.FechaHasta)
                {
                    return true;
                }
                return false;


                // Comparar la fecha ingresada con la fecha actual
                if (fechaActual>fechaIngresada)
				{
                    return true;
				}
				return false;
			}
			else
			{
				return false;
			}
		}

        private DateTime ConvertirFecha(string fecha)
        {
            SetearCultureInfoES();
            DateTime fechaIngresada;
            if (DateTime.TryParse(fecha, out fechaIngresada))
            {
                return fechaIngresada;
            }
            else
            {
                return fechaIngresada;
            }
        }

        private DateTime FechaActual()
        {
            SetearCultureInfoES();
            DateTime fecha = DateTime.Now.Date;
            return fecha;
        }

        private DateTime FechaActual2()
        {
            string fecha = DateTime.Now.ToString();  
            DateTime fechaParseada;
            if (DateTime.TryParse(fecha, out fechaParseada))
            {
                return fechaParseada;
            }
            else
            {
                return fechaParseada;
            }
        }

        private void SetearCultureInfoES()
		{
            CultureInfo cultura = new CultureInfo("es-ES");
            CultureInfo.CurrentCulture = cultura;
            CultureInfo.CurrentUICulture = cultura;
        }

        private void SetearCultureInfoUS()
        {
            CultureInfo cultura = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = cultura;
            CultureInfo.CurrentUICulture = cultura;
        }

        static string InvertirFecha(string fechaOriginal)
        {
            // Dividir la cadena por el carácter "-"
            string[] partes = fechaOriginal.Split('-');

            // Reconstruir la cadena en el orden deseado
            string fechaReversa = partes[2] + "-" + partes[1] + "-" + partes[0];

            return fechaReversa;
        }
    }
}
