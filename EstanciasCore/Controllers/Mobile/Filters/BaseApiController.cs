using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace EstanciasCore.API.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected EstanciasContext _context { get; }

        public BaseApiController(EstanciasContext context)
        {
            _context = context;
        }

        public bool ValidaCuilYTelefono(RespuestaAPIWH respuesta)
        {
            if (respuesta.CUIL?.Trim().Length != 11) { respuesta.Mensaje = "El CUIL recibido no es valido"; respuesta.Status = 400; return false; }
            if (respuesta.Telefono == null || respuesta.Telefono?.Trim().Length == 0) { respuesta.Mensaje = "El Telefono recibido no es valido"; respuesta.Status = 400; return false; }
            return true;
        }

        public Usuario TraeUsuarioUAT(string uat)
        {
            return _context.UAT.Where(u => u.Token == uat).Select(u => u.Cliente.Usuario).FirstOrDefault();
        }

        public Usuario TraeUsuario(string nombreUsuario)
        {
            return _context.Users.Where(u => u.NormalizedUserName == nombreUsuario).FirstOrDefault();
        }

        public async Task<string> GenerarUAT(Usuario usuario)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            string uat = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: usuario.NormalizedUserName,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var nuevaUat = new UAT { FechaHora = DateTime.Now, Token = uat, Usuario = usuario };
            _context.Add(nuevaUat);
            await _context.SaveChangesAsync();
            
            return uat;
        }

        public string TraeUAT(string nombreUsuario, bool generarUAT = false)
        {
            var usuario = TraeUsuario(nombreUsuario);
            if (usuario == null) return null;

            var uat = _context.UAT.Where(u => u.Usuario.Id == usuario.Id).OrderByDescending(u => u.FechaHora).Select(u => u.Token).FirstOrDefault();
            if (uat == null && generarUAT)
            {
                return GenerarUAT(usuario).Result;    
            }
            return uat;
        }

        public DAL.Models.Core.Billetera TraeBilletera(Usuario usuario)
        {
            return _context.Billeteras.Where(b => b.Cliente.Usuario.Id == usuario.Id).FirstOrDefault();
        }

        public DAL.Models.Core.Billetera TraeBilleteraCVU(string cvu)
        {
            return _context.Billeteras.Where(b => b.CVU == cvu).FirstOrDefault();
        }

        public Clientes TraeCliente(Usuario usuario)
        {
            return _context.Clientes.Where(c => c.Usuario.Id == usuario.Id).FirstOrDefault();
        }

    }
}
