using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Servicios
    {
        public int Id { get; set; }
        public virtual Empresas Empresa { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string Nombre { get; set; }
        public int TopeAnualFinde { get; set; }
        public int TopeMensualFinde { get; set; }
        public int TopePendienteFinde { get; set; }
        public int TopeAnualSemana { get; set; }
        public int TopeMensualSemana { get; set; }
        public int TopePendienteSemana { get; set; }
        public int DiasProgramacion { get; set; }
        public int TopePendienteGlobal { get; set; }
        public int Cupo { get; set; }
        public byte[] Icono { get; set; }
        public virtual List<Horarios> Horario { get; set; } = new List<Horarios>();
        public virtual List<ClientesServicios> TiposSocios { get; set; } = new List<ClientesServicios>();
        public DateTime? FechaUnica { get; set; }
        public string Observaciones { get; set; }
        public void CambiarImagen(byte[] bytes)
        {
            Icono = bytes;
        }
        public virtual TipoServicio Tipo { get; set; }
    }
    public class TipoServicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class ClientesServicios
    {
        public int Id { get; set; }
        public virtual TiposClientes TipoCliente { get; set; }
        public virtual Servicios Servicio { get; set; }
        public int TopeMensual { get; set; }
        public int TopeSemanal { get; set; }
    }
    public class Horarios
    {
        public int Id { get; set; }
        public virtual Servicios Servicio { get; set; }
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public int DiaSemana { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaBaja { get; set; }
}

    public class DiasSemana
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }   
}
