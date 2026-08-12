using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using DAL.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class PreRegistroCategoriasController : Controller
    {
        private readonly EstanciasContext _context;

        public PreRegistroCategoriasController(EstanciasContext context)
        {
            _context = context;
        }

        // GET: Administracion/PreRegistroCategorias
        public async Task<IActionResult> Index()
        {
            return View(await _context.PreRegistroCategorias.ToListAsync());
        }

        // GET: Administracion/PreRegistroCategorias/_Create
        public IActionResult _Create()
        {
            return PartialView();
        }

        // POST: Administracion/PreRegistroCategorias/_Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(PreRegistroCategoriaDTO model)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                // Verify if DNI already exists to avoid duplicates
                if (await _context.PreRegistroCategorias.AnyAsync(p => p.DNI == model.DNI))
                {
                    ModelState.AddModelError("DNI", "El DNI ingresado ya existe.");
                    return PartialView(model);
                }

                var entity = new PreRegistroCategorias
                {
                    DNI = model.DNI,
                    NombreCompleto = model.NombreCompleto,
                    Categoria = model.Categoria
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView(model);
        }

        // GET: Administracion/PreRegistroCategorias/_Edit/5
        public async Task<IActionResult> _Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.PreRegistroCategorias.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var model = new PreRegistroCategoriaDTO
            {
                Id = entity.Id,
                DNI = entity.DNI,
                NombreCompleto = entity.NombreCompleto,
                Categoria = entity.Categoria
            };

            return PartialView(model);
        }

        // POST: Administracion/PreRegistroCategorias/_Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Edit(int id, PreRegistroCategoriaDTO model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var entity = await _context.PreRegistroCategorias.FindAsync(id);
                if (entity == null)
                {
                    return NotFound();
                }

                // Verify if another record has the same DNI
                if (await _context.PreRegistroCategorias.AnyAsync(p => p.DNI == model.DNI && p.Id != id))
                {
                    ModelState.AddModelError("DNI", "El DNI ingresado ya está asignado a otro registro.");
                    return PartialView(model);
                }

                entity.DNI = model.DNI;
                entity.NombreCompleto = model.NombreCompleto;
                entity.Categoria = model.Categoria;

                try
                {
                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreRegistroCategoriasExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return PartialView(model);
        }

        // GET: Administracion/PreRegistroCategorias/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.PreRegistroCategorias.FindAsync(id);
            if (entity != null)
            {
                _context.PreRegistroCategorias.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Registro eliminado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PreRegistroCategoriasExists(int id)
        {
            return _context.PreRegistroCategorias.Any(e => e.Id == id);
        }

        // GET: Administracion/PreRegistroCategorias/_Importar
        public IActionResult _Importar()
        {
            return PartialView();
        }

        // GET: Administracion/PreRegistroCategorias/DescargarPlantilla
        public IActionResult DescargarPlantilla()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Plantilla");
                worksheet.Cells[1, 1].Value = "DNI";
                worksheet.Cells[1, 2].Value = "NombreCompleto";
                worksheet.Cells[1, 3].Value = "Categoria";

                // Formateo de la cabecera
                using (var range = worksheet.Cells[1, 1, 1, 3])
                {
                    range.Style.Font.Bold = true;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_PreRegistroCategorias.xlsx");
            }
        }

        // POST: Administracion/PreRegistroCategorias/Importar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Importar(IFormFile archivo)
        {
            if (archivo == null || archivo.Length <= 0)
            {
                TempData["Error"] = "Por favor, seleccione un archivo válido.";
                return RedirectToAction(nameof(Index));
            }

            var extension = Path.GetExtension(archivo.FileName).ToLower();

            if (extension == ".xlsx")
            {
                await ImportarExcel(archivo);
            }
            else if (extension == ".csv")
            {
                await ImportarCsv(archivo);
            }
            else
            {
                TempData["Error"] = "Formato de archivo no soportado. Use .xlsx o .csv";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task ImportarExcel(IFormFile archivo)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await archivo.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault(); // Get first sheet
                        if (worksheet == null) throw new Exception("No se encontraron hojas en el archivo Excel.");
                        
                        var rowCount = worksheet.Dimension?.Rows ?? 0;

                        // Start from row 2 assuming row 1 is header
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var dni = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            var nombreCompleto = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                            var categoria = worksheet.Cells[row, 3].Value?.ToString()?.Trim();

                            if (!string.IsNullOrEmpty(dni))
                            {
                                await ProcessRecord(dni, nombreCompleto, categoria);
                            }
                        }
                    }
                }
                TempData["Success"] = "Importación de Excel completada.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al procesar el Excel: {ex.Message}";
            }
        }

        private async Task ImportarCsv(IFormFile archivo)
        {
            try
            {
                using (var reader = new StreamReader(archivo.OpenReadStream()))
                {
                    var isFirstRow = true;
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (isFirstRow)
                        {
                            isFirstRow = false; // Skip header
                            continue;
                        }

                        // Handle both comma and semicolon separators
                        char separator = line.Contains(";") ? ';' : ',';
                        var values = line.Split(separator);

                        if (values.Length >= 3)
                        {
                            var dni = values[0]?.Trim();
                            var nombreCompleto = values[1]?.Trim();
                            var categoria = values[2]?.Trim();

                            if (!string.IsNullOrEmpty(dni))
                            {
                                await ProcessRecord(dni, nombreCompleto, categoria);
                            }
                        }
                    }
                }
                TempData["Success"] = "Importación de CSV completada.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al procesar el CSV: {ex.Message}";
            }
        }

        private async Task ProcessRecord(string dni, string nombreCompleto, string categoria)
        {
            var existingRecord = await _context.PreRegistroCategorias.FirstOrDefaultAsync(p => p.DNI == dni);

            if (existingRecord != null)
            {
                // Update
                existingRecord.NombreCompleto = nombreCompleto ?? existingRecord.NombreCompleto;
                existingRecord.Categoria = categoria ?? existingRecord.Categoria;
                _context.Update(existingRecord);
            }
            else
            {
                // Insert
                var newRecord = new PreRegistroCategorias
                {
                    DNI = dni,
                    NombreCompleto = nombreCompleto ?? "",
                    Categoria = categoria ?? ""
                };
                _context.Add(newRecord);
            }

            await _context.SaveChangesAsync();
        }
    }
}
