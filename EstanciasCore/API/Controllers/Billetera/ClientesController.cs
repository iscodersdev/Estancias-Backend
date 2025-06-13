using EstanciasCore.API.Filters;
using DAL.Data;
using DAL.DTOs.API;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading.Tasks;

namespace EstanciasCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : BaseApiController
    {

        public ClientesController(EstanciasContext context) : base(context)
        {

        }

        [HttpPost("ClienteBilletera")]
        public async Task<IActionResult> ClienteBilletera([FromBody] ClienteBilleteraDTO clienteBilleteraDTO)
        {
            try
            {
                var billetera = TraeBilleteraCVU(clienteBilleteraDTO.CVU);
                var cliente = billetera.Cliente;

                return new JsonResult(new ClienteBilleteraDTO { Status = 200, UAT = clienteBilleteraDTO.UAT, Mensaje = "Datos de contacto enviados", Nombre = cliente.RazonSocial });
            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta de medios de pago - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = clienteBilleteraDTO.UAT, Mensaje = "Error en consulta de contacto" });
            }

        }

    }
}
