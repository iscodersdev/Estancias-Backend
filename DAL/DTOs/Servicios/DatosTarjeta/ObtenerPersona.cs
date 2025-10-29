using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Servicios.DatosTarjeta
{
      

    public class LoginInterface
    {
        public string Login { get; set; }
        public string Clave { get; set; }
        public string Token { get; set; }
    }

    public class ProvinciaLOAN
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public int CodigoCabal { get; set; }
        public double PorcentajeSello { get; set; }
        public double MontoMinimoSello { get; set; }
        public double MontoFijoSello { get; set; }
        public bool ImprimePagare { get; set; }
    }

    public class PartidoLOAN
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public ProvinciaLOAN Provincia { get; set; }
    }

    public class LocalidadLOAN
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public PartidoLOAN Partido { get; set; }
        public string CodigoPostal { get; set; }
        public bool Asignada { get; set; }
    }

    public class EntidadSimpleLOAN
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class CalleBaseLOAN
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public LocalidadLOAN Localidad { get; set; }
        public PartidoLOAN Partido { get; set; }
        public ProvinciaLOAN Provincia { get; set; }
    }

    public class ViviendaLOAN
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Descripcion { get; set; }
    }

    public class DomicilioLOAN
    {
        public int Id { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Piso { get; set; }
        public string Entre { get; set; }
        public string Barrio { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string TelefonoCelular { get; set; }
        public LocalidadLOAN Localidad { get; set; }
        public CalleBaseLOAN CalleBase { get; set; }
        public string Departamento { get; set; }
        public string Comentarios { get; set; }
        public ViviendaLOAN Vivienda { get; set; }
    }

    public class ReferenciaLOAN
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Vinculo { get; set; }
        public string Telefono { get; set; }
        public int Documento { get; set; }
        public EntidadSimpleLOAN TipoDocumento { get; set; }
    }

    public class EmpleoLOAN
    {
        public int Id { get; set; }
        public EntidadSimpleLOAN TipoEmpleo { get; set; }
    }

    // ---------------------------------
    // Clases Principales
    // ---------------------------------

    public class PersonaLOAN
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Cuil1 { get; set; }
        public string Cuil2 { get; set; }
        public EntidadSimpleLOAN Sexo { get; set; }
        public string FechaNacimiento { get; set; }
        public EntidadSimpleLOAN TipoDocumento { get; set; }
        public int Documento { get; set; }
        public EntidadSimpleLOAN Nacionalidad { get; set; }
        public string CBU { get; set; }
        public int IngresoMensual { get; set; }
        public bool SujetoObligado { get; set; }
        public bool PersonaExpuestaPoliticamente { get; set; }
        public DomicilioLOAN Domicilio { get; set; }
        public DomicilioLOAN DomicilioLegal { get; set; }
        public DomicilioLOAN DomicilioLaboral { get; set; }
        public List<ReferenciaLOAN> Referencias { get; set; }
        public EmpleoLOAN Empleo { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public object Banco { get; set; }
        public string GetDireccion() { 
            return $"{Domicilio.Calle} {Domicilio.Numero}, {Domicilio.Localidad.Descripcion}";
        }
    }

    public class ResultadoInfo
    {
        public int Resultado { get; set; }
        public string Mensaje { get; set; }
        public int CodigoError { get; set; }
    }

    // ---------------------------------
    // Clase Raíz para la Respuesta Completa
    // ---------------------------------

    public class ApiResponseObtenerPersonaDTO
    {
        public ResultadoInfo Resultado { get; set; }
        public PersonaLOAN Persona { get; set; }
    }
}
