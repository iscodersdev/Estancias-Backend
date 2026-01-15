using Commons.Models;
using DAL.Data;
using DAL.Models;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;

namespace EstanciasCore.Controllers
{
    [Area("Core")]
    public class ListaDistribucionController : EstanciasCoreController
    {
        public ListaDistribucionController(EstanciasContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index(int Id)
        {
            breadcumb.Add(new Message() { DisplayName = "Lista de Distribución" });
            ViewBag.Breadcrumb = breadcumb;
            if (Id!=0)
            {
                ViewBag.ListaId = Id;
            }
            else
            {
                ViewBag.ListaId = null;
            }
            return View();
        }

        public async Task<IActionResult> _ListadoDistribucion(Page<ListaDistribucion> page)
        {
            var c = _context.ListaDistribucion.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/ListaDistribucion/_ListadoDistribucion",
                _context.ListaDistribucion, c);

            return PartialView("_ListadoDistribucion", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(ListaDistribucion lista)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.ListaDistribucion.AddAsync(lista);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Lista de Distribución " + lista.Nombre + ".");
                    return RedirectToAction("Index", "ListaDistribucion");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Lista de Distribución. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "ListaDistribucion");
                }
            }
            else
            {
                return PartialView(lista);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            ListaDistribucion lista = await _context.ListaDistribucion.FindAsync(Id);
            return PartialView(lista);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(ListaDistribucion lista)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.ListaDistribucion.Update(lista);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Lista de Distribución " + lista.Nombre + ".");
                    return RedirectToAction("Index", "ListaDistribucion");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Lista de Distribución. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "ListaDistribucion");
                }
            }
            else
            {
                return PartialView(lista);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                ListaDistribucion lista = _context.ListaDistribucion.Where(s => s.Id == id).First();

                List<DistribucionDestinatarios> destinatarios = _context.DistribucionDestinatarios.Where(s => s.ListaDistribucion.Id == lista.Id).ToList();

                _context.DistribucionDestinatarios.RemoveRange(destinatarios);
                _context.ListaDistribucion.Remove(lista);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Lista.");
                return RedirectToAction("Index", "ListaDistribucion");
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Lista.");
                return RedirectToAction("Index", "ListaDistribucion");
            }
        }

        /*------------------------------------------------------------ Destinatarios ---------------------------------------------------------------------------------*/

        public async Task<IActionResult> _ListadoDestinatarios(Page<DistribucionDestinatarios> page, int Id)
        {
            ListaDistribucion lista = _context.ListaDistribucion.Where(x => x.Id==Id).First();
            ViewBag.Lista = lista.Nombre;
            ViewBag.ListaId = lista.Id;

            var c = _context.DistribucionDestinatarios.Where(x=>x.ListaDistribucion.Id==Id).Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/ListaDistribucion/_ListadoDestinatarios",
                _context.DistribucionDestinatarios.Where(x => x.ListaDistribucion.Id==Id), c);

            return PartialView("_ListadoDestinatarios", page);
        }

        public IActionResult _CreateDestinatario(int Id)
        {
            ViewBag.ListaId = Id;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _CreateDestinatario(DistribucionDestinatarios destinatario, string destinatarioId)
        {
            destinatario.Id=0;
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    if (destinatarioId==null)
                    {
                        AddPageAlerts(PageAlertType.Error, "Hubo un error al agregar el Destinatario. Intentelo nuevamente mas tarde.");
                        return RedirectToAction("Index", "ListaDistribucion", new { @Id = destinatario.ListaDistribucion.Id });
                    }
                    destinatario.ListaDistribucion = _context.ListaDistribucion.Where(x=>x.Id==destinatario.ListaDistribucion.Id).FirstOrDefault();
                    destinatario.Destinatario = _context.Usuarios.Where(x=>x.Id==destinatarioId).FirstOrDefault();
                    await _context.DistribucionDestinatarios.AddAsync(destinatario);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se agregó correctamente el Destinatario " + destinatario.Destinatario.UserName + ".");
                    //return RedirectToAction("Index", "ListaDistribucion");
                    return RedirectToAction("Index", "ListaDistribucion", new { @Id = destinatario.ListaDistribucion.Id });
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al agregar el Destinatario. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "ListaDistribucion", new { @Id = destinatario.ListaDistribucion.Id });
                }
            }
            else
            {
                return PartialView(destinatario);
            }
        }

        public IActionResult DeleteDestinatarios(int id)
        {
            DistribucionDestinatarios destinatario = _context.DistribucionDestinatarios.Where(s => s.Id == id).First();
            var ListaId = destinatario.ListaDistribucion.Id;
            try
            {
                _context.DistribucionDestinatarios.Remove(destinatario);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Destinatario.");
                return RedirectToAction("Index", "ListaDistribucion", new { @Id = ListaId });
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Destinatario.");
                return RedirectToAction("Index", "ListaDistribucion", new { @Id = ListaId });
            }
        }

        public IActionResult VaciarListaDestinatarios(int id)
        {
            try
            {
                var destinatarios = _context.DistribucionDestinatarios.Where(s => s.ListaDistribucion.Id == id).ToList();
                if (destinatarios.Any())
                {
                    _context.DistribucionDestinatarios.RemoveRange(destinatarios);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se ha vaciado la lista de destinatarios correctamente.");
                }
                else
                {
                    AddPageAlerts(PageAlertType.Info, "La lista ya estaba vacía.");
                }
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al vaciar la lista de destinatarios.");
            }
            return RedirectToAction("Index", "ListaDistribucion", new { @Id = id });
        }

        public JsonResult DestinatariosComboJson(string q)
        {
            var items = _context.Usuarios
                .Where(x => x.Personas.NroDocumento.Contains(q))
                .Select(x => new
                {
                    Text = $"{x.Personas.Apellido}, {x.Personas.Nombres}",
                    Value = x.Id,
                    Subtext = $"{x.UserName}",
                    Icon = "fa fa-user"
                }).Take(10).ToArray();

            return Json(items);
        }

        public IActionResult _ImportarDestinatarios(int Id)
        {
            ViewBag.ListaId = Id;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _ImportarDestinatarios(int Id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                AddPageAlerts(PageAlertType.Error, "Por favor seleccione un archivo.");
                return RedirectToAction("Index", "ListaDistribucion", new { @Id = Id });
            }

            try
            {
                int addedCount = 0;
                int notFoundCount = 0;
                int existingCount = 0;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        if (package.Workbook.Worksheets.Count == 0)
                        {
                            AddPageAlerts(PageAlertType.Error, "El archivo Excel no contiene hojas.");
                            return RedirectToAction("Index", "ListaDistribucion", new { @Id = Id });
                        }

                        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                        var rowCount = worksheet.Dimension?.Rows ?? 0;

                        if (rowCount < 2)
                        {
                            AddPageAlerts(PageAlertType.Error, "El archivo no contiene datos.");
                            return RedirectToAction("Index", "ListaDistribucion", new { @Id = Id });
                        }

                        var lista = await _context.ListaDistribucion.FindAsync(Id);

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var dni = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            if (!string.IsNullOrEmpty(dni))
                            {
                                var usuario = _context.Usuarios.FirstOrDefault(u => u.Personas.NroDocumento == dni);

                                if (usuario != null)
                                {
                                    bool exists = _context.DistribucionDestinatarios.Any(dd => dd.ListaDistribucion.Id == Id && dd.Destinatario.Id == usuario.Id);
                                    if (!exists)
                                    {
                                        var nuevoDestinatario = new DistribucionDestinatarios
                                        {
                                            ListaDistribucion = lista,
                                            Destinatario = usuario
                                        };
                                        _context.DistribucionDestinatarios.Add(nuevoDestinatario);
                                        addedCount++;
                                    }
                                    else
                                    {
                                        existingCount++;
                                    }
                                }
                                else
                                {
                                    notFoundCount++;
                                }
                            }
                        }

                        if (addedCount > 0)
                        {
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                string msg = $"Proceso finalizado. Agregados: {addedCount}.";
                if (existingCount > 0) msg += $" Ya existían: {existingCount}.";
                if (notFoundCount > 0) msg += $" No encontrados (DNI inexistente): {notFoundCount}.";

                AddPageAlerts(PageAlertType.Success, msg);
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Error al importar el archivo: " + ex.Message);
            }

            return RedirectToAction("Index", "ListaDistribucion", new { @Id = Id });
        }

        public IActionResult DescargarPlantilla()
        {
            string sWebRootFolder = _context.GetType() == null ? "" : ""; // Dummy usage to avoid static if needed, but easier to just use MemoryStream
            // Actually better to just generate byte array.
            
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Plantilla");
                worksheet.Cells[1, 1].Value = "DNI";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                
                // Add some example data maybe? No, just header is cleaner.
                
                var content = package.GetAsByteArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PlantillaDestinatarios.xlsx");
            }
        }

    }
}
