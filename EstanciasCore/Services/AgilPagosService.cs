using System;  
using DAL.Data;
using System.Net.Http;
using System.Linq;
using DAL.DTOs.AgilPagosDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace EstanciasCore.Services
{
    [ApiController]
    [AllowAnonymous]
    public class AgilPagosService : ControllerBase
    {
        public EstanciasContext _context;
        public string UrlNotificacion = "https://billetera-qa.agilpagos.com.ar/"; //Testing
    // public string UrlNotificacion = "https://billetera.agilpagos.com.ar/"; //Produccion
        public AgilPagosService(EstanciasContext context)
        {
            _context = context;
        }


        #region Endpoints a desarrollar por Entidad 

        [Route("api/Cliente/{tipoDocumento}/{numeroDocumento}")]
        [HttpPost]
        public IActionResult ValidarCliente([FromBody] ValidarClienteDTO cliente, string tipoDocumento, string numeroDocumento)
        {
            try
            {
                var result = _context.Clientes.Where(x => x.Persona.NroDocumento == numeroDocumento && x.Persona.TipoDocumento.Id == Convert.ToInt32(tipoDocumento)).FirstOrDefault();
                if (result != null)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [Route("api/Cuentas/NovedadCVU")]
        [HttpPost]
        public IActionResult NovedadCVU([FromBody] NovedadCVUDTO novedad)
        {
            try
            {
                var result = "Cargar Novedad";
                if (result != null)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [Route("api/Saldo/{cuit}/{nroCuentaEnEntidad}")]
        [HttpGet]
        public IActionResult Saldo(string cuit, string nroCuentaEnEntidad)
        {
            try
            {
                var result = "Cargar Novedad";
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("api/Transacciones")]
        [HttpPost]
        public IActionResult Transacciones(TransaccionesDTO transaccion)
        {
            try
            {
                var result = "Cargar Novedad";
                if (result != null)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("api/Conciliacion/MovimientosEntidad")]
        [HttpPost]
        public IActionResult ConciliacionMovimientosEntidad(MovimientoEntidadDTO movimientoEntidad)
        {
            try
            {
                var result = "Cargar Novedad";
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("api/Cuentas/{cuit}")]
        [HttpGet]
        public IActionResult CuentasCuit(string cuit)
        {
            try
            {
                var result = "Cargar Novedad";
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #endregion

        #region Endpoints para notificar operaciones a Billetera API 

        private IActionResult AvisoMovimientoEnEntidad(AvisoMovimientoEnEntidadDTO aviso)
        {
            ResponseNotificionApiDTO responseNotificion = new ResponseNotificionApiDTO();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UrlNotificacion);
                HttpResponseMessage response = client.PostAsJsonAsync("AvisoMovimientoEnEntidad", aviso).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<ResponseNotificionApiDTO>();
                    readTask.Wait();
                    responseNotificion = readTask.Result;
                    if(responseNotificion.pCodRespuesta == "200")
                    {
                        return Ok(responseNotificion);
                    }  
                    return NotFound(responseNotificion);                    
                }
            }
            return BadRequest(responseNotificion);
        }


        private IActionResult AvisoNovedadCuentaEnEntidad(AvisoNovedadCuentasEnEntidadDTO novedad)
        {
            ResponseNotificionApiDTO responseNotificion = new ResponseNotificionApiDTO();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UrlNotificacion);
                HttpResponseMessage response = client.PostAsJsonAsync("AvisoNovedadCuentasEnEntidad", novedad).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<ResponseNotificionApiDTO>();
                    readTask.Wait();
                    responseNotificion = readTask.Result;                   
                    if (responseNotificion.pCodRespuesta == "200")
                    {
                        return Ok(responseNotificion);
                    }
                    return NotFound(responseNotificion);
                }
            }
            return BadRequest(responseNotificion);
        }

        private IActionResult AvisoNovedadCuentaEnEntidadConCVU(AvisoNovedadCuentasEnEntidadCvuDTO novedad)
        {
            ResponseNotificionApiCvuDTO responseNotificion = new ResponseNotificionApiCvuDTO();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UrlNotificacion);
                HttpResponseMessage response = client.PostAsJsonAsync("AvisoNovedadCuentasEnEntidad", novedad).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<ResponseNotificionApiCvuDTO>();
                    readTask.Wait();
                    responseNotificion = readTask.Result;
                    if (responseNotificion.pCodRespuesta == "200")
                    {
                        return Ok(responseNotificion);
                    }
                    return NotFound(responseNotificion);
                }
            }
            return BadRequest(responseNotificion);
        }

        #endregion
    }
}

