using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MTraePrestamosDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<PrestamoDTO> Prestamos { get; set; }
    }

    public class PrestamoDTO
    {
        public int Id { get; set; }
        public string Estado { get; set; }
        public string Producto { get; set; }
        public DateTime Fecha { get; set; }
        public decimal MontoPrestado { get; set; }
        public decimal Saldo { get; set; }
        public decimal CuotasRestantes { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public string Color { get; set; }
        public bool Anulable { get; set; }
        public bool Confirmable { get; set; }
        public bool Transferido { get; set; }
        public decimal CFT { get; set; }
    }
    public class MTraePrestamosRenglonesDTO
    {
        public string UAT { get; set; }
        public int PrestamoId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<PrestamoRenglonDTO> Renglones { get; set; }
    }

    public class PrestamoRenglonDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Concepto { get; set; }
        public decimal Credito { get; set; }
        public decimal Debito { get; set; }
        public decimal Saldo { get; set; }
    }

    public class MTraeLineasPrestamosDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<LineasPrestamoDTO> LineasPrestamos { get; set; }
    }

    public class LineasPrestamoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal MontoMinimo { get; set; }
        public decimal MontoMaximo { get; set; }
        public decimal Intervalo { get; set; }
    }

    public class MSolicitaPrestamoCGEDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int EntidadId { get; set; }
        public Int64 DNI { get; set; }
        public decimal ImporteSolicitado { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public int TipoPrestamoId { get; set; }
        public byte[] FotoDNIAnverso { get; set; }
        public byte[] FotoDNIReverso { get; set; }
        public byte[] FotoSosteniendoDNI { get; set; }
        public byte[] LegajoElectronico { get; set; }
        public int PrestamoCGEId { get; set; }
        public byte[] FirmaOlografica { get; set; }
        public byte[] LegajoEntidad { get; set; }
        public List<MPrecancelacionesDTO> Precancelaciones { get; set; }
    }

    public class MSolicitaPrestamoDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int LineaPrestamoId { get; set; }
        public decimal ImporteSolicitado { get; set; }
        public int TipoPersonaId { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public byte[] FotoDNIAnverso { get; set; }
        public byte[] FotoDNIReverso { get; set; }
        public byte[] FotoSosteniendoDNI { get; set; }
        public byte[] FotoReciboHaber { get; set; }
        public byte[] FotoCertificadoDescuento { get; set; }
        public byte[] LegajoElectronico { get; set; }
        public byte[] FirmaOlografica { get; set; }
        public List<MPrecancelacionesDTO> Precancelaciones { get; set; }
    }
    public class MTraePrecancelacionesDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public Int64 DNI { get; set; }
        public List<MPrecancelacionesDTO> Precancelaciones { get; set; }
    }
    public class MPrecancelacionesDTO
    {
        public int PrestamoId { get; set; }
        public string Entidad { get; set; }
        public string NumeroConvenio { get; set; }
        public decimal ImporteCuota { get; set; }
        public bool Precancelar { get; set; }
    }
    public class MActualizaLegajoEntidadDTO
    {
        public string UAT { get; set; }
        public int PrestamoCGEId { get; set; }
        public byte[] LegajoEntidad { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class MLoginEntidadesDTO
    {
        public Int64 CUIT { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class MEstadoPrestamoDTO
    {
        public string UAT { get; set; }
        public int PrestamoCGEId { get; set; }
        public int EstadoId { get; set; }
        public decimal Capital { get; set; }
        public decimal MontoCuota { get; set; }
        public int CantidadCuotas { get; set; }
        public DateTime? FechaAnulado { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class MDatosPersonaDTO
    {
        public Int64 CUIL { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public int DNI { get; set; }
        public int NOU { get; set; }
        public string eMail { get; set; }
        public string Celular { get; set; }
        public string Categoria { get; set; }
        public string Unidad { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaIngreso { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string UAT { get; set; }
        public int Token { get; set; }
        public string TipoPersona { get; set; }
    }
    public class MTraeLegajoElectronicoDTO
    {
        public string UAT { get; set; }
        public int PrestamoId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public string LegajoElectronico { get; set; }
    }
    public class MConfirmarPrestamoDTO
    {
        public string UAT { get; set; }
        public int PrestamoId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int Token { get; set; }
        public byte[] FirmaOlografica { get; set; }
    }
    public class MTraePlanesDisponiblesDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int LineaId { get; set; }
        public decimal ImporteDeseado { get; set; }
        public List<PlanesDisponiblesDTO> PlanesDisponibles { get; set; }
    }

    public class PlanesDisponiblesDTO
    {
        public int Id { get; set; }
        public decimal MontoPrestado { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public decimal CFT { get; set; }
    }
    public class MTraeDsiponibleCGEDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int DNI { get; set; }
        public decimal Disponible { get; set; }
    }
    public class MSolicitaTokenDTO
    {
        public string UAT { get; set; }
        public int PrestamoId { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
    }
    public class MEnviaTokenPrestamoDTOCGE
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int PrestamoId { get; set; }
    }
    public class MEnviaOpcionesConfirmadasDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int PrestamoId { get; set; }
        public int CantidadCuotas { get; set; }
        public Decimal ImporteCuota { get; set; }
        public Decimal ImportePrestado { get; set; }
        public Decimal ImporteCuotaSocial { get; set; }
        public int Token { get; set; }
        public byte[] FirmaOlografica { get; set; }
        public byte[] LegajoEntidad { get; set; }
    }
}
