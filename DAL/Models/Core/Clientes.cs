using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class TiposClientes
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int CantidadActividadesSemanales { get; set; }
    }

    public class Clientes 
    {
        public int Id { get; set; }
        public virtual Persona Persona { get; set; }
        public string UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual TiposClientes TipoCliente { get; set; }
        public virtual Clientes DependeDe { get; set; }
        public virtual Empresas Empresa { get; set; }
        public string RazonSocial { set; get; }
        public string NumeroCliente { get; set; }
        public string Domicilio { get; set; }
        public virtual Localidad Localidad { get; set; }
        public virtual Provincia Provincia { get; set; }
        public string CodigoPostal { get; set; }
        public string CBU { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaIngresoLaboral { get; set; }
        public string NumeroLegajoLaboral { get; set; }
        public string CategoriaLaboral { get; set; }
        public string DestinoLaboral { get; set; }
        public int NumeroAsociado { get; set; }
        public virtual Clientes Codeudor { get; set; }
        [Display(Name = "¿Es una persona politicamente expuesta?")]
        public bool PersonaPoliticamenteExpuesta { get; set; }
        [Display(Name = "¿Es Personal Militar?")]
        public bool EsMilitar { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaBaja { get; set; }
        public bool ClienteValidado { get; set; }
        [Display(Name = "¿Desea recibir Publicidad de la Mutual?")]
        public bool RecibirPublicidad { get; set; }
        public string NroDocReferido { get; set; }
        public byte[] FotoDNIAnverso { get; set; }
        public byte[] FotoDNIReverso { get; set; }
        public byte[] FotoSosteniendoDNI { get; set; }
        public byte[] LegajoElectronico { get; set; }
        public byte[] FirmaOlografica { get; set; }
        public byte[] FirmaOlograficaConfirmacion { get; set; }
        [Display(Name = "Referencia 1")]
        public virtual Referencia ReferenciaA { get; set; }
        [Display(Name = "Referencia 2")]
        public virtual Referencia ReferenciaB { get; set; }
        //public virtual DomicilioCliente Domicilio { get; set; }
        public string Password { get; set; }
        public bool RegistroMobile { get; set; }

    }
    public class DomicilioCliente
    {
        public int Id { get; set; }
        public virtual Provincia Provincia { get; set; }
        public virtual Localidad Localidad { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string CodigoPostal { get; set; }
    }
    public class Referencia
    {
        public int Id { get; set; }
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }
        [Display(Name = "Vínculo")]
        public string Vinculo { get; set; }
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }
    }

}