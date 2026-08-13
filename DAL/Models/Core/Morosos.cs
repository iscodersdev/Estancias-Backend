using System;

namespace DAL.Models
{
    public class Morosos
    {
        public int Id { get; set; }
        public string DNI { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaCarga { get; set; } = DateTime.Now;
    }
}
