using DAL.Models;
using System;
using System.Collections.Generic;

namespace DAL.DTOs
{
    public class CatalogoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public Marcas Marca { get; set; }
    }

    public class CatalogoCreateDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public int? MarcaId { get; set; }
    }

    public class CatalogoUpdateDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public int? MarcaId { get; set; }
    }

    public class CatalogoListadoDTO
    {
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int CantidadPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public List<CatalogoDTO> Catalogos { get; set; } = new List<CatalogoDTO>();
    }

    public class CatalogoResponseDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class CatalogoItemResponseDTO : CatalogoResponseDTO
    {
        public CatalogoDTO Catalogo { get; set; } = new CatalogoDTO();
    }

    public class CatalogoListadoResponseDTO : CatalogoResponseDTO
    {
        public CatalogoListadoDTO Data { get; set; } = new CatalogoListadoDTO();
    }
}