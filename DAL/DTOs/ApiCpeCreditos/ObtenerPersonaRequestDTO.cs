using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.ApiCpeCreditos
{

    /** DTO para la solicitud de obtener persona desde la API de CPE Creditos.**/
    public class ObtenerPersonaRequestDTO
    {
        public LoginInterface LoginInterface { get; set; }
        public string Documento { get; set; }
    }
    public class ObtenerCreditosRequestDTO
    {
        public LoginInterface LoginInterface { get; set; }
        public int IdPersona { get; set; }
    }
    public class ObtenerCreditosDetallesRequestDTO
    {
        public LoginInterface LoginInterface { get; set; }
        public int IdSolicitud { get; set; }
    }
    public class ObteneOperacionDetallesRequestDTO
    {
        public LoginServicio LoginServicio { get; set; }
        public string numeroOperacion { get; set; }
    }

    public class LoginInterface
    {
        public string Login { get; set; }
        public string Clave { get; set; }
    }
    public class LoginServicio
    {
        public string Login { get; set; }
        public string Clave { get; set; }
    }


    /* DTO para las Respuestas de obtener persona desde la API de CPE Creditos.*/
    /*--------------------Api Nueva-----------------------*/


    public class ResponseObtenerDatosPersonaDTO
    {
        public ResultadoInfo Resultado { get; set; }
        public Persona Persona { get; set; }
    }

    public class ResultadoInfo
    {
        public int Resultado { get; set; }
        public string Mensaje { get; set; }
        public int CodigoError { get; set; }
    }

    public class Persona
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Cuil1 { get; set; }
        public string Cuil2 { get; set; }
        public Lookup Sexo { get; set; }
        public string FechaNacimiento { get; set; } // Puede ser DateTime si el formato es estándar
        public Lookup TipoDocumento { get; set; }
        public long Documento { get; set; }
        public Lookup Nacionalidad { get; set; }
        public Lookup PaisNacimiento { get; set; }
        public Lookup EstadoCivil { get; set; }
        public string CBU { get; set; }
        public decimal IngresoMensual { get; set; }
        public bool SujetoObligado { get; set; }
        public bool PersonaExpuestaPoliticamente { get; set; }
        public bool EmpleadoBacs { get; set; }
        public Lookup ActividadAfip { get; set; }
        public Domicilio Domicilio { get; set; }
        public Domicilio DomicilioLegal { get; set; }
        public Domicilio DomicilioLaboral { get; set; }
        public List<Referencia> Referencias { get; set; }
        public Lookup PaisResidenciaFiscal { get; set; }
        public long IdTributariaPaisResidenciaFiscal { get; set; }
        public bool UsPerson { get; set; }
        public bool AceptaRecibirInformacion { get; set; }
        public Empleo InfoEmpleo { get; set; }
        public string Puesto { get; set; }
        public string FechaIngreso { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public Lookup Ramo { get; set; }
        public Lookup ConyugeTipoDocumento { get; set; }
        public long ConyugeDocumento { get; set; }
        public string ConyugeNombre { get; set; }
        public string ConyugeApellido { get; set; }
        public string IdEmpresa { get; set; }
        public Lookup TipoDiaHabilCobro { get; set; }
        public Banco Banco { get; set; }
    }

    public class Domicilio
    {
        public int Id { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Piso { get; set; }
        public string Entre { get; set; }
        public string Barrio { get; set; }
        public string CodigoPostal { get; set; }
        public string PlanoFilcar { get; set; }
        public string CoordenadaFilcar { get; set; }
        public string Telefono { get; set; }
        public string TelefonoCelular { get; set; }
        public Localidad Localidad { get; set; }
        public string LocalidadAnterior { get; set; }
        public string TelefonoCodigoArea { get; set; }
        public string TelefonoCaracteristica { get; set; }
        public string TelefonoNumero { get; set; }
        public string TelefonoReferenciaCodigoArea { get; set; }
        public string TelefonoReferenciaCaracteristica { get; set; }
        public string TelefonoReferenciaNumero { get; set; }
        public string TelefonoReferencia { get; set; }
        public string TelefonoCelularCodigoArea { get; set; }
        public string TelefonoCelularCaracteristica { get; set; }
        public string TelefonoCelularNumero { get; set; }
        public CalleBase CalleBase { get; set; }
        public string Departamento { get; set; }
        public string Manzana { get; set; }
        public string Edificio { get; set; }
        public string Monoblock { get; set; }
        public string Escaleras { get; set; }
        public string Pasillo { get; set; }
        public string Casa { get; set; }
        public string Lote { get; set; }
        public string Comentarios { get; set; }
        public Vivienda Vivienda { get; set; }
    }

    public class Localidad
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public Partido Partido { get; set; }
        public string CodigoPostal { get; set; }
        public bool Asignada { get; set; }
    }

    public class Partido
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public Provincia Provincia { get; set; }
    }

    public class Provincia
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public int CodigoCabal { get; set; }
        public decimal PorcentajeSello { get; set; }
        public decimal MontoMinimoSello { get; set; }
        public decimal MontoFijoSello { get; set; }
        public bool ImprimePagare { get; set; }
    }

    public class CalleBase
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int Numero { get; set; }
        public Localidad Localidad { get; set; }
        public Partido Partido { get; set; }
        public Provincia Provincia { get; set; }
        public string Mail { get; set; }
        public string CPA { get; set; }
    }

    public class Referencia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Vinculo { get; set; }
        public bool TelefonoCelular { get; set; }
        public string TelefonoCodigoArea { get; set; }
        public string TelefonoCaracteristica { get; set; }
        public string TelefonoNumero { get; set; }
        public string Telefono { get; set; }
        public long Documento { get; set; }
        public Lookup TipoDocumento { get; set; }
    }

    public class Empleo
    {
        public int Id { get; set; }
        public Lookup TipoEmpleo { get; set; }
    }

    public class Banco : Lookup
    {
        public string CodigoBCRA { get; set; }
    }

    public class Vivienda : Lookup
    {
        public int Codigo { get; set; }
    }

    // Clase genérica para objetos con Id y Descripcion
    public class Lookup
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }


    /*--------------------------------------------*/

    public class ResponseObtenerCreditosDTO
    {
        public ResultadoInfo Resultado { get; set; }
        public List<Credito> Credito { get; set; }
    }

    public class Credito
    {
        public int IdSolicitud { get; set; }
        public string Producto { get; set; }
        public string Fecha { get; set; }
        public string FechaCobro { get; set; }
        public string Operacion { get; set; }
        public string Estado { get; set; }
        public string EstadoToolTip { get; set; }

        // Vienen como string en el JSON
        public string ImporteCredito { get; set; }
        public string CapitalPedido { get; set; }
        public string ImporteCuota { get; set; }
        public string CantidadCuotas { get; set; }
        public string ImporteGastos { get; set; }
        public string ImporteInteres { get; set; }
        public string ImporteImpuestos { get; set; }

        public List<DatoAnexo> DatosAnexo { get; set; }

        public string Tna { get; set; }
        public string Tea { get; set; }
        public string FechaProximoVencimiento { get; set; }
        public string Pendiente { get; set; }
        public string Comercio { get; set; }
        public string FechaUltimoPago { get; set; }
    }

    /*--------------------------------------------*/


    public class ResponseObtenerCreditosDetallesDTO
    {
        public ResultadoInfo Resultado { get; set; }
        public List<CreditoDetalleDTO> CreditoDetalles { get; set; }
    }

    public class CreditoDetalleDTO
    {
        public long Id { get; set; }
        public string Fecha { get; set; }
        public string Cuota { get; set; }
        public string Estado { get; set; }
        public string ImporteCuota { get; set; }
        public string ImportePunitorios { get; set; }
        public int idTipoEntidad { get; set; }
    }



    /*--------------------------------------------*/

    public class ResultadoServicioWeb
    {
        public int Resultado { get; set; }
        public string Mensaje { get; set; }
    }

    public class ResponseObtenerOperacionDetallesDTO
    {
        public ResultadoServicioWeb ResultadoServicioWeb { get; set; }
        public string EstadoOperacion { get; set; }
        public DateTime? FechaEstadoOperacion { get; set; }
        public decimal? MontoPromesa { get; set; }
        public DateTime? FechaPromesa { get; set; }
        public string UsuarioPromesa { get; set; }
        public DateTime? FechaUltimoTramite { get; set; }
        public DateTime? FechaProximaAccion { get; set; }
        public string Estudio { get; set; }
        public DateTime? FechaEstudio { get; set; }
        public string Compania { get; set; }
        public string CodigoCompania { get; set; }
        public string CodigoEstadoOperacion { get; set; }
    }



    public class DatoAnexo
    {
        public string TipoDatoAnexo { get; set; }
        public string Valor { get; set; }
    }
}

