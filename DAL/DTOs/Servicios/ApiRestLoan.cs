using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Servicios
{
    // Se usa en casi todas las peticiones para la autenticación
    // Se usa en casi todas las peticiones para la autenticación.
    public class LoginInterface
    {
        public string Login { get; set; }
        public string Clave { get; set; }
        public string Token { get; set; }
    }

    // Objeto de resultado común en varias respuestas.
    public class Resultado
    {
        public int Result { get; set; }
        public string Mensaje { get; set; }
        public int CodigoError { get; set; }
    }

    // REQUEST para el endpoint de login de usuario.
    public class LoginUsuarioRequest
    {
        public LoginInterface LoginInterface { get; set; }
        public string Login { get; set; }
        public string Clave { get; set; }
    }

    // RESPONSE del endpoint de login de usuario.
    public class LoginUsuarioResponse
    {
        public ResultadoLogin Resultado { get; set; }
    }

    public class ResultadoLogin
    {
        public int Resultado { get; set; }
        public string Mensaje { get; set; }
        public int CodigoError { get; set; }
        public int IdPersona { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        // Puedes agregar aquí todos los demás campos que necesites de la respuesta.
    }

    // REQUEST para el endpoint de obtener créditos.
    // La documentación indica que el parámetro obligatorio es IdPersona.
    public class ObtenerCreditosRequest
    {
        public LoginInterface LoginInterface { get; set; }
        public int IdPersona { get; set; }
    }

    // RESPONSE del endpoint de obtener créditos.
    public class ObtenerCreditosResponse
    {
        public string Mensaje { get; set; }
        public int CodigoError { get; set; }
        public List<Credito> Credito { get; set; }
    }

    public class Credito
    {
        public int IdSolicitud { get; set; }
        public string Producto { get; set; }
        public string Fecha { get; set; }
        public string Operacion { get; set; }
        public string Estado { get; set; }
        public string EstadoToolTip { get; set; }
        public string ImporteCredito { get; set; }
        public string CapitalPedido { get; set; }
        public string ImporteCuota { get; set; }
        public string CantidadCuotas { get; set; }
        public string ImporteGastos { get; set; }
        public string ImporteInteres { get; set; }
        public string ImporteImpuestos { get; set; }
    }

    public class ObtenerPersonaRequest
    {
        public LoginInterface LoginInterface { get; set; }

        // Usamos tipos que acepten nulos para los parámetros opcionales.
        public int? IdPersona { get; set; }
        public string Documento { get; set; }
        public string Nombre { get; set; }
        public SexoRequest Sexo { get; set; }
    }

    public class SexoRequest
    {
        public int Id { get; set; }
    }

    // RESPONSE de POST api/ecommerce/obtenerpersona
    public class ObtenerPersonaResponse
    {
        public Resultado Resultado { get; set; }
        public PersonaLoanDTO Persona { get; set; }
    }

    // Modelo para el objeto Persona (es grande, aquí están los campos clave)
    public class PersonaLoanDTO
    {
        // ¡Este es el ID que necesitas para las otras llamadas!
        public string Id { get; set; }

        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Documento { get; set; }
        public Sexo Sexo { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public string FechaNacimiento { get; set; }
        public string CBU { get; set; }

        // Objeto anidado para el domicilio
        public Domicilio Domicilio { get; set; }

        // NOTA: La respuesta de la API es muy extensa.
        // Agrega aquí el resto de las propiedades (EstadoCivil, Empleo, Referencias, etc.)
        // que necesites de la misma manera.
    }

    // Clases para los objetos anidados dentro de Persona
    public class Sexo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class TipoDocumento
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class Domicilio
    {
        public int Id { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Localidad { get; set; }
        // Agrega más campos del domicilio si son necesarios...
    }
}
