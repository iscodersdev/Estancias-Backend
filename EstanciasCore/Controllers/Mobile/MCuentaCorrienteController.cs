using DAL.Data;
using DAL.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Commons.Controllers;
using Commons.Identity.Services;

namespace EstanciasCore.Controllers
{
    [Route("api/[controller]")]

    public class MCuentaCorrienteController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MCuentaCorrienteController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        [Route("TraeCuentaCorriente")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeCuentaCorrienteDTO TraeCuentaCorriente([FromBody] MTraeCuentaCorrienteDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            var cuentacorriente = _context.CuentaCorriente.Where(x=>x.Cliente.Id==Uat.Cliente.Id).OrderByDescending(x => x.Fecha).Select(x => new MCuentaCorriente { Fecha = x.Fecha,Concepto=x.Concepto.Nombre, Id = x.Id, Importe=x.Importe,Saldo=x.Saldo,Observaciones=x.Observaciones }).ToList();
            if (cuentacorriente.Count > 0)
            {
                uat.CuentaCorriente =cuentacorriente;
            }
            uat.Status = 200;
            return uat;
        }
    }
}