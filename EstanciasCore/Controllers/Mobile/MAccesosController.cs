using System;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Cors;
using DAL.Data;
using DAL.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Commons.Controllers;
using Commons.Identity.Services;

namespace EstanciasCore.Controllers
{
    [Route("api/[controller]")]
    public class MAccesosController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MAccesosController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        [Route("GrabaAcceso")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MGrabaAccesoDTO GrabaAcceso([FromBody] MGrabaAccesoDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            uat.Status = 200;
            var puesto = _context.PuestosCodigos.FirstOrDefault(x => x.HashQR == uat.Token);
            if (puesto == null)
            {
                uat.Status = 500;
                uat.Mensaje = "QR Invalido";
                return uat;
            }
            if (puesto.Puesto.Empresa.Id != Uat.Cliente.Empresa.Id || Uat.Cliente.NumeroCliente==null)
            {
                uat.Status = 500;
                uat.Mensaje = "Persona No Asociada al Club";
                return uat;
            }
            uat.Status = 200;
            uat.Mensaje = "Acceso Registrado Correctamente";
            Accesos acceso = new Accesos();
            acceso.Cliente = Uat.Cliente;
            acceso.Coordenadas = uat.Coordenadas;
            acceso.FechaHora = DateTime.Now;
            acceso.Puesto = puesto.Puesto;
            acceso.Deuda = 0;
            acceso.EstadoDeuda = _context.EstadosDeudas.Find(1);
            var deuda = _context.CuentaCorriente.OrderByDescending(x => x.Fecha.Date).ThenByDescending(x => x.Concepto.Signo).FirstOrDefault(x => x.Cliente.Id == Uat.Cliente.Id );
            if (deuda == null)
            {
                acceso.EstadoDeuda = _context.EstadosDeudas.Find(3);
            }
            else
            {
                if (deuda.Saldo > 0)
                {
                    acceso.Deuda = deuda.Saldo;
                    if (deuda.Vencimiento is null)
                    {
                        acceso.EstadoDeuda = _context.EstadosDeudas.Find(4);
                        uat.Mensaje = " - Ud. Su Deuda es $" + acceso.Deuda.ToString();
                    }
                    else
                    if (deuda.Vencimiento > DateTime.Now)
                    {
                        acceso.EstadoDeuda = _context.EstadosDeudas.Find(4);
                        uat.Mensaje = " - Ud. Su Deuda sin Vencer $" + acceso.Deuda.ToString();
                    }
                    else
                    {
                        acceso.EstadoDeuda = _context.EstadosDeudas.Find(2);
                        uat.Mensaje = " - Su Deuda Vencida es $" + acceso.Deuda.ToString();
                    }
                }
            }
            if (puesto.Puesto.Coordenadas!=null)
            {
                double latitudpuesto = Convert.ToDouble(puesto.Puesto.Coordenadas.Split(" ")[0]);
                double latitudacceso = Convert.ToDouble(uat.Coordenadas.Split(" ")[0]);
                double longitudpuesto = Convert.ToDouble(puesto.Puesto.Coordenadas.Split(" ")[1]);
                double longitudacceso = Convert.ToDouble(uat.Coordenadas.Split(" ")[1]);
                if (Math.Abs(latitudpuesto - longitudpuesto) < 0.01)
                {
                    acceso.ValidadoPorGPS = true;
                }
                else
                {
                    acceso.ValidadoPorGPS = false;
                }
            }
            else
            {
                acceso.ValidadoPorGPS = true;
            }
            var turnos = _context.Reservas.Where(x => x.Fecha.Date == acceso.FechaHora.Date && x.Cliente.Id == acceso.Cliente.Id);
            acceso.Turnos = "";
            foreach(var turno in turnos)
            {
                acceso.Turnos = ((acceso.Turnos.Length == 0) ? "" : "; ") + acceso.Turnos + turno.Horario.Servicio.Nombre + " - " + turno.Horario.Nombre;
            }
            _context.Accesos.Add(acceso);
            _context.SaveChanges();
            return uat;
        }
        [HttpPost]
        [Route("TraeAccesos")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MAccesosDTO TraeAccesos([FromBody] MAccesosDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            uat.Status = 200;
            var accesos = _context.Accesos.Where(x => x.Cliente.Id == Uat.Cliente.Id && x.Id<uat.FilaActual).OrderByDescending(x => x.FechaHora).Select(x => new MAccesos { FechaHora = x.FechaHora, Id = x.Id, Puesto = x.Puesto.Nombre, Tipo = x.EstadoDeuda.Nombre,Deuda=x.Deuda==0?"Cuota Al Dia":"Deuda $"+x.Deuda.ToString() }).Take(5).ToList();
            if (accesos.Count > 0)
            {
                uat.Accesos = accesos;
            }
            return uat;
        }
    }
}