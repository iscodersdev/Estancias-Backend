using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NumeroDocumento { get; set; }
        public string Grado { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string Unidad { get; set; }
        public string Servicio { get; set; }
        public bool EsAdmin { get; set; }
        public bool Ldap { get; set; }
    }
}
