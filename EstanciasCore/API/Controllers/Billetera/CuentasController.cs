using EstanciasCore.API.Filters;
using DAL.Data;
using DAL.DTOs.API;
using DAL.Models;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading.Tasks;

namespace EstanciasCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class CuentasController : BaseApiController
    {

        public CuentasController(EstanciasContext context) : base(context)
        {
        }

        [HttpPost("Alta")]
        public async Task<IActionResult> Alta([FromBody] AltaCuentaDTO altaCuentaDTO)
        {
            try
            {
                var billetera = TraeBilletera(TraeUsuarioUAT(altaCuentaDTO.UAT));
                var cuenta = new CuentaBancaria { Titular = altaCuentaDTO.Titular, Numero = altaCuentaDTO.Numero, Alias = altaCuentaDTO.Alias, CBU = altaCuentaDTO.CBU, Terceros = altaCuentaDTO.Terceros };
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
