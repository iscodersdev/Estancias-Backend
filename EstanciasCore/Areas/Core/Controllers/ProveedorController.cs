using EstanciasCore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class ProveedorController : EstanciasCoreController
    {
        public object FotoProveedor { get; private set; }

        public ProveedorController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Proveedor" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }
        public IActionResult ProveedorDataTable()
        {
            List<Proveedor> proveedores = _context.Proveedores.Where(x => x.Activo).ToList();
            var query = from p in proveedores
                        select new ListProveedorDTO
                        {
                            Id = p.Id,
                            NombreCompleto = p.Nombre?.ToUpper(),
                            CUIT = p.CUIT.ToString(),
                            RazonSocial = p.RazonSocial,
                            Empresa = p.Empresa?.RazonSocial
                        };
            return DataTable(query.AsQueryable());
        }
        public ActionResult _Create()
        {
            ProveedorDTO modelo = new ProveedorDTO();
            modelo= CreateProveedorDTO(modelo);
            return PartialView(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Create([Bind("Nombre, CUIT, RazonSocial, Domicilio, RubrosSeleccionados,EmpresaId")] ProveedorDTO nuevoProveedor)
        {
            try
            {
                if (nuevoProveedor.EmpresaId == 0)
                {
                    ModelState.AddModelError("EmpresaId", "Debe seleccionar una Empresa");
                }
                if (nuevoProveedor.RubrosSeleccionados == null || nuevoProveedor.RubrosSeleccionados.Count()<1)
                {
                    ModelState.AddModelError("RubrosSeleccionados", "Debe seleccionar un Rubro");
                }
                if (ModelState.IsValid)
                {
                    Proveedor proveedor = new Proveedor
                    {
                        Nombre = nuevoProveedor.Nombre,
                        CUIT = nuevoProveedor.CUIT,
                        RazonSocial = nuevoProveedor.RazonSocial,
                        Domicilio = nuevoProveedor.Domicilio,
                        Empresa = await _context.Empresas.FindAsync(nuevoProveedor.EmpresaId),
                        Activo=true
                    };
                    await _context.Proveedores.AddAsync(proveedor);
                    foreach(int r in nuevoProveedor.RubrosSeleccionados)
                    {
                        ProveedorRubro rubro = new ProveedorRubro
                        {
                            Proveedor = proveedor,
                            Rubro = await _context.Rubros.FindAsync(r)
                        };
                        await _context.ProveedorRubros.AddAsync(rubro);
                    }
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Proveedor " + nuevoProveedor.Nombre + ".");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return PartialView(CreateProveedorDTO(nuevoProveedor));
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Proveedor. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }
        public ActionResult _Delete(int id)
        {
            Proveedor proveedor = _context.Proveedores.Find(id);
            if (proveedor != null)
            {
                ProveedorDTO deleteProveedor = new ProveedorDTO();
                deleteProveedor = CreateProveedorDTO(deleteProveedor, proveedor);
                return PartialView(deleteProveedor);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al borrar el Proveedor. Intentelo nuevamente mas tarde.");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int ProveedorId)
        {
            try
            {
                Proveedor proveedor = _context.Proveedores.Find(ProveedorId);
                if (proveedor != null)
                {
                    proveedor.Activo = false;
                    _context.Proveedores.Update(proveedor);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Proveedor " + proveedor.Nombre + ".");
                    return RedirectToAction(nameof(Index));
                }
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Proveedor. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Proveedor. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult _Update(int id)
        {
            Proveedor proveedor = _context.Proveedores.Find(id);
            if (proveedor != null)
            {
                ProveedorDTO editProveedor = new ProveedorDTO();
                editProveedor = CreateProveedorDTO(editProveedor, proveedor);
                return PartialView(editProveedor);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Proveedor. Intentelo nuevamente mas tarde.");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Update([Bind("ProveedorId, Nombre, CUIT, RazonSocial, Domicilio, RubrosSeleccionados,EmpresaId")] ProveedorDTO editProveedor)
        {
            try
            {
                if (editProveedor.EmpresaId == 0)
                {
                    ModelState.AddModelError("EmpresaId", "Debe seleccionar una Empresa");
                }
                if (editProveedor.RubrosSeleccionados == null || editProveedor.RubrosSeleccionados.Count() < 1)
                {
                    ModelState.AddModelError("RubrosSeleccionados", "Debe seleccionar un Rubro");
                }
                if (ModelState.IsValid)
                {
                    Proveedor proveedor = await _context.Proveedores.FindAsync(editProveedor.ProveedorId);
                    _context.ProveedorRubros.RemoveRange(proveedor.Rubros.Where(r=>!editProveedor.RubrosSeleccionados.Contains(r.Rubro.Id)).ToList());
                    foreach (int r in editProveedor.RubrosSeleccionados.Where(x=>!proveedor.Rubros.Select(y=>y.Rubro.Id).ToList().Contains(x)))
                    {
                        ProveedorRubro rubro = new ProveedorRubro
                        {
                            Proveedor = proveedor,
                            Rubro = await _context.Rubros.FindAsync(r)
                        };
                        await _context.ProveedorRubros.AddAsync(rubro);
                    }

                    proveedor.Nombre = editProveedor.Nombre;
                    proveedor.CUIT = editProveedor.CUIT;
                    proveedor.RazonSocial = editProveedor.RazonSocial;
                    proveedor.Domicilio = editProveedor.Domicilio;
                    proveedor.Empresa = await _context.Empresas.FindAsync(editProveedor.EmpresaId);
                    proveedor.Activo = true;
                    _context.Proveedores.Update(proveedor);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Proveedor.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return PartialView(CreateProveedorDTO(editProveedor));
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Proveedor. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> _DetalleProveedor(int Id)
        {
            Proveedor proveedor = await _context.Proveedores.FindAsync(Id);
            if (proveedor != null)
            {
                return View("_DetalleProveedor",proveedor);
            }
            else
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al acceder a la lista de Productos del Proveedor. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }            
        }

        private ProveedorDTO CreateProveedorDTO(ProveedorDTO proveedorDto,Proveedor proveedor=null)
        {
            proveedorDto.Rubros = _context.Rubros.Where(x=>x.Activo).Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            proveedorDto.Empresas = _context.Empresas.Select(g => new SelectListItem() { Text = g.RazonSocial, Value = g.Id.ToString() });
            if (proveedor != null)
            {
                proveedorDto.ProveedorId = proveedor.Id;
                proveedorDto.Nombre = proveedor.Nombre;
                proveedorDto.CUIT = proveedor.CUIT;
                proveedorDto.RazonSocial = proveedor.RazonSocial;
                proveedorDto.Domicilio = proveedor.Domicilio;
                proveedorDto.EmpresaId = proveedor.Empresa.Id;
                proveedorDto.RubrosSeleccionados = proveedor.Rubros.Select(x=>x.Rubro.Id).ToList();
            }
            return proveedorDto;
        }
        public ActionResult _Image(int Id)
        {
            Proveedor proveedor= _context.Proveedores.Find(Id);
            if (proveedor != null)
            {
                if (proveedor.Foto != null)
                {
                    ViewBag.Foto = Convert.ToBase64String(proveedor.Foto);
                }
                return PartialView(proveedor);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Imagen del Proveedor. Intentelo nuevamente mas tarde.");
            return RedirectToAction("Index", "Proveedor");
        }

        [HttpPost]
        public async Task<ActionResult> _Image(Proveedor proveedor, IFormFile FotoProveedor)
        {
            try
            {
                Proveedor proveedorEdit = await _context.Proveedores.FindAsync(proveedor.Id);
                if (FotoProveedor != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await FotoProveedor.CopyToAsync(memoryStream);
                        proveedorEdit.Foto = memoryStream.ToArray();
                    }
                }

                _context.Proveedores.Update(proveedorEdit);
                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se cargo correctamente la Foto del Proveedor.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la Foto del Producto. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Proveedor");
            }
        }
    }
}
