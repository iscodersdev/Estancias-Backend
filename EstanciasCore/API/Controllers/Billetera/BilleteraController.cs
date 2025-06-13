using EstanciasCore.API.Filters;
using DAL.Data;
using DAL.DTOs.API;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class BilleteraController : BaseApiController
    {

        public BilleteraController(EstanciasContext context) : base(context)
        {

        }


        [HttpPost("MediosPago")]
        public async Task<IActionResult> MediosPago([FromBody] RespuestaAPI consultaDTO)
        {
            try
            {
                var usuario = TraeUsuarioUAT(consultaDTO.UAT);
                var billetera = TraeBilletera(usuario);

                List<MedioPagoDTO> mediosPago = billetera.Tarjetas.Select(t => new MedioPagoDTO { Id = t.Id, Descripcion = t.Numero, TipoMedioPago = TipoMedioPago.TipoTarjeta, DetalleAdicional = t.Vencimiento }).ToList();
                mediosPago.AddRange(billetera.Cuentas.Where(c => !c.Terceros).Select(c => new MedioPagoDTO { Id = c.Id, Descripcion = c.CBU, TipoMedioPago = TipoMedioPago.TipoCuenta, DetalleAdicional = c.Titular }).ToList());
                mediosPago.Add(new MedioPagoDTO { Id = billetera.Id, Descripcion = "Mi Billetera", TipoMedioPago = TipoMedioPago.TipoBilletera, DetalleAdicional = billetera.CVU });
                return new JsonResult(new ListaMediosPagoDTO { Status = 200, UAT = consultaDTO.UAT, Mensaje = "Medios de pago enviados", MediosPago = mediosPago.OrderBy(m => m.TipoMedioPago).ToList() });
            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta de medios de pago - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = consultaDTO.UAT, Mensaje = "Error en consulta de medios de pago" });
            }

        }

        [HttpPost("BilleterasAsociadas")]
        public async Task<IActionResult> BilleterasAsociadas([FromBody] RespuestaAPI consultaDTO)
        {
            try
            {
                var billetera = TraeBilletera(TraeUsuarioUAT(consultaDTO.UAT));

                var billeterasAsociadas = _context.Billeteras.Where(b => billetera.Contactos.Select(c => c.ClienteContacto.Id).Contains(b.Cliente.Id))
                    .Select(b => new BilleteraAsociadaDTO { Titular = b.Cliente.RazonSocial, CVU = b.CVU }).ToList();
                return new JsonResult(new ListaBilleterasDTO { Status = 200, UAT = consultaDTO.UAT, Mensaje = "Billeteras asociadas enviadas", Billeteras = billeterasAsociadas });

            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta de billeteras asociadas - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = consultaDTO.UAT, Mensaje = "Error en consulta de billeteras asociadas" });
            }

        }

        [HttpPost("SaldoBilletera")]
        public async Task<IActionResult> SaldoBilletera([FromBody] RespuestaAPI consultaSaldoDTO)
        {
            try
            {
                var usuario = TraeUsuarioUAT(consultaSaldoDTO.UAT);
                var billetera = _context.Billeteras.Where(b => b.Cliente.Usuario.Id == usuario.Id).FirstOrDefault();
                return new JsonResult(new SaldoDTO { Status = 200, UAT = consultaSaldoDTO.UAT, Mensaje = "Saldo enviado", Saldo = billetera.Saldo });
            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta del saldo - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = consultaSaldoDTO.UAT, Mensaje = "Error en consulta de saldo" });
            }

        }

        [HttpPost("CVUBilletera")]
        public async Task<IActionResult> CVUBilletera([FromBody] RespuestaAPI consultaDTO)
        {
            try
            {
                var usuario = TraeUsuarioUAT(consultaDTO.UAT);
                var billetera = _context.Billeteras.Where(b => b.Cliente.Usuario.Id == usuario.Id).FirstOrDefault();
                return new JsonResult(new CVUBilleteraDTO { Status = 200, UAT = consultaDTO.UAT, Mensaje = "CVU enviado", CVU = billetera.CVU, Alias = billetera.AliasCVU });
            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta del CVU - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = consultaDTO.UAT, Mensaje = "Error en consulta de CVU" });
            }

        }

        [HttpPost("MovimientosBilletera")]
        public async Task<IActionResult> MovimientosBilletera([FromBody] RespuestaAPI consultaMovimientosDTO)
        {
            try
            {
                var usuario = TraeUsuarioUAT(consultaMovimientosDTO.UAT);
                var billetera = _context.Billeteras.Where(b => b.Cliente.Usuario.Id == usuario.Id).FirstOrDefault();
                var movimientos = billetera.Movimientos.Select(m => new MovimientoBilleteraDTO { Monto = m.Monto, TipoMovimiento = m.TipoMovimiento.Nombre, Fecha = m.Fecha }).ToList();
                movimientos.AddRange(billetera.Tarjetas.SelectMany(t => t.Movimientos).Select(m => new MovimientoBilleteraDTO { Monto = m.Monto, TipoMovimiento = m.TipoMovimiento.Nombre, Fecha = m.Fecha }).ToList());
                movimientos.AddRange(billetera.Cuentas.Where(c => !c.Terceros).SelectMany(c => c.Movimientos).Select(m => new MovimientoBilleteraDTO { Monto = m.Monto, TipoMovimiento = m.TipoMovimiento.Nombre, Fecha = m.Fecha }).ToList());
                return new JsonResult(new ListaMovimientoDTO { Status = 200, UAT = consultaMovimientosDTO.UAT, Mensaje = "Movimientos enviados", Movimientos = movimientos.OrderByDescending(m => m.Fecha).ToList() });
            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta de movimientos - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = consultaMovimientosDTO.UAT, Mensaje = "Error en consulta de movimientos" });
            }

        }

        [HttpPost("TarjetasBilletera")]
        public async Task<IActionResult> TarjetasBilletera([FromBody] RespuestaAPI consultaTarjetasDTO)
        {
            try
            {
                var billetera = TraeBilletera(TraeUsuarioUAT(consultaTarjetasDTO.UAT));
                var tarjetas = billetera.Tarjetas.Select(t => new TarjetasBilleteraDTO { Titular = t.Titular, Numero = t.Numero, Vencimiento = t.Vencimiento }).ToList();
                return new JsonResult(new ListaTarjetasDTO { Status = 200, UAT = consultaTarjetasDTO.UAT, Mensaje = "Tarjetas enviadas", Tarjetas = tarjetas });
            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta de tarjetas - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = consultaTarjetasDTO.UAT, Mensaje = "Error en consulta de tarjetas" });
            }

        }

        [HttpPost("CuentasBilletera")]
        public async Task<IActionResult> CuentasBilletera([FromBody] RespuestaAPI consultaDTO)
        {
            try
            {
                var billetera = TraeBilletera(TraeUsuarioUAT(consultaDTO.UAT));
                var cuentas = billetera.Cuentas.Where(c => !c.Terceros).Select(c => new CuentaBilleteraDTO { CBU = c.CBU, Descripcion = $"{c.Alias} {c.Titular}".Trim() }).ToList();
                return new JsonResult(new ListaCuentasDTO { Status = 200, UAT = consultaDTO.UAT, Mensaje = "Cuentas enviadas", Cuentas = cuentas });
            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta de tarjetas - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = consultaDTO.UAT, Mensaje = "Error en consulta de cuentas" });
            }

        }

        [HttpPost("CuentasTercerosBilletera")]
        public async Task<IActionResult> CuentasTercerosBilletera([FromBody] RespuestaAPI consultaDTO)
        {
            try
            {
                var billetera = TraeBilletera(TraeUsuarioUAT(consultaDTO.UAT));
                var cuentas = billetera.Cuentas.Where(c => c.Terceros).Select(c => new CuentaBilleteraDTO { CBU = c.CBU, Descripcion = $"{c.Alias} {c.Titular}".Trim() }).ToList();
                return new JsonResult(new ListaCuentasDTO { Status = 200, UAT = consultaDTO.UAT, Mensaje = "Cuentas enviadas", Cuentas = cuentas });
            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta de tarjetas - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = consultaDTO.UAT, Mensaje = "Error en consulta de cuentas" });
            }

        }


    }
}
