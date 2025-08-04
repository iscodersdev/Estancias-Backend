using System;
using System.Linq;
using System.Threading.Tasks;
using EstanciasCore.Areas.Administracion.ViewModels;
using Commons.Identity.Services;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32;
using Newtonsoft.Json;
using DAL.DTOs;
using DAL.DTOs.Servicios;
using EstanciasCore.Services;
using DAL.DTOs.Servicios.DatosTarjeta;

namespace EstanciasCore.Controllers
{
    [Area("Administracion")]
    public class UsuariosController : EstanciasCoreController
    {


        private readonly EstanciasContext _context;
        private readonly UserService<Usuario> _userService;
        private readonly UserManager<Usuario> _UserManager;

        public UsuariosController(EstanciasContext context, UserService<Usuario> userService, UserManager<Usuario> userManager) : base(context)
        {
            _context = context;
            _userService = userService;
            this._UserManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> BorrarUsuario()
        {
            return View();
        }

        public IActionResult UsuariosDataTable()
        {
            //var usuarios = _context.Users.ToList();
            var query = from usu in _context.Users
                select new UserDTViewModel
                {
                    Id = usu.Id,
                    Usuario = usu.UserName,
                    Nombre = (usu.Personas!=null) ? usu.Personas.Apellido+" "+usu.Personas.Nombres : " ",
                    Empresa ="",
                    Administrador = usu.Administradores,
                    AdministradorTexto = usu.Administradores==true ? "SI" : "NO",
                    NroDocumento =(usu.Personas!=null) ? usu.Personas.NroDocumento : " ",

                };

            return DataTable<UserDTViewModel>(query.AsQueryable<UserDTViewModel>());
            //return DataTable(query.AsQueryable());
        }

        [HttpGet]
        public async Task<IActionResult> CreateUserEstancia()
        {        
            return PartialView();
        }   

        [HttpPost]
        public async Task<IActionResult> CreateUserEstancia(UsuarioEstanciasVM userDTO)
        {
            var empresa = _context.Empresas.FirstOrDefault(x => x.Id == 3);
            var user = await _userService.FindByEmailAsync(userDTO.Mail.ToString().Trim());
            var personaLoan = getPersonaloanByNroTarjeta(userDTO.TarjetaEstancia.ToString());
            if (user!=null && userDTO.TarjetaEstancia!=null)
            {
                if (user.Personas!=null)
                {
                    user.Personas.NroTarjeta = userDTO.TarjetaEstancia.ToString();
                    _context.Usuarios.Update(user);
                    _context.SaveChanges();
                }
            }

            var personalocal = _context.Personas.FirstOrDefault(x => x.Email == userDTO.Mail.ToString().Trim());
            var clienteLocal = _context.Clientes.FirstOrDefault(x => x.Persona.Email == userDTO.Mail.ToString().Trim());
            int token = common.NiumeroRandom(100000, 999999);

            if (userDTO.Password == null)
            {
            
            }

            if (empresa == null)
            {
            }

            if (clienteLocal!=null)
            {
                if (clienteLocal.Persona!=null) // Si la persona Existe
                {
                    if (user!=null)
                    {
                        if (clienteLocal.Id == user.Clientes.Id)
                        {
                            //"Socio Ya Ingresado, debera recuperar contraseña";
                        }
                        else
                        {
                            clienteLocal.RegistroMobile = true;
                            CreateOrUpdateUser(user, clienteLocal, userDTO.Mail.Trim(), token, userDTO.Password);
                            var result = await _userService.ChangePasswordAsync(user, "xahs567g", userDTO.Password);
                        }
                    }
                    else
                    {
                        clienteLocal.RegistroMobile = true;
                        CreateOrUpdateUser(user, clienteLocal, userDTO.Mail.Trim(), token, userDTO.Password);
                        //var result = await _userService.ChangePasswordAsync(user, "xahs567g", Registro.Password1);
                    }
                }
                else
                {
                    clienteLocal.Persona = new Persona()
                    {
                        NroDocumento = userDTO.NroDocumento.ToString(),
                        Apellido = userDTO.Apellido,
                        Nombres = userDTO.Nombre,
                        FechaNacimiento = Convert.ToDateTime(personaLoan.FechaNacimiento),
                        Email = userDTO.Mail.Trim(),
                        NroTarjeta = userDTO.TarjetaEstancia.ToString().TrimStart('0')
                    };

                    if (userDTO.TarjetaEstancia!=null)
                    {
                        var persona = getPersonaloanByNroTarjeta(userDTO.TarjetaEstancia.ToString());
                        if (persona!=null)
                        {
                            clienteLocal.Persona.NroDocumento = persona.NroDocumento;
                        }
                        clienteLocal.Persona.FechaNacimiento = Convert.ToDateTime(personaLoan.FechaNacimiento);
                    }

                    clienteLocal.RegistroMobile = true;
                    CreateOrUpdateUser(user, clienteLocal, userDTO.Mail.Trim(), token, userDTO.Password);
                    var result = await _userService.ChangePasswordAsync(user, "xahs567g", userDTO.Password);

                }
            }
            else
            {
                Clientes cliente = new Clientes();
                cliente.Empresa = _context.Empresas.FirstOrDefault();
                cliente.TipoCliente = _context.TiposClientes.Find(1);
                cliente.Celular = (userDTO.Telefono != null) ? userDTO.Telefono : "";
                cliente.Localidad = _context.Localidad.Find(24860);
                cliente.Provincia = _context.Provincia.Find(6);

                Persona personaLocal = _context.Personas.Where(x => x.Email==userDTO.Mail.Trim()).FirstOrDefault();

                if (personaLocal!=null)
                {
                    cliente.Persona = personaLocal;
                    cliente.RegistroMobile = true;
                    user = CreateOrUpdateUser(user, cliente, userDTO.Mail.Trim(), token, userDTO.Password);
                    var result = await _userService.ChangePasswordAsync(user, "xahs567g", userDTO.Password);
                }
                else
                {
                    cliente.Persona = new Persona()
                    {
                        NroDocumento = userDTO.NroDocumento.ToString(),
                        Apellido = userDTO.Apellido,
                        Nombres = userDTO.Nombre,
                        FechaNacimiento = Convert.ToDateTime(personaLoan.FechaNacimiento),
                        Email = userDTO.Mail.Trim(),
                        NroTarjeta = userDTO.TarjetaEstancia.ToString().TrimStart('0')
                    };
                    if (userDTO.TarjetaEstancia!=null)
                    {
                        var persona = getPersonaloanByNroTarjeta(userDTO.TarjetaEstancia.ToString());
                        if (persona!=null)
                        {
                            cliente.Persona.NroDocumento = persona.NroDocumento;
                        }
                    }

                    cliente.RegistroMobile = true;
                    user = CreateOrUpdateUser(user, cliente, userDTO.Mail.Trim(), token, userDTO.Password);
                    var result = await _userService.ChangePasswordAsync(user, "xahs567g", userDTO.Password);
                }
            }
            user = await _userService.FindByEmailAsync(userDTO.Mail.ToString().Trim());
            if (user !=null)
            {
                user.EmailConfirmed = true;
                user.Password = userDTO.Password;

                // Generar un token de reseteo de contraseña
                var tokenReset = await _userService.GeneratePasswordResetTokenAsync(user);
                var result = await _userService.ResetPasswordAsync(user, tokenReset, userDTO.Password);
                _context.Usuarios.Update(user);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se creó correctamente el Usuario.");
                return RedirectToAction("Index");
            }
            else
            {
                AddPageAlerts(PageAlertType.Error, "Error al crear el Usuario.");
                //return PartialView(userDTO);
                return RedirectToAction("Index");
            }
        }   

        [HttpPost]
        public UsuarioEstanciasVM BuscarUsuarioLoan(string numeroTarjeta)
        {
            var persona = _context.Personas.Where(x => x.NroTarjeta==numeroTarjeta.TrimStart('0').ToString()).FirstOrDefault();
            UsuarioEstanciasVM newUser = new UsuarioEstanciasVM();
            if (persona!=null)
            {
                newUser.Error = true;
                newUser.Mensaje = "El Número de Tarjeta ya esta regisrado a un Usuario";
                return newUser;
            }
            var personaLoan = getPersonaloanByNroTarjeta(numeroTarjeta.ToString());

            if (personaLoan!=null)
            {
                string[] partes = personaLoan.Nombres.Split(',');
                string apellido = partes[0];
                string nombre = partes[1];
                newUser.TarjetaEstancia = numeroTarjeta;
                newUser.NroDocumento = personaLoan.NroDocumento;
                newUser.Apellido = partes[0];
                newUser.Nombre =  partes[1];
                newUser.Mail = personaLoan.Email;
                newUser.Error = false;
                newUser.Mensaje = "";
            }
            else
            {
                newUser.Error = true;
                newUser.Mensaje = "No se encontró ninguna Persona con el Número de Tarjeta";
            }
            return newUser;
        }         
        
        
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            UsuarioVM newUser =CargarViewBag(new UsuarioVM());
            return PartialView(newUser);
        }     
        
        [HttpPost]
        public async Task<string> BuscarUsuario(string valor, string tipoDeBusqueda)
        {
            Usuario usuario = null;
            Persona persona = null;
            Clientes cliente = null;

            switch (tipoDeBusqueda)
            {
                case "1":
                    persona = _context.Personas.FirstOrDefault(x => x.NroDocumento == valor);
                    if (persona != null)
                    {
                        usuario = _context.Usuarios.FirstOrDefault(x => x.Personas.Id == persona.Id);
                        if (usuario != null)
                        {
                            cliente = _context.Clientes.FirstOrDefault(x => x.UsuarioId == usuario.Id);
                        }
                    }
                    break;
                case "2":
                    usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == valor);
                    if (usuario != null)
                    {
                        persona = usuario.Personas;
                        cliente = _context.Clientes.FirstOrDefault(x => x.UsuarioId == usuario.Id);
                    }
                    break;
                case "3":
                    persona = _context.Personas.FirstOrDefault(x => x.NroTarjeta == valor.TrimStart('0'));
                    if (persona != null)
                    {
                        usuario = _context.Usuarios.FirstOrDefault(x => x.Personas.Id == persona.Id);
                        if (usuario != null)
                        {
                            cliente = _context.Clientes.FirstOrDefault(x => x.UsuarioId == usuario.Id);
                        }
                    }
                    break;
            }

            if (usuario == null || persona == null || cliente == null)
            {
                return JsonConvert.SerializeObject(new { success = false, message = "No se encontraron resultados." });
            }

            var usuarioDto = new UsuarioBusquedaDTO();

            if (persona != null)
            {
                usuarioDto.NroDocumento = persona.NroDocumento;
                usuarioDto.Apellidos = persona.Apellido;
                usuarioDto.Nombres = persona.Nombres;
                usuarioDto.NroTarjeta = persona.NroTarjeta;                
                usuarioDto.FechaNacimiento = persona.FechaNacimiento?.ToString("dd/MM/yyyy");
                usuarioDto.Persona = 1;
                usuarioDto.Pagos = _context.PagoTarjeta.Where(x=>x.Persona.Id==persona.Id).Select(x=> new UsuarioBusquedaComprobanteDTO { FechaComprobante = ((DateTime)x.FechaComprobante).ToString("dd/MM/yyyy"), ComprobantePago = x.ComprobantePago}) .ToList();
            }
            else
            {
                usuarioDto.Persona = 0;
            }

            if (usuario != null)
            {
                usuarioDto.UserName = usuario.UserName;
                usuarioDto.DeviceId = usuario.DeviceId;
                usuarioDto.WonderPushDeviceId = usuario.UserIdNotification;
                usuarioDto.Usuario = 1;
            }
            else
            {
                usuarioDto.Usuario = 0;
            }

            if (cliente != null)
            {
                usuarioDto.Celular = cliente.Celular;
                usuarioDto.Cliente = 1;
            }
            else
            {
                usuarioDto.Cliente = 0;
            }

            return JsonConvert.SerializeObject(new { success = true, usuario = usuarioDto });
        }


        [HttpPost]
        public async Task<string> BorrarUsuario(string valor, string tipoDeBusqueda)
        {
            Usuario usuario = null;
            Persona persona = null;
            Clientes cliente = null;
            try
            {
                switch (tipoDeBusqueda)
                {
                    case "1":
                        persona = _context.Personas.FirstOrDefault(x => x.NroDocumento == valor);
                        if (persona != null)
                        {
                            usuario = _context.Usuarios.FirstOrDefault(x => x.Personas.Id == persona.Id);
                            if (usuario != null)
                            {
                                cliente = _context.Clientes.FirstOrDefault(x => x.UsuarioId == usuario.Id);
                            }
                        }
                        break;
                    case "2":
                        usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == valor);
                        if (usuario != null)
                        {
                            persona = usuario.Personas;
                            cliente = _context.Clientes.FirstOrDefault(x => x.UsuarioId == usuario.Id);
                        }
                        break;
                    case "3":
                        persona = _context.Personas.FirstOrDefault(x => x.NroTarjeta == valor.TrimStart('0'));
                        if (persona != null)
                        {
                            usuario = _context.Usuarios.FirstOrDefault(x => x.Personas.Id == persona.Id);
                            if (usuario != null)
                            {
                                cliente = _context.Clientes.FirstOrDefault(x => x.UsuarioId == usuario.Id);
                            }
                        }
                        break;
                }

                if (cliente!=null)
                {
                    var uats = _context.UAT.Where(x=>x.Cliente.Id==cliente.Id).ToList();
                    var notificaciones = _context.NotificacionesPersonas.Where(x => x.Cliente.Id==cliente.Id).ToList();
                    _context.UAT.RemoveRange(uats);
                    _context.NotificacionesPersonas.RemoveRange(notificaciones);
                    _context.Clientes.Remove(cliente);
                    _context.SaveChanges();
                }

                if (usuario != null)
                {
                    _context.Usuarios.Remove(usuario);
                    _context.SaveChanges();
                }

                if (persona != null)
                {
                    var pagos = _context.PagoTarjeta.Where(x => x.Persona.Id==persona.Id).ToList();
                    _context.PagoTarjeta.RemoveRange(pagos);
                    _context.Personas.Remove(persona);
                    _context.SaveChanges();

                }
                return JsonConvert.SerializeObject(new { success = true, respuesta = true });
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new { success = true, respuesta = false });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(UsuarioVM usuario)
        {
            try
            {
                ModelState.Remove("Persona.TipoDocumento.Descripcion");
                ModelState.Remove("Persona.Genero.Descripcion");
                if (ModelState.IsValid)
                {
                    Usuario nuevoUsuario = new Usuario()
                    {
                        UserName = usuario.Mail,
                        Email = usuario.Mail,
                        Mail = usuario.Mail,
                        Password=usuario.Password,
                        Administradores=usuario.Administrador

                    };
                    var result = await _userService.CreateAsync(nuevoUsuario, usuario.Password);
                    
                    usuario.Persona.TipoDocumento = _context.TipoDocumento.Find(usuario.Persona.TipoDocumento.Id);
                    usuario.Persona.Pais = _context.Paises.Find(usuario.Persona.Pais.Id);
                    await _context.Personas.AddAsync(usuario.Persona);
                    nuevoUsuario.Personas = usuario.Persona;
                   // if (!usuario.Administrador)
                    //{
                        nuevoUsuario.Clientes = new Clientes();
                        nuevoUsuario.Clientes.Empresa = _context.Empresas.FirstOrDefault();
                        nuevoUsuario.Clientes.Persona = usuario.Persona;
                        nuevoUsuario.Clientes.RazonSocial = usuario.Persona.Apellido + ", " + usuario.Persona.Nombres;
                        nuevoUsuario.Clientes.FechaIngreso = DateTime.Now;
                        await _context.Clientes.AddAsync(nuevoUsuario.Clientes);
                    //}
                    _context.Usuarios.Update(nuevoUsuario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    usuario = CargarViewBag(usuario);
                    return PartialView(usuario);
                }
                
            }
            catch (Exception e)
            {

                return RedirectToAction("Index");
            }
          
        }

        [HttpGet]
        public async Task<IActionResult> Update(string Id)
        {
            Usuario updateUser =await _context.Usuarios.FindAsync(Id);
            UsuarioVM updateModel = new UsuarioVM()
            {
                UserId = updateUser.Id,
                Mail = updateUser.Email,
                Password = updateUser.Password,
                Administrador = updateUser.Administradores,
                Persona=updateUser.Personas
            };
            updateModel=CargarViewBag(updateModel);
            return PartialView(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UsuarioVM usuario)
        {
            try
            {
                ModelState.Remove("Persona.TipoDocumento.Descripcion");
                ModelState.Remove("Persona.Genero.Descripcion");
                if (ModelState.IsValid)
                {
                    Persona updatePersona = await _context.Personas.FindAsync(usuario.Persona.Id);
                    updatePersona.NroDocumento = usuario.Persona.NroDocumento;
                    updatePersona.Nombres = usuario.Persona.Nombres;
                    updatePersona.Apellido = usuario.Persona.Apellido;
                    updatePersona.Cuil = usuario.Persona.Cuil;
                    updatePersona.FechaNacimiento = usuario.Persona.FechaNacimiento;
                    updatePersona.TipoDocumento = await _context.TipoDocumento.FindAsync(usuario.Persona.TipoDocumento.Id);
                    updatePersona.Pais = await _context.Paises.FindAsync(usuario.Persona.Pais.Id);
                    _context.Personas.Update(updatePersona);
                    await _context.SaveChangesAsync();

                    var user = await _context.Users.FindAsync(usuario.UserId);
                    user.Personas = updatePersona;
                    user.UserName = usuario.Mail;
                    user.NormalizedUserName = usuario.Mail.ToUpper();
                    user.Email = usuario.Mail;
                    user.NormalizedEmail = usuario.Mail.ToUpper();
                    user.Mail = usuario.Mail;
                    user.Password = usuario.Password;
                    user.Administradores = usuario.Administrador;
                    if (usuario.Administrador)
                    {
                        user.Clientes = null;
                    }
                    else
                    {
                        if (user.Clientes != null)
                        {
                            user.Clientes = new Clientes();
                            user.Clientes.Empresa = _context.Empresas.Find(1);
                            user.Clientes.Persona = updatePersona;
                            await _context.Clientes.AddAsync(user.Clientes);
                        }

                    }
                    user.PasswordHash = _UserManager.PasswordHasher.HashPassword(user, usuario.Password);
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    usuario=CargarViewBag(usuario);
                    return PartialView(usuario);
                }
            }
            catch (Exception e)
            {

                return RedirectToAction("Index");
            }

        }


        [HttpGet]
        public async Task<IActionResult> _EnabledAdmin(string Id)
        {
            Usuario usuario = _context.Usuarios.Where(s => s.Id == Id).First();
            if (ModelState.IsValid)
            {
                try
                {
                    usuario.Administradores = true;
                    _context.Usuarios.Update(usuario);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modificó correctamente el Usuario " + usuario.UserName + ".");
                    return RedirectToAction("Index", "Usuarios");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Usuario. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Usuarios");
                }

            }
            else
            {
                return PartialView(usuario);
            }
        }

        [HttpGet]
        public async Task<IActionResult> _DisabledAdmin(string Id)
        {
            Usuario usuario = _context.Usuarios.Where(s => s.Id == Id).First();
            if (ModelState.IsValid)
            {
                try
                {
                    usuario.Administradores = false;
                    _context.Usuarios.Update(usuario);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modificó correctamente el Usuario " + usuario.UserName + ".");
                    return RedirectToAction("Index", "Usuarios");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Usuario. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Usuarios");
                }

            }
            else
            {
                return PartialView(usuario);
            }
        }


        public IActionResult _ChangePassword(string Id)
        {
            try
            {
                var usuario = _context.Usuarios.Where(x=>x.Id==Id).FirstOrDefault();
                return PartialView(usuario);

            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Ha ocurrido un error al modificar el usuario, intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> _ChangePassword(string userId, string password, string RepeatPassword)
        {

            try
            {
                if (password==RepeatPassword)
                {
                    var usuario = await _UserManager.FindByIdAsync(userId);
                    if (usuario == null)
                    {
                        AddPageAlerts(PageAlertType.Error, "Hubo un error al cambiar la contraseña del usuario.");
                        return RedirectToAction(nameof(Index));
                    }

                    // Generar un token de reseteo de contraseña
                    var token = await _UserManager.GeneratePasswordResetTokenAsync(usuario);

                    // Resetear la contraseña del usuario
                    var result = await _UserManager.ResetPasswordAsync(usuario, token, password);
                    usuario.Password = password;
                    _context.Usuarios.Update(usuario);
                    _context.SaveChanges();

                    AddPageAlerts(PageAlertType.Success, "Se ha cambiado la contraseña del usuario exitosamente.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al cambiar la contraseña del usuario.");
                    return RedirectToAction(nameof(Index));
                }

                
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Ha ocurrido un error al actualizar el usuario, intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }


        public UsuarioVM CargarViewBag(UsuarioVM user)
        {
            user.TipoDocumento = _context.TipoDocumento.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            user.Pais = _context.Paises.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            return user;
        }

        private PersonaLoan getPersonaloanByNroTarjeta(string nroTarjeta) //Obtiene la persona y me comprueba la existensia de la persona.
        {
            BDExternaPersonalService consulta = new BDExternaPersonalService(_context);
            var persona = consulta.getPersonaloanByNroTarjeta(nroTarjeta)?.FirstOrDefault();
            if (persona != null)
            {
                return persona;
            }
            else
                return null;
        }

        
        private void UpdateUser(Usuario usuario, Clientes cliente)
        {
            cliente.Usuario=usuario;
            usuario.Clientes=cliente;
            usuario.Personas=cliente.Persona;
            _context.Update(usuario);
            _context.SaveChanges();
        }


        private bool CreateUser(string UserName, string Email, int token, string password = "xahs567g")
        {
            try
            {
                var user = new Usuario()
                {
                    UserName = UserName,
                    Email = Email,
                    Token = token
                };
                var result = _userService.CreateAsync(user, password);
                return result.Result.Succeeded;
            }
            catch (Exception)
            {

                return false;
            }

        }
        private Usuario CreateOrUpdateUser(Usuario usuario, Clientes clienteLocal = null, string mail = null, int token = 0, string password = "xahs567g")
        {
            try
            {
                if (usuario!=null)
                {
                    Clientes Cliente = _context.Clientes.Where(x => x.UsuarioId==usuario.Id).FirstOrDefault();
                    if (Cliente!=null)
                    {
                        Cliente.Persona = clienteLocal.Persona;
                        clienteLocal = Cliente;
                    }
                    UpdateUser(usuario, clienteLocal);
                    return usuario;
                }
                else
                {
                    if (CreateUser(mail, mail, token, password))
                    {
                        Usuario usuarioLocal = _context.Usuarios.FirstOrDefault(x => x.UserName == mail);
                        UpdateUser(usuarioLocal, clienteLocal);
                        return usuarioLocal;
                    }
                }
                return null;
            }
            catch (Exception e)
            {

                return null;
            }

        }

    }
}
