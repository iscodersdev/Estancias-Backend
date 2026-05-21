using System;

namespace DAL.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string NombreCompleto { get; set; }
        public string CUIL { get; set; }
        public string RazonSocial { get; set; }
        public string Empresa { get; set; }
        public string FechaIngreso { get; set; }
        public bool Estado { get; set; }
    }

    public class ClienteDetalleDTO
    {
        public int Id { get; set; }

        public int? TipoClienteId { get; set; }
        public string TipoClienteNombre { get; set; }

        public int? PersonaId { get; set; }
        public string NombreCompleto { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string NroDocumento { get; set; }
        public string CUIL { get; set; }

        public string UsuarioId { get; set; }
        public string UsuarioEmail { get; set; }

        public int? EmpresaId { get; set; }
        public string Empresa { get; set; }

        public string RazonSocial { get; set; }
        public string NumeroCliente { get; set; }
        public string Domicilio { get; set; }

        public int? ProvinciaId { get; set; }
        public string Provincia { get; set; }

        public int? LocalidadId { get; set; }
        public string Localidad { get; set; }

        public string CodigoPostal { get; set; }
        public string CBU { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }

        public DateTime FechaIngresoLaboral { get; set; }
        public string NumeroLegajoLaboral { get; set; }
        public string CategoriaLaboral { get; set; }
        public string DestinoLaboral { get; set; }
        public int NumeroAsociado { get; set; }

        public int? DependeDeId { get; set; }
        public string DependeDeNombre { get; set; }

        public int? CodeudorId { get; set; }
        public string CodeudorNombre { get; set; }

        public bool PersonaPoliticamenteExpuesta { get; set; }
        public bool EsMilitar { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaBaja { get; set; }
        public bool ClienteValidado { get; set; }
        public bool RecibirPublicidad { get; set; }
        public string NroDocReferido { get; set; }
        public bool RegistroMobile { get; set; }

        public bool TieneFotoDNIAnverso { get; set; }
        public bool TieneFotoDNIReverso { get; set; }
        public bool TieneFotoSosteniendoDNI { get; set; }
        public bool TieneLegajoElectronico { get; set; }
        public bool TieneFirmaOlografica { get; set; }
        public bool TieneFirmaOlograficaConfirmacion { get; set; }
    }

    public class ClienteCreateRequest
    {
        public int TipoClienteId { get; set; }
        public int TipoDocumentoId { get; set; }
        public int PaisId { get; set; }
        public int EmpresaId { get; set; }

        public int? ProvinciaId { get; set; }
        public int? LocalidadId { get; set; }
        public int? DependeDeId { get; set; }
        public int? CodeudorId { get; set; }

        public string Mail { get; set; }

        public string NroDocumento { get; set; }
        public string CUIL { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int CantidadHijos { get; set; }

        public string RazonSocial { get; set; }
        public string NumeroCliente { get; set; }
        public string Domicilio { get; set; }
        public string CodigoPostal { get; set; }
        public string CBU { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }

        public DateTime FechaIngreso { get; set; }
        public DateTime FechaIngresoLaboral { get; set; }
        public string CategoriaLaboral { get; set; }
        public string DestinoLaboral { get; set; }
        public string NumeroLegajoLaboral { get; set; }
        public int NumeroAsociado { get; set; }

        public bool PersonaPoliticamenteExpuesta { get; set; }
        public bool EsMilitar { get; set; }
        public bool RecibirPublicidad { get; set; }

        public ReferenciaRequest ReferenciaA { get; set; }
        public ReferenciaRequest ReferenciaB { get; set; }
    }

    public class ClienteUpdateRequest : ClienteCreateRequest
    {
    }

    public class ReferenciaRequest
    {
        public string NombreCompleto { get; set; }
        public string Vinculo { get; set; }
        public string Telefono { get; set; }
    }

    public class ClienteComboDTO
    {
        public string text { get; set; }
        public int id { get; set; }
    }

    public class LocalidadSelectDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
}