using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MRecuperaPasswordDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string email { get; set; }
        public int NumeroDocumento { get; set; }
    }
    public class MLoginDTO
    {
        public Int64 NumeroDocumento { get; set; }
        public bool Recordarme { get; set; }
        public string Password { get; set; }
        public string UAT { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string Categoria { get; set; }
        public string Unidad { get; set; }
        public string DeviceId { get; set; }
        public string DeviceToken { get; set; }
        public string Mail { get; set; }
        public string MailOculto { get; set; }
        public string Celular { get; set; }
        public byte[] Foto { get; set; }
        public byte[] LogoMutual { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string NumeroCliente { get; set; }
        public string ColorCarnet { get; set; }
        public string ColorFontCarnet { get; set; }
        public bool PrimerIngreso { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public byte[] FondoMutual { get; set; }
        public int EmpresaId { get; set; }
        public int? ProveedorId { get; set; }
        public int? ClienteId { get; set; }
        public int? LocalidadId { get; set; }
        public string LocalidadDescripcion { get; set; }
        public string Domicilio { get; set; }
        public string NroTarjeta { get; set; }
        public bool Administrador { get; set; }
        public bool verificado { get; set; }
    }
    public class MRegistraPersonaDTO
    {
        public int NumeroDocumento { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string FechaNacimiento { get; set; }
        public string Mail { get; set; }
        public string Celular { get; set; }
        public int EmpresaId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int? Token { get; set; }
        public string Referido { get; set; }
        public int LocalidadId { get; set; }
        public string NroTarjeta { get; set; }
    }
    public class MValidarPasswordDTO
    {
        public int  NumeroDocumento { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public string eMail { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int Token { get; set; }
        
    }
    public class MTraeDatosUsuarioDTO
    {
        public string UAT { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string Categoria { get; set; }
        public string Unidad { get; set; }
        public string DeviceId { get; set; }
        public string Mail { get; set; }
        public string Celular { get; set; }
        public byte[] Foto { get; set; }
        public byte[] FondoMobile { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string NumeroCliente { get; set; }
        public byte[] LogoMutual { get; set; }
        public string ColorCarnet { get; set; }
        public string ColorFontCarnet { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string WhatsApp { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Domicilio { get; set; }
    }
    public class MPreLogin20DTO
    {
        public string eMail { get; set; }
        public string MailOculto { get; set; }
        public bool Recordarme { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public bool NecesitaRegistro { get; set; }
        public bool verificado { get; set; }
        public List<MMutualesDTO> Mutuales { get; set; }
    }
    public class MMutualesDTO
    {
        public int ClienteId { get; set; }
        public string NombrEstancias { get; set; }
        public string ColorFondo { get; set; }
        public string ColorBotones { get; set; }
        public string ColorLogin { get; set; }
        public byte[] GIFLogoMutual { get; set; }
        public byte[] ImagenLogin { get; set; }
        public byte[] LogoMutual { get; set; }
    }
    public class MPreLoginDTO
    {
        public string NumeroDocumento { get; set; }
        public bool Recordarme { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public string NombrEstancias { get; set; }
        public string ColorFondo { get; set; }
        public string ColorBotones { get; set; }
        public string ColorLogin { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public byte[] GIFLogoMutual { get; set; }
        public byte[] ImagenLogin { get; set; }
        public byte[] LogoMutual { get; set; }
        public bool NecesitaRegistro { get; set; }
    }
    public class MTraeCredencialesDTO
    {
        public string UAT { get; set; }
        public string Unidad { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public byte[] LogoClub { get; set; }
        public string ColorCarnet { get; set; }
        public string ColorFontCarnet { get; set; }
        public List<CredencialDTO> Credenciales { get; set; }
    }

    public class CredencialDTO
    {
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string TipoCliente { get; set; }
        public string NumeroCliente { get; set; }
        public string NumeroDocumento { get; set; }
        public byte[] Foto { get; set; }
    }
    public class MActualizaDatosPersonaDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string Mail { get; set; }
        public string Celular { get; set; }
        public string Domicilio { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }

    }
    public class MActualizaDatosLocalidadDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int LocalidadId  { get; set; }

    }
    public class MActualizaFotoDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public byte[] Foto { get; set; }
    }

    public class SociosDTO
    {
        public int Id { get; set; }
        public string NumeroSocio { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string NumeroDocumento { get; set; }
        public string TipoSocio { get; set; }
        public string Celular { get; set; }
    }
    public class MPreRegistroDTO
    {
        public int NumeroDocumento { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string Celular { get; set; }
        public string eMail { get; set; }
        public string MailOculto { get; set; }
        public int FormularioRegistro { get; set; }
        public int Token { get; set; }
        public string NroTarjeta { get; set; }
    }

    public class MNroTarjetaDTO
    {
        public int NumeroDocumento { get; set; }
        public string NroTarjeta { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string UAT { get; set; }
        public string CodUnit { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
    }

    public class MVerificarDocumentoDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string UAT { get; set; }
        public bool TieneDocumento { get; set; }
        public string NumeroDocumento { get; set; }
    }
    public class MWonderPushDTO
    {
        public string UAT { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
}
