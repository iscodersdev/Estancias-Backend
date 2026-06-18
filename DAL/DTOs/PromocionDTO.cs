using DAL.Models;
using System;
using System.Collections.Generic;

namespace DAL.DTOs
{
    public class PromocionDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Texto { get; set; }
        public string TextoBoton { get; set; }
        public string Link { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public bool Publica { get; set; }
        public bool Vencimiento { get; set; }
        public bool PromocionFija { get; set; }
        public bool QR { get; set; }

        public int Orden { get; set; }

        public string Foto { get; set; }

        public int? EmpresaId { get; set; }

        public string Estado { get; set; }
        public Marcas Marca { get; set; }
    }

    public class PromocionCreateDTO
    {
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Texto { get; set; }
        public string TextoBoton { get; set; }
        public string Link { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public bool Publica { get; set; }

        public bool TieneFechaVencimiento { get; set; }

        public int PromocionFija { get; set; }

        public int Orden { get; set; }

        public int ColorId { get; set; }
        public int MarcaId { get; set; }
    }

    public class PromocionUpdateDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Texto { get; set; }
        public string TextoBoton { get; set; }
        public string Link { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public bool Publica { get; set; }

        public bool TieneFechaVencimiento { get; set; }

        public int PromocionFija { get; set; }

        public int ColorId { get; set; }
        public int MarcaId { get; set; }
    }

    public class PromocionCambiarOrdenDTO
    {
        public int Id { get; set; }
        public int Orden { get; set; }
    }

    public class PromocionQRDTO
    {
        public int Id { get; set; }
        public int PromocionId { get; set; }
        public string Hash { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaUtilizado { get; set; }
    }

    public class PromocionListadoResponseDTO
    {
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int CantidadPorPagina { get; set; }
        public int TotalPaginas { get; set; }

        public List<PromocionDTO> Promociones { get; set; }
    }

    public class ValidarPromocionResponseDTO
    {
        public bool EsActivo { get; set; }
        public string Texto { get; set; }
    }
}