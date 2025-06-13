using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Data;
using Serilog;
using EstanciasCore.API.Filters;
using DAL.Mobile;
using DAL.Models.Core;

namespace EstanciasCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class MCuentasController : BaseApiController
    {

        public MCuentasController(EstanciasContext context) : base(context)
        {
        }

        [HttpPost("Alta")]
        public async Task<IActionResult> Alta([FromBody] AltaCuentaDTO altaCuentaDTO)
        {
            try
            {
                var billetera = TraeBilletera(TraeUsuarioUAT(altaCuentaDTO.UAT));
                var cuenta = new CuentaBancaria { Titular = altaCuentaDTO.Titular, Numero = altaCuentaDTO.Numero, Alias = altaCuentaDTO.Alias,CBU = altaCuentaDTO.CBU, Terceros = altaCuentaDTO.Terceros };
                billetera.Cuentas.Add(cuenta);
                _context.Update(billetera);
                await _context.SaveChangesAsync();

                return new JsonResult(new RespuestaAPI { Status = 200, UAT = altaCuentaDTO.UAT, Mensaje = "Cuenta creada con exito" });
            }
            catch (Exception e)
            {
                Log.Error($"Error en creacion de cuenta - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = altaCuentaDTO.UAT, Mensaje = $"Error en creacion de cuenta" });
            }

        }


    }




}
