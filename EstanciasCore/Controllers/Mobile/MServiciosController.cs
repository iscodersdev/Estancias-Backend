using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Cors;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Commons.Controllers;
using Commons.Identity.Services;
using EstanciasCore.Services;

namespace EstanciasCore.Controllers
{
    [Route("api/[controller]")]
    public class MServiciosController : BaseController
    {
        private readonly UserService<Usuario> _userManager;
        public EstanciasContext _context;
        public MServiciosController(EstanciasContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("TraeServicios")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeServiciosDTO TraeServicios([FromBody] MTraeServiciosDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            List<MListaServicios> ListaServicios = new List<MListaServicios>();
            var servicios = _context.ClientesServicios.Where(x=> x.Servicio.FechaBaja==null && x.Servicio.Empresa.Id==Uat.Cliente.Empresa.Id && x.Servicio.Activo==true && x.TipoCliente.Id==Uat.Cliente.TipoCliente.Id && (x.Servicio.Tipo.Id==uat.TipoServicioId || uat.TipoServicioId==0));
            foreach (var Servicio in servicios)
            {
                MListaServicios servicio = new MListaServicios();
                servicio.Id = Servicio.Servicio.Id;
                servicio.Nombre = Servicio.Servicio.Nombre;
                ListaServicios.Add(servicio);
            }
            uat.Status = 200;
            uat.Mensaje = "Lista Generada Correctamente";
            uat.Servicios = ListaServicios;
            return uat;
        }

        [HttpPost]
        [Route("TraeReservas")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeReservasDTO TraeReservas([FromBody] MTraeReservasDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            List<MListaReservas> ListaReservas = new List<MListaReservas>();
            var reservas = _context.Reservas.Where(x => x.Cliente.Id==Uat.Cliente.Id && x.Fecha>=DateTime.Today && x.FechaAnulada==null && (x.Horario.Servicio.Tipo.Id==uat.TipoServicioId || uat.TipoServicioId==0)).OrderBy(x=>x.Fecha).ThenBy(x=>x.Horario.Nombre);
            foreach (var Reserva in reservas)
            {
                MListaReservas reserva = new MListaReservas();
                reserva.Id = Reserva.Id;
                reserva.Nombre = Reserva.Horario.Servicio.Nombre;
                reserva.FondoServicio = Reserva.Horario.Servicio.Icono;
                reserva.Fecha = Reserva.Fecha.ToString("dddd dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
                reserva.Horario = Reserva.Horario.Nombre;
                ListaReservas.Add(reserva);
            }
            uat.Status = 200;
            uat.Mensaje = "Resulttado Correcto";
            uat.Reservas = ListaReservas;
            return uat;
        }

        [HttpPost]
        [Route("TraeFechasDisponibles")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeFechasDTO TraeFechasDisponibles([FromBody] MTraeFechasDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            List<MListaFechas> ListaFechas = new List<MListaFechas>();
            var servicio = _context.Servicios.Find(uat.ServicioId);
            int Cantidad = servicio.Cupo;
            DateTime fecha = DateTime.Today.AddDays(servicio.DiasProgramacion);
            if (servicio.FechaUnica == null)
            {
                for (DateTime fecha_ser = DateTime.Today; fecha_ser < fecha; fecha_ser = fecha_ser.AddDays(1))
                {
                    var horarios = _context.Horarios.Where(x => x.FechaBaja==null && x.Servicio.Id == uat.ServicioId && (x.DiaSemana == -1 || x.DiaSemana == Convert.ToInt32(fecha_ser.DayOfWeek)));
                    if (horarios.Count() > 0)
                    {
                        foreach (var Horario in horarios)
                        {
                            int CantidadReservada = _context.Reservas.Where(x => x.Horario.Id == Horario.Id && x.Fecha == fecha_ser && x.FechaAnulada == null).Count();
                            int CantidadReservadaPersona = _context.Reservas.Where(x => x.Horario.Id == Horario.Id && x.Fecha == fecha_ser && x.FechaAnulada == null && x.Cliente.Id==Uat.Cliente.Id).Count();
                            if (Cantidad > CantidadReservada && CantidadReservadaPersona==0)
                            {
                                MListaFechas Fecha = new MListaFechas();
                                Fecha.Fecha = fecha_ser;
                                Fecha.NombreFecha = fecha_ser.ToString("dddd dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
                                ListaFechas.Add(Fecha);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                int CantidadReservada = _context.Reservas.Where(x => x.Horario.Servicio.Id == uat.ServicioId && x.Fecha.Date == servicio.FechaUnica && x.FechaAnulada == null).Count();
                if (Cantidad > CantidadReservada)
                {
                    MListaFechas Fecha = new MListaFechas();
                    Fecha.Fecha = Convert.ToDateTime(servicio.FechaUnica);
                    Fecha.NombreFecha = Fecha.Fecha.ToString("dddd dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
                    ListaFechas.Add(Fecha);
                }
            }
            uat.Status = 200;
            uat.Mensaje = "Ok";
            uat.Fechas = ListaFechas;
            return uat;
        }

        [HttpPost]
        [Route("TraeHorariosDisponibles")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeHorariosDTO TraeHorariosDisponibles([FromBody] MTraeHorariosDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            List<MListaHorarios> ListaHorarios = new List<MListaHorarios>();
            var Horarios = _context.Horarios.Where(x => x.FechaBaja==null && x.Servicio.Id == uat.ServicioId && (x.DiaSemana==Convert.ToInt32(uat.Fecha.DayOfWeek) || x.DiaSemana==-1)).OrderBy(x => x.Orden);
            foreach (var horario in Horarios)
            {
                var reserva = _context.Reservas.Count(x => x.Horario.Id == horario.Id && x.Fecha.Date == uat.Fecha.Date && x.FechaAnulada == null);
                var reservapropia = _context.Reservas.Count(x => x.Horario.Id == horario.Id && x.Fecha.Date == uat.Fecha.Date && x.FechaAnulada == null && x.Cliente.Id==Uat.Cliente.Id);
                if (reserva < horario.Servicio.Cupo && reservapropia==0)
                {
                    MListaHorarios Hora = new MListaHorarios();
                    Hora.Id = horario.Id;
                    Hora.NombreHorario = horario.Nombre;
                    Hora.CupoDisponible = horario.Servicio.Cupo - reserva;
                    ListaHorarios.Add(Hora);
                }
            }
            uat.Status = 200;
            uat.Mensaje = "Ok";
            uat.Horarios = ListaHorarios;
            return uat;
        }

        [HttpPost]
        [Route("GrabaReserva")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MGrabaReservaDTO GrabaReserva([FromBody] MGrabaReservaDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            if (uat.HorarioId == 0)
            {
                uat.Status = 500;
                uat.Mensaje = "Debe Seleccionar Un Horario";
                return uat;
            }
            var deuda = _context.CuentaCorriente.OrderByDescending(x => x.Fecha).ThenByDescending(x=>x.Concepto.Signo).FirstOrDefault(x => x.Cliente.Id == Uat.Cliente.Id && (x.Vencimiento < DateTime.Today || x.Vencimiento == null));
            if (deuda != null)
            {
                if (deuda.Saldo > 0)
                {
                    var cuota = _context.CuentaCorriente.OrderByDescending(x => x.Fecha).ThenByDescending(x => x.Concepto.Signo).FirstOrDefault(x => x.Cliente.Id == Uat.Cliente.Id && x.Concepto.Signo == 1 && x.Vencimiento != null);
                    if (cuota != null)
                    {
                        if (cuota.Vencimiento < DateTime.Today) ;
                        {
                            uat.Status = 500;
                            uat.Mensaje = " ¨Debe Regularizar su Deuda de $" + cuota.Saldo.ToString();
                            return uat;
                        }
                    }
                }
            }
            var horario = _context.Horarios.Find(uat.HorarioId);
            if (horario.Servicio.TopeAnualFinde > 0 && (uat.Fecha.DayOfWeek == DayOfWeek.Saturday || uat.Fecha.DayOfWeek == DayOfWeek.Sunday))
            {
                int Cantidad = _context.Reservas.Where(x => x.FechaAnulada==null && x.Horario.Servicio.Id == horario.Servicio.Id && x.Fecha.Year == uat.Fecha.Year && (x.Fecha.DayOfWeek == DayOfWeek.Saturday || x.Fecha.DayOfWeek == DayOfWeek.Sunday)).Count();
                if (Cantidad >= horario.Servicio.TopeAnualFinde)
                {
                uat.Status = 500;
                uat.Mensaje = "Supera el Tope de Reservas Anuales (Fin De Semana)";
                return uat;
                }
            }
            if (horario.Servicio.TopeMensualFinde > 0 && (uat.Fecha.DayOfWeek == DayOfWeek.Saturday || uat.Fecha.DayOfWeek == DayOfWeek.Sunday))
            {
                int Cantidad = _context.Reservas.Where(x => x.FechaAnulada == null && x.Horario.Servicio.Id == horario.Servicio.Id && x.Fecha.Year == uat.Fecha.Year && x.Fecha.Month == uat.Fecha.Month && (x.Fecha.DayOfWeek == DayOfWeek.Saturday || x.Fecha.DayOfWeek == DayOfWeek.Sunday)).Count();
                if (Cantidad >= horario.Servicio.TopeMensualFinde)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Supera el Tope de Reservas Mensuales (Fin De Semana)";
                    return uat;
                }
            }
            if (horario.Servicio.TopePendienteGlobal > 0 )
            {
                int Cantidad = _context.Reservas.Where(x => x.FechaAnulada == null && x.Horario.Servicio.Id == horario.Servicio.Id && x.Fecha > uat.Fecha && x.Cliente.Id==Uat.Cliente.Id).Count();
                if (Cantidad >= horario.Servicio.TopePendienteGlobal)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Supera el Tope de Reservas Pendientes Global";
                    return uat;
                }
            }
            if (horario.Servicio.TopePendienteFinde > 0 && (uat.Fecha.DayOfWeek == DayOfWeek.Saturday || uat.Fecha.DayOfWeek == DayOfWeek.Sunday))
            {
                int Cantidad = _context.Reservas.Where(x => x.Cliente.Id==Uat.Cliente.Id && x.FechaAnulada == null && x.Horario.Servicio.Id == horario.Servicio.Id && x.Fecha > uat.Fecha && (x.Fecha.DayOfWeek == DayOfWeek.Saturday || x.Fecha.DayOfWeek == DayOfWeek.Sunday)).Count();
                if (Cantidad >= horario.Servicio.TopePendienteFinde)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Supera el Tope de Reservas Pendientes (Fin De Semana)";
                    return uat;
                }
            }
            if (horario.Servicio.TopeAnualSemana > 0)
            {
                int Cantidad = _context.Reservas.Where(x => x.FechaAnulada == null && x.Horario.Servicio.Id ==  horario.Servicio.Id && x.Fecha.Year == uat.Fecha.Year && x.Cliente.Id==Uat.Cliente.Id).Count();
                if (Cantidad >= horario.Servicio.TopeAnualSemana)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Supera el Tope de Reservas Anuales en Dias de Semana";
                    return uat;
                }
            }
            if (horario.Servicio.TopeMensualSemana > 0)
            {
                int Cantidad = _context.Reservas.Where(x => x.FechaAnulada == null && x.Horario.Servicio.Id == horario.Servicio.Id && x.Fecha.Year == uat.Fecha.Year && x.Fecha.Month == uat.Fecha.Month).Count();
                if (Cantidad >= horario.Servicio.TopeMensualSemana)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Supera el Tope de Reservas Mensuales en Dias de Semana";
                    return uat;
                }
            }
            if (horario.Servicio.TopePendienteSemana > 0)
            {
                int Cantidad = _context.Reservas.Where(x => x.FechaAnulada == null && x.Horario.Servicio.Id == horario.Servicio.Id && x.Fecha > uat.Fecha).Count();
                if (Cantidad >= horario.Servicio.TopePendienteSemana)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Supera el Tope de Reservas Pendientes en Dias de Semana";
                    return uat;
                }
            }
            if (Uat.Cliente.TipoCliente.CantidadActividadesSemanales>0)
            {
                var dia1 = uat.Fecha.AddDays((Convert.ToInt32(uat.Fecha.DayOfWeek) * -1)-1);
                var dia2 = dia1.AddDays(6);
                int Cantidad = _context.Reservas.Where(x => x.Cliente.Id==Uat.Cliente.Id && x.FechaAnulada == null && x.Fecha >= dia1 && x.Fecha<=dia2).Count();
                if (Cantidad >= Uat.Cliente.TipoCliente.CantidadActividadesSemanales)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Supera el Tope de Reservas Semanaes del Tipo de Cliente";
                    return uat;
                }
            }
            Reservas Reserva = new Reservas();
            Reserva.Fecha = uat.Fecha;
            Reserva.Horario = horario;
            Reserva.Observaciones = uat.Observaciones;
            Reserva.Cliente = Uat.Cliente;
            _context.Reservas.Add(Reserva);
            _context.SaveChanges();
            uat.Mensaje = "Reserva Grabada";
            uat.Status = 200;
            string leyenda = "";
            if (horario.Servicio.Observaciones != null)
            {
                leyenda = "<BR/><BR/><b>" + horario.Servicio.Observaciones + "</b>";
            }
            string textomail = "Confirmacion de Reserva de Actividad " + horario.Servicio.Nombre + "<BR/><BR/>Dia: " + uat.Fecha.ToString("dd/MM/yyyy") + "<BR/>Horario: " + horario.Nombre+ "<BR/><BR/>Cliente: " + Uat.Cliente?.Persona?.Apellido+", "+Uat.Cliente?.Persona?.Nombres+leyenda;
            common.EnviarMail(Uat.Cliente.Usuario.Mail, "Confirma Reserva Actividad", textomail, "Estancias", null,null);
            return uat;
        }

        [HttpPost]
        [Route("AnulaReserva")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MAnulaReservaDTO AnulaReserva([FromBody] MAnulaReservaDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            var reserva = _context.Reservas.Find(uat.ReservaId);
            reserva.FechaAnulada = DateTime.Now;
            reserva.ClienteAnula = Uat.Cliente;
            _context.Reservas.Update(reserva);
            _context.SaveChanges();
            uat.Mensaje = "Reserva Anulada";
            uat.Status = 200;
            string textomail = "Anulacion de Reserva de Actividad " + reserva.Horario.Servicio.Nombre + "<BR/><BR/>Dia: " + reserva.Fecha.ToString("dd/MM/yyyy") + "<BR/>Horario: " + reserva.Horario.Nombre + "<BR/>Cliente: " + Uat.Cliente?.Persona?.Apellido + ", " + Uat.Cliente?.Persona.Nombres;
            common.EnviarMail(Uat.Cliente.Usuario.Mail, "Anulacion Reserva Actividad", textomail, "Estancias", null,null);
            return uat;
        }
    }
}
