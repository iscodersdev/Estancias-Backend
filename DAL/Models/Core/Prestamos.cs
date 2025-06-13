using Commons.Identity;
using Commons.Models;
using DAL.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Vendedores
    {
        public int Id { get; set; }
        public virtual Persona Persona { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
    }

    public class FormasPago
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class Monedas
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class TiposMovimientos 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Credito { get; set; }
        public bool Debito { get; set; }
    }
    public class CuentasCorrientes
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal Credito { get; set; }
        public decimal Debito { get; set; }
        public decimal Saldo { get; set; }
        public virtual TiposMovimientos TipoMovimiento { get; set; }
    }

    public class Campanas
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaBaja { get; set; }
        public virtual Empresas Empresa { get; set; }
        public string Observaciones { get; set; }
        public string TextoMail { get; set; }
        public int ProvinciaId { get; set; }
        public decimal MinimoDisponible { get; set; }
        public decimal MaximoDisponible { get; set; }
        public int UnidadId { get; set; }
        public int CantidadPersonas { get; set; }
        public int CantidadUnidades { get; set; }
        public int CantidadProvincias { get; set; }
        public byte[] Imagen { get; set; }
    }
    public class CampanasRenglones
    {
        public int Id { get; set; }
        public virtual Campanas Cabecera { get; set; }
        public Int64 DNI { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string eMail { get; set; }
        public string Celular { get; set; }
        public decimal Disponible { get; set; }
        public decimal ImporteMaximo { get; set; }
        public int UnidadId { get; set; }
        public int ProvinciaId { get; set; }
        public string Unidad { get; set; }
        public string Provincia { get; set; }
        public string TipoPersona { get; set; }
    }
    public class MTraeListaProvinciasDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<MListaProvinciasDTO> Provincias { get; set; }
    }
    public class MListaProvinciasDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class MTraeListaUnidadesDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int ProvinciaId { get; set; }
        public List<MListaUnidadesDTO> Unidades { get; set; }
    }
    public class MListaUnidadesDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
    public class LeyendaTipoMovimiento
    {
        public int Id { get; set; }
        public string NombreMovimiento { get; set; }
        public string TextoLeyenda { get; set; }
        public bool Activo { get; set; }
    }
}