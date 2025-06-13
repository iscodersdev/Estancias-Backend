using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Data;
using Serilog;
using EstanciasCore.API.Filters;
using EstanciasCore.Services;

namespace EstanciasCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class MVentasController : BaseApiController
    {
        private readonly NotificacionAPIService _notificacionAPIService;
        private readonly EstanciasContext _context;

        public MVentasController(EstanciasContext context, NotificacionAPIService notificacionAPIService) : base(context)
        {
            _notificacionAPIService = notificacionAPIService;
            _context = context;
        }

        

        [HttpPost("ConfirmaVentas")]
        public async Task<IActionResult> ConfirmaVentas([FromBody] MVentaDTO confirmaVentaDTO)
        {
            try
            {
                return new JsonResult(new RespuestaAPI { Status = 200, UAT = confirmaVentaDTO.UAT , Mensaje = "Venta ejecutada satifactoriamente" });
            }
            catch (Exception e)
            {
                Log.Error($"Error en confirmacion de pagos para servicios - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 400, UAT = confirmaVentaDTO.UAT, Mensaje = "Error en la confirmacion de pago" });
            }

        }


    }
}
