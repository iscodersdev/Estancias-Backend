namespace DAL.DTOs
{
    public class EmpresaDTO
    {
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Grupo { get; set; }
    }

    public class EmpresaDetalleDTO
    {
        public int Id { get; set; }
        public string CUIT { get; set; }
        public int? GrupoId { get; set; }
        public string Grupo { get; set; }
        public string RazonSocial { get; set; }
        public string Abreviatura { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public string ColorFontCarnet { get; set; }
        public string ColorCarnet { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string WhatsApp { get; set; }
        public string ColorFondo { get; set; }
        public string ColorBotones { get; set; }
        public string ColorLogin { get; set; }
    }

    public class EmpresaCreateRequest
    {
        public long CUIT { get; set; }
        public int GrupoId { get; set; }
        public string RazonSocial { get; set; }
        public string Abreviatura { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public string ColorFontCarnet { get; set; }
        public string ColorCarnet { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string WhatsApp { get; set; }
        public string ColorFondo { get; set; }
        public string ColorBotones { get; set; }
        public string ColorLogin { get; set; }
    }

    public class EmpresaUpdateRequest
    {
        public long CUIT { get; set; }
        public int GrupoId { get; set; }
        public string RazonSocial { get; set; }
        public string Abreviatura { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public string ColorFontCarnet { get; set; }
        public string ColorCarnet { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string WhatsApp { get; set; }
        public string ColorFondo { get; set; }
        public string ColorBotones { get; set; }
        public string ColorLogin { get; set; }
    }
}