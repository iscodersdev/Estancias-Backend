using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Data;
using Serilog;
using EstanciasCore.API.Filters;
using System.Data;
using EstanciasCore.Services;
using DAL.Mobile;
using DAL.Models.Core;

namespace EstanciasCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class MPagosController : BaseApiController
    {
        private readonly NotificacionAPIService _notificacionAPIService;

        public MPagosController(EstanciasContext context, NotificacionAPIService notificacionAPIService) : base(context)
        {
            _notificacionAPIService = notificacionAPIService;
        }

        [HttpPost("DetallePagoBarras")]
        public async Task<IActionResult> DetallePagoBarras([FromBody] DetallePagoBarrasDTO detallePagoDTO)
        {
            try
            {
                //Uso el Mock de la API de pagos por ahora
                var detallePago = _context.MockServicios.Where(s => s.CodigoBarras == detallePagoDTO.CodigoBarras).FirstOrDefault();

                return new JsonResult(new DetallePagoBarrasDTO { Status = 200, UAT = detallePagoDTO.UAT, Mensaje = "Detalle de pago enviado",CodigoBarras = detallePago.CodigoBarras, Monto =detallePago.Monto, NombreServicio = detallePago.NombreServicio });

            }
            catch (Exception e)
            {
                Log.Error($"Error en detalle de pagos para servicios - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = detallePagoDTO.UAT, Mensaje = "Error en detalle de pago para servicios" });
            }

        }

        [HttpPost("ConfirmaPagoBarras")]
        public async Task<IActionResult> ConfirmaPagoBarras([FromBody] ConfirmaPagoBarrasDTO confirmaPagoDTO)
        {
            try
            {
                //Uso el Mock de la API de pagos por ahora
                var detallePago = _context.MockServicios.Where(s => s.CodigoBarras == confirmaPagoDTO.CodigoBarras).FirstOrDefault();

                var billeteraPagadora = TraeBilletera(TraeUsuarioUAT(confirmaPagoDTO.UAT));
                if (!billeteraPagadora.ChequeaDebito(detallePago.Monto))
                {
                    Log.Error($"Error el monto supera el saldo");
                    return new JsonResult(new RespuestaAPI { Status = 400, UAT = confirmaPagoDTO.UAT, Mensaje = "El monto supera su saldo" });
                }

                var movimiento = new MovimientoBilletera
                {
                    Fecha = DateTime.Now,
                    Monto = detallePago.Monto,
                    TipoMovimiento = _context.TipoMovimientoBilletera.Find((int)TipoMovimientoBilleteraEnum.PagoServicio)
                };

                var exito = true;
                switch (confirmaPagoDTO.TipoMedioPago)
                {
                    case TipoMedioPago.TipoBilletera:
                        billeteraPagadora.Movimientos.Add(movimiento);
                        break;
                    case TipoMedioPago.TipoTarjeta:
                        var tarjetaPagadora = billeteraPagadora.Tarjetas.Where(t => t.Id == confirmaPagoDTO.MetodoPagoId).FirstOrDefault();
                        tarjetaPagadora.Movimientos.Add(movimiento);
                        break;
                    case TipoMedioPago.TipoCuenta:
                        var cuentaPagadora = billeteraPagadora.Cuentas.Where(c => c.Id == confirmaPagoDTO.MetodoPagoId).FirstOrDefault();
                        cuentaPagadora.Movimientos.Add(movimiento);
                        break;
                    default:
                        exito = false;
                        break;
                }
                if (exito)
                {
                    billeteraPagadora.Saldo -= detallePago.Monto;
                    _context.Update(billeteraPagadora);
                    _context.SaveChanges();
                    _notificacionAPIService.Envia_Push(billeteraPagadora.Cliente.Usuario.DeviceId, "Pago de servicios", $"Ha abonado un servicio por ${detallePago.Monto} desde su billetera");
                    return new JsonResult(new RespuestaAPI { Status = 200, UAT = confirmaPagoDTO.UAT, Mensaje = "Pago de servicio efectuado" });
                }
                else
                {
                    return new JsonResult(new RespuestaAPI { Status = 400, UAT = confirmaPagoDTO.UAT, Mensaje = "Error en el registro del pago" });
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error en confirmacion de pagos para servicios - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 400, UAT = confirmaPagoDTO.UAT, Mensaje = "Error en la confirmacion de pago" });
            }

        }


    }
}
