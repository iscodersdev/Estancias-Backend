using System;

namespace DAL.DTOs
{
    public class PuntosObtenidosClientesDTO
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }
        public string UserName { get; set; }

        public string NroDocumento { get; set; }
        public string NroTarjeta { get; set; }

        public long IdSolicitud { get; set; }
        public string IdOperacion { get; set; }

        public decimal MontoCompra { get; set; }
        public DateTime FechaCompra { get; set; }

        public string Compania { get; set; }
        public int CompaniaId { get; set; }

        public long PuntosObtenidos { get; set; }
        public long PuntosDisponibles { get; set; }

        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaProcesada { get; set; }
    }

    public class CrearPuntosObtenidosClientesDTO
    {
        public string UsuarioId { get; set; }
        public decimal MontoCompra { get; set; }
        public long PuntosObtenidos { get; set; }
        public string Motivo { get; set; }
        public string SearchTerm { get; set; }
    }
}