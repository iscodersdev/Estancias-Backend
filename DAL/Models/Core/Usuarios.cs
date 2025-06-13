using Commons.Identity;
using DAL.Models.Core;
using System.Collections.Generic;

namespace DAL.Models
{
    public class Usuario : CommonsUser
    {
        public string Mail { get; set; }
        public string DeviceId { get; set; }
        public string Password { get; set; }
        public bool RecordarPassword { get; set; }
        public virtual Clientes Clientes { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Vendedores Vendedores { get; set; }
        public virtual Persona Personas { get; set; }
        public string UserIdNotification { get; set; }
        public bool Administradores { get; set; }
        public int Token { get; set; }
        public override string GetFirstName()
        {
            return "";
        }
        public override string GetMiddleName()
        {
            return "";
        }

        public override string GetLastName()
        {
            return "";
        }

        public override string GetRoleString()
        {
            return "";
        }

        public override List<IWorkSpace> GetRelatedIWorkSpaces()
        {
            return new List<IWorkSpace>();
        }

    }

}