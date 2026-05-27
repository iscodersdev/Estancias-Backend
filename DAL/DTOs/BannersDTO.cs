using System;
using System.Collections.Generic;

namespace DAL.DTOs
{
    public class BannersListadoDTO : RequestApi
    {
        public List<BannersDTO> Banners { get; set; }
        public int TotalRegistros { get; set; }
        public int Pagina { get; set; }
        public int CantidadPorPagina { get; set; }

        public BannersListadoDTO()
        {
            Banners = new List<BannersDTO>();
            TotalRegistros = 0;
            Pagina = 1;
            CantidadPorPagina = 10;
        }
    }

    public class BannerResponseDTO : RequestApi
    {
        public BannersDTO Banner { get; set; }

        public BannerResponseDTO()
        {
            Banner = new BannersDTO();
        }
    }

    public class BannersDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Texto { get; set; }
        public string TextoBoton { get; set; }

        public DateTime Fecha { get; set; }
        public DateTime FechaDesde { get; set; }

        /*
            En el modelo original FechaHasta puede ser null.
            Para evitar usar DateTime? en el DTO, se manda como string.
            Si no tiene vencimiento, vuelve vacío.
        */
        public string FechaHasta { get; set; }

        public bool Publico { get; set; }
        public bool LinkExterno { get; set; }
        public bool BannerFijo { get; set; }
        public bool Vencimiento { get; set; }
        public bool EsVideo { get; set; }

        public string Link { get; set; }
        public string Video { get; set; }
        public string Foto { get; set; }

        public int Orden { get; set; }

        public BannersDTO()
        {
            Titulo = "";
            Subtitulo = "";
            Texto = "";
            TextoBoton = "";
            FechaHasta = "";
            Link = "";
            Video = "";
            Foto = "";
            Fecha = new DateTime();
            FechaDesde = new DateTime();
            Publico = false;
            LinkExterno = false;
            BannerFijo = false;
            Vencimiento = false;
            EsVideo = false;
            Orden = 0;
        }
    }

    public class BannerCrearDTO
    {
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Texto { get; set; }
        public string TextoBoton { get; set; }

        public DateTime Fecha { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public bool Publico { get; set; }
        public bool LinkExterno { get; set; }
        public bool BannerFijo { get; set; }
        public bool TieneFechaVencimiento { get; set; }

        public string Link { get; set; }

        public int Orden { get; set; }

        public BannerCrearDTO()
        {
            Titulo = "";
            Subtitulo = "";
            Texto = "";
            TextoBoton = "";
            Link = "";
            Fecha = new DateTime();
            FechaDesde = new DateTime();
            FechaHasta = new DateTime();
            Publico = false;
            LinkExterno = false;
            BannerFijo = false;
            TieneFechaVencimiento = false;
            Orden = 0;
        }
    }

    public class BannerEditarDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string Texto { get; set; }
        public string TextoBoton { get; set; }

        public DateTime Fecha { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public bool Publico { get; set; }
        public bool LinkExterno { get; set; }
        public bool BannerFijo { get; set; }
        public bool TieneFechaVencimiento { get; set; }

        public string Link { get; set; }

        public int Orden { get; set; }

        public BannerEditarDTO()
        {
            Titulo = "";
            Subtitulo = "";
            Texto = "";
            TextoBoton = "";
            Link = "";
            Fecha = new DateTime();
            FechaDesde = new DateTime();
            FechaHasta = new DateTime();
            Publico = false;
            LinkExterno = false;
            BannerFijo = false;
            TieneFechaVencimiento = false;
            Orden = 0;
        }
    }

    public class BannerOrdenDTO
    {
        public int Id { get; set; }
        public int Orden { get; set; }
    }
}