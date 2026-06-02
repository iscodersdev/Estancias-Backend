using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace EstanciasCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginApiController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly UserManager<Usuario> _userManager;

        public LoginApiController(
            EstanciasContext context,
            UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginApiDTO loginDTO)
        {
            try
            {
                if (loginDTO == null)
                {
                    return BadRequest(new LoginApiRespuestaDTO
                    {
                        Status = 400,
                        Mensaje = "Debe enviar usuario y contraseña.",
                        UAT = "",
                        Usuario = "",
                        FechaHora = DateTime.Now
                    });
                }

                if (string.IsNullOrWhiteSpace(loginDTO.Usuario) ||
                    string.IsNullOrWhiteSpace(loginDTO.Password))
                {
                    return BadRequest(new LoginApiRespuestaDTO
                    {
                        Status = 400,
                        Mensaje = "Usuario y contraseña son obligatorios.",
                        UAT = "",
                        Usuario = loginDTO.Usuario ?? "",
                        FechaHora = DateTime.Now
                    });
                }

                var identityUser = await _userManager.FindByEmailAsync(loginDTO.Usuario.Trim());

                if (identityUser == null)
                {
                    identityUser = await _userManager.FindByNameAsync(loginDTO.Usuario.Trim());
                }

                if (identityUser == null)
                {
                    Log.Warning("Intento de login API con usuario inexistente: {Usuario}", loginDTO.Usuario);

                    return Unauthorized(new LoginApiRespuestaDTO
                    {
                        Status = 401,
                        Mensaje = "Usuario o contraseña incorrectos.",
                        UAT = "",
                        Usuario = loginDTO.Usuario,
                        FechaHora = DateTime.Now
                    });
                }

                var passwordOk = await _userManager.CheckPasswordAsync(identityUser, loginDTO.Password);

                if (!passwordOk)
                {
                    Log.Warning("Intento de login API con contraseña incorrecta. Usuario: {Usuario}", loginDTO.Usuario);

                    return Unauthorized(new LoginApiRespuestaDTO
                    {
                        Status = 401,
                        Mensaje = "Usuario o contraseña incorrectos.",
                        UAT = "",
                        Usuario = loginDTO.Usuario,
                        FechaHora = DateTime.Now
                    });
                }

                var usuarioSistema = await _context.Usuarios
                    .Include(x => x.Personas)
                    .Include(x => x.Clientes)
                    .FirstOrDefaultAsync(x => x.Id == identityUser.Id);

                if (usuarioSistema == null)
                {
                    Log.Warning("Usuario válido en Identity pero no encontrado en _context.Usuarios. Id: {Id}", identityUser.Id);

                    return Unauthorized(new LoginApiRespuestaDTO
                    {
                        Status = 401,
                        Mensaje = "El usuario existe, pero no está vinculado correctamente al sistema.",
                        UAT = "",
                        Usuario = loginDTO.Usuario,
                        FechaHora = DateTime.Now
                    });
                }

                var token = GenerarTokenSeguro();

                var uat = new UAT
                {
                    Usuario = usuarioSistema,
                    Cliente = usuarioSistema.Clientes,
                    Persona = usuarioSistema.Personas,
                    Token = token,
                    FechaHora = DateTime.Now
                };

                _context.UAT.Add(uat);
                await _context.SaveChangesAsync();

                Log.Information("Login API correcto. Usuario: {Usuario}", loginDTO.Usuario);

                return Ok(new LoginApiRespuestaDTO
                {
                    Status = 200,
                    Mensaje = "Login correcto.",
                    UAT = token,
                    Usuario = usuarioSistema.UserName,
                    FechaHora = uat.FechaHora
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en LoginApiController.Login");

                return StatusCode(500, new LoginApiRespuestaDTO
                {
                    Status = 500,
                    Mensaje = "Error interno al iniciar sesión.",
                    UAT = "",
                    Usuario = loginDTO != null ? loginDTO.Usuario : "",
                    FechaHora = DateTime.Now
                });
            }
        }

        private static string GenerarTokenSeguro()
        {
            var bytes = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return Convert.ToBase64String(bytes);
        }
    }
}