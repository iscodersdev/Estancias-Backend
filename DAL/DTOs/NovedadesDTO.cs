using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DAL.DTOs
{
    public class NovedadesDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Foto { get; set; }
        public string Texto { get; set; }
        public string TextoBoton { get; set; }
        public bool Publica { get; set; }

        public int? EmpresaId { get; set; }
        public int? ColorId { get; set; }
    }

    public class NovedadesListadoDTO
    {
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public List<NovedadesDTO> Novedades { get; set; }
    }

    public class NovedadesResponseDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public NovedadesDTO Novedad { get; set; }
    }

    public class CambiarImagenNovedadDTO
    {
        public int Id { get; set; }
        public IFormFile File { get; set; }
    }

    public class NotificacionNovedadResponseDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int CantidadEnviadas { get; set; }
    }
}