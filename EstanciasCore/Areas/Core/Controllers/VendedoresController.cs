using Commons.Identity.Services;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class VendedoresController : EstanciasCoreController
    {
        private readonly UserService<Usuario> _userService;
        public VendedoresController(EstanciasContext context, UserService<Usuario> userService) : base(context)
        {
            _userService = userService;
            breadcumb.Add(new Message() { DisplayName = "Gestión" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Vendedores" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public IActionResult VendedoresDataTable()
        {
            var vendedores = _context.Vendedores.ToList();
            var query = from v in vendedores
                        select new ListVendedoresDTO
                        {
                            Id=v.Id,
                            NroDocumento=(v.Persona==null)?"---":v.Persona.NroDocumento,
                            Nombre= (v.Persona == null) ? "---" : v.Persona.GetNombreCompleto(),
                            Domicilio= (String.IsNullOrWhiteSpace(v.Domicilio)) ? "---" : v.Domicilio.ToString(),
                            Telefono = (String.IsNullOrWhiteSpace(v.Telefono))?"---":v.Telefono.ToString(),
                            Mail=(String.IsNullOrWhiteSpace(v.Mail))?"---":v.Mail.ToString()
                        };
            return DataTable(query.AsQueryable());
        }

        public IActionResult _Create()
        {
            VendedorViewBag();
            Vendedores model = new Vendedores();
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create([Bind("Persona,Persona.TipoDocumento,Persona.Pais,Persona.NroDocumento,Persona.Cuil,Persona.Apellido,Persona.Nombres,Persona.FechaNacimiento,Persona.EstadoCivil, Domicilio, Telefono, Mail")] Vendedores vendedor)
        {
            try
            {
                if (vendedor.Persona.NroDocumento == null)
                {
                    ModelState.AddModelError("Persona.NroDocumento", "Debe ingresar el Numero de Documento del Vendedor.");
                }
                else
                {
                    var usuarioVendedor = _context.Usuarios.Where(x => x.Vendedores.Persona.NroDocumento == vendedor.Persona.NroDocumento.ToString()).FirstOrDefault();
                    if (usuarioVendedor != null)
                    {
                        ModelState.AddModelError("Persona.NroDocumento", "Ya existe un Vendedor con el Numero de Documento Ingresado");
                    }
                }
                if (vendedor.Persona.TipoDocumento == null)
                {
                    ModelState.AddModelError("Persona.TipoDocumento", "Debe seleccionar un Tipo de Documento");
                }
                else
                {
                    ModelState.Remove("Persona.TipoDocumento.Descripcion");
                }
                if (vendedor.Persona.Pais == null)
                {
                    ModelState.AddModelError("Persona.Pais", "Debe seleccionar un Pais");
                }               
                ModelState.Remove("Id");
                ModelState.Remove("Persona.Id");
                ModelState.Remove("Persona.Genero.Descripcion");
                if (ModelState.IsValid)
                {
                    var usuarioVendedor = _context.Usuarios.Where(x => x.UserName == vendedor.Mail).FirstOrDefault();
                    if (usuarioVendedor == null)
                    {
                        vendedor.Persona.TipoDocumento = await _context.TipoDocumento.FindAsync(vendedor.Persona.TipoDocumento.Id);
                        vendedor.Persona.Pais = await _context.Paises.FindAsync(vendedor.Persona.Pais.Id);
                        await _context.Personas.AddAsync(vendedor.Persona);
                        await _context.Vendedores.AddAsync(vendedor);
                        Usuario nuevoUsuario = new Usuario()
                        {
                            UserName = vendedor.Mail,
                            Email = vendedor.Mail,
                            Mail = vendedor.Mail
                        };
                        var result = await _userService.CreateAsync(nuevoUsuario, vendedor.Persona.NroDocumento.ToString());
                        nuevoUsuario.Vendedores = vendedor;
                        nuevoUsuario.Personas = vendedor.Persona;
                        _context.Usuarios.Update(nuevoUsuario);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (usuarioVendedor.Personas == null)
                        {
                            vendedor.Persona.TipoDocumento = await _context.TipoDocumento.FindAsync(vendedor.Persona.TipoDocumento.Id);
                            vendedor.Persona.Pais = await _context.Paises.FindAsync(vendedor.Persona.Pais.Id);
                            await _context.Personas.AddAsync(vendedor.Persona);
                            usuarioVendedor.Personas = vendedor.Persona;
                        }
                        else
                        {
                            usuarioVendedor.Personas.TipoDocumento = await _context.TipoDocumento.FindAsync(vendedor.Persona.TipoDocumento.Id);
                            usuarioVendedor.Personas.NroDocumento = vendedor.Persona.NroDocumento;
                            usuarioVendedor.Personas.Apellido = vendedor.Persona.Apellido;
                            usuarioVendedor.Personas.Nombres = vendedor.Persona.Nombres;
                            usuarioVendedor.Personas.Cuil = vendedor.Persona.Cuil;
                            usuarioVendedor.Personas.Pais = await _context.Paises.FindAsync(vendedor.Persona.Pais.Id);
                            usuarioVendedor.Personas.FechaNacimiento = vendedor.Persona.FechaNacimiento;
                            _context.Personas.Update(usuarioVendedor.Personas);
                        }
                        if (usuarioVendedor.Vendedores == null)
                        {
                            vendedor.Persona = usuarioVendedor.Personas;
                            await _context.Vendedores.AddAsync(vendedor);
                            usuarioVendedor.Vendedores = vendedor;
                        }
                        else
                        {
                            usuarioVendedor.Vendedores.Mail = vendedor.Mail;
                            usuarioVendedor.Vendedores.Domicilio = vendedor.Domicilio;
                            usuarioVendedor.Vendedores.Telefono = vendedor.Telefono;
                            _context.Vendedores.Update(usuarioVendedor.Vendedores);
                        }
                        _context.Usuarios.Update(usuarioVendedor);
                        await _context.SaveChangesAsync();
                    }
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Cliente.");
                    return RedirectToAction("Index", "Clientes");
                }
                else
                {
                    VendedorViewBag();
                    return PartialView(vendedor);
                }
            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al crear el Vendedor. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Vendedores");
            }            
        }

        public async Task<IActionResult> _Update(int Id)
        {
            VendedorViewBag();
            Vendedores vendedor = await _context.Vendedores.FindAsync(Id);
            return PartialView(vendedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Vendedores vendedor)
        {
            ModelState.Remove("Id");
            ModelState.Remove("Persona.Id");
            ModelState.Remove("Persona.Genero.Descripcion");
            if (ModelState.IsValid)
            {
                try
                {
                    var vendedorEditar = await _context.Vendedores.FindAsync(vendedor.Id);
                    if (vendedorEditar == null)
                    {
                        AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Vendedor. Intentelo nuevamente mas tarde.");
                        return RedirectToAction("Index", "Vendedores");
                    }
                    else
                    {
                        vendedorEditar.Domicilio = vendedor.Domicilio;
                        vendedorEditar.Telefono = vendedor.Telefono;
                        vendedorEditar.Persona.TipoDocumento = await _context.TipoDocumento.FindAsync(vendedor.Persona.TipoDocumento.Id);
                        vendedorEditar.Persona.NroDocumento = vendedor.Persona.NroDocumento;
                        vendedorEditar.Persona.Apellido = vendedor.Persona.Apellido;
                        vendedorEditar.Persona.Nombres = vendedor.Persona.Nombres;
                        vendedorEditar.Persona.Cuil = vendedor.Persona.Cuil;
                        vendedorEditar.Persona.Pais = await _context.Paises.FindAsync(vendedor.Persona.Pais.Id);
                        vendedorEditar.Persona.FechaNacimiento = vendedor.Persona.FechaNacimiento;                 
                    }
                    var usuarioVendedor = _context.Usuarios.Where(x => x.UserName == vendedor.Mail).FirstOrDefault();
                    if (usuarioVendedor == null)
                    {
                        Usuario user = new Usuario()
                        {
                            UserName = vendedor.Mail,
                            Email = vendedor.Mail,
                            Mail = vendedor.Mail
                        };
                        var result = await _userService.CreateAsync(user, vendedor.Persona.NroDocumento.ToString());
                        usuarioVendedor = user;
                    }
                    usuarioVendedor.Vendedores = vendedorEditar;
                    usuarioVendedor.Personas = vendedorEditar.Persona;
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente el Vendedor " + vendedor.Persona.GetNombreCompleto() + ".");
                    return RedirectToAction("Index", "Vendedores");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Vendedor. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Vendedores");
                }
            }
            else
            {
                VendedorViewBag();
                return PartialView(vendedor);
            }
        }

        public async Task<IActionResult> _Delete(int Id)
        {
            VendedorViewBag();
            Vendedores vendedor = await _context.Vendedores.FindAsync(Id);
            return PartialView(vendedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                Vendedores vendedor = _context.Vendedores.Where(s => s.Id == id).First();
                var usuarioVendedor = _context.Usuarios.Where(x => x.UserName == vendedor.Mail).FirstOrDefault();
                if (usuarioVendedor != null)
                {
                    usuarioVendedor.Vendedores = null;
                }
                _context.Usuarios.Update(usuarioVendedor);
                _context.Vendedores.Remove(vendedor);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Vendedor.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Vendedor.");
                return RedirectToAction("Index", "Vendedores");
            }
        }
        private void VendedorViewBag()
        {
            ViewBag.TiposDocumento = _context.TipoDocumento.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            ViewBag.Paises = _context.Paises.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
        }

    }
}
