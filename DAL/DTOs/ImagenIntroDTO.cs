using System;
using System.Collections.Generic;

namespace DAL.DTOs
{
    public class ImagenIntroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public int Orden { get; set; }
        public string Foto { get; set; }
        public bool EsVideo { get; set; }

        public ImagenIntroDTO()
        {
            Titulo = string.Empty;
            Foto = string.Empty;
        }
    }

    public class ImagenIntroCreateDTO
    {
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public int Orden { get; set; }

        public ImagenIntroCreateDTO()
        {
            Titulo = string.Empty;
        }
    }

    public class ImagenIntroUpdateDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public int Orden { get; set; }

        public ImagenIntroUpdateDTO()
        {
            Titulo = string.Empty;
        }
    }

    public class ImagenIntroListadoDTO
    {
        public List<ImagenIntroDTO> Items { get; set; }
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int CantidadPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public string Buscar { get; set; }

        public ImagenIntroListadoDTO()
        {
            Items = new List<ImagenIntroDTO>();
            Buscar = string.Empty;
        }
    }

    public class ImagenIntroResponseDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public ImagenIntroDTO Data { get; set; }

        public ImagenIntroResponseDTO()
        {
            Mensaje = string.Empty;
            Data = new ImagenIntroDTO();
        }
    }

    public class ImagenIntroDeleteResponseDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public bool Eliminado { get; set; }

        public ImagenIntroDeleteResponseDTO()
        {
            Mensaje = string.Empty;
        }
    }
}