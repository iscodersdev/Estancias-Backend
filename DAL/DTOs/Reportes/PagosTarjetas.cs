using DAL.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Reportes
{
    public class LiquidacionCuota
    {
        public int Id { get; set; }
        public Cuota Cuota { get; set; }
        public EstadoCuota Estado { get; set; }
        // ... otras propiedades
    }

    public class Cuota
    {
        public int Id { get; set; }
        public Prestamo Prestamo { get; set; }
        public decimal MontoCuota { get; set; }
        // ... otras propiedades
    }

    public class Prestamo
    {
        public int Id { get; set; }
        public Cliente Cliente { get; set; }
        // ... otras propiedades
    }

    public class Cliente
    {
        public int Id { get; set; }
        public int Legajo { get; set; }
        public Persona Persona { get; set; }
        // ... otras propiedades
    }

    public class Persona
    {
        public int Id { get; set; }
        public string NroDocumento { get; set; }
        public string ApellidoNombre { get; set; } // Asumo que existe para el filtro
                                                   // ... otras propiedades
    }

    public enum EstadoCuota
    {
        PedienteLiquidar, // Asumo que es "PendienteLiquidar"
        Liquidada,
        // ... otros estados
    }

    // Placeholder para simular la descripción de BocaDePago
    public class BocaDePagoInfo
    {
        public string Descripcion { get; set; } = "General"; // Valor por defecto
    }

    public class FiltroPagosViewModel
    {
        // Parámetros de entrada para el filtro
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public int? EstadoId { get; set; }
        public uint? PersonaId { get; set; } // Unificado para NroDocumento o Nombre/Apellido si ambos son ID
        public string NombrePersona { get; set; } // Para búsqueda por texto
        public string Monto { get; set; }

        // Parámetros para paginación (ej. para DataTables)
        public int Start { get; set; } = 0; // 'draw' en DataTables
        public int Length { get; set; } = 15; // 'length' en DataTables

        // Propiedades para devolver los resultados
        public List<PagoTarjeta> Pagos { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
    }
}
