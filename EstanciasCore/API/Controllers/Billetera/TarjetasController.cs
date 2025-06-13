using EstanciasCore.API.Filters;
using DAL.Data;
using DAL.DTOs.API;
using DAL.Models;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class TarjetasController : BaseApiController
    {

        public TarjetasController(EstanciasContext context) : base(context)
        {
        }

        [HttpPost("Alta")]
        public async Task<IActionResult> Alta([FromBody] AltaTarjetaDTO altaTarjetaDTO)
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


    }
}
