using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Provincia
    {
        public int Id { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionCompleta { get; set; }
    }

    public class Localidad
    {
        public int Id { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int IdDepartamento { get; set; }
        public int IdProvincia { get; set; }
        public string Descripcion { get; set; }
        public string ProvinciaNombre { get; set; }
    }

    public class DatosEstructura
    {
        public int Id { get; set; }
        public string Calle { get; set; }
        public string Sigla { get; set; }
        public string Numero { get; set; }
        public string CodigoPostal { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string CUIT { get; set; }
        public string Telefono { get; set; }
        public string FAX { get; set; }
        public string Sucursal { get; set; }
        public string CBU { get; set; }
        public string Alias { get; set; }
        public string Convenio { get; set; }
        public string Entidad { get; set; }
        public string NombreDependencia { get; set; }
        public string URLReportes { get; set; }
        public string UsuarioReportes { get; set; }
        public string CredencialReportes { get; set; }
        public string UsernameWS { get; set; }
        public string PasswordWS { get; set; }
    }


    public class Paises
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class TipoDocumento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Requerido"), Display(Name = "Tipo de Documento")]
        public string Descripcion { get; set; }
    }
}