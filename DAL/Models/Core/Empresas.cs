using Commons.Identity;
using Commons.Models;
using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class Grupos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class Empresas
    {
        public int Id { get; set; }
        public Int64 CUIT { set; get; }
        public int EntidadIdCGE { get; set; }
        public string TokenCGE { get; set; }
        public string PasswordCGE { get; set; }
        public string RazonSocial { set; get; }
        public string Domicilio { set; get; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
        public string ColorFontCarnet { get; set; }
        public string ColorCarnet { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string WhatsApp { get; set; }
        public byte[] FondoMobile { get; set; }
        public byte[] GIFLogoMutual { get; set; }
        public byte[] LogoMutual { get; set; }
        public byte[] ImagenLogin { get; set; }
        public virtual Grupos Grupo { get; set; }
        public string Abreviatura { get; set; }
        public string ColorFondo { get; set; }
        public string ColorBotones { get; set; }
        public string ColorLogin { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string UrlMail { get; set; }
        public string UsernameMail { get; set; }
        public string PasswordMail { get; set; }
        public int PuertoMail { get; set; }
        public bool SSLMail { get; set; }
        public string CasillaMail { get; set; }
        public bool MailBienvenida { get; set; }
        public byte[] CabeceraMail { get; set; }

    }
    public class Sucursales
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string group { get; set; }

    }
    
}