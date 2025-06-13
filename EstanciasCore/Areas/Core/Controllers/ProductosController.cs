using EstanciasCore.Controllers;
using DAL.Data;
using DAL.DTOs;
using DAL.Models.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EstanciasCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class ProductosController : EstanciasCoreController
    {
        public ProductosController(EstanciasContext context) : base(context)
        {
        }
        public IActionResult _ListaProductos(int Id)
        {
            ViewBag.ProveedorId = Id;
            return PartialView();
        }
        public IActionResult ProductosDataTable(int ProveedorId)
        {
            List<Producto> productos = _context.Productos.Where(x => x.Activo && x.Proveedor.Id==ProveedorId).ToList();
            var query = from p in productos
                        select new ListProductosDTO
                        {
                            Id = p.Id,
                            Descripcion = p.Descripcion?.ToUpper(),
                            DescripcionAmpliada = p.DescripcionAmpliada?.ToUpper(),
                            Precio = p.Precio, 
                            Oferta = (p.Oferta!=null)?p.Oferta.ToString():"---", 
                            Financiable = (p.Financiable)?"SI":"NO", 
                            Rubro = p.Rubro?.Nombre?.ToString()

                        };
            return DataTable(query.AsQueryable());
        }
        public ActionResult _Create(int Id)
        {
            ProductoDTO modelo = new ProductoDTO();
            modelo = CreateProductoDTO(modelo,Id);
            return PartialView(modelo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Create([Bind("Descripcion, ProveedorId, RubroSeleccionado,DescripcionAmpliada,Precio,Oferta,Financiable")] ProductoDTO nuevoProducto)
        {
            try
            {
                if (nuevoProducto.ProveedorId<1)
                {
                    ModelState.AddModelError("ProveedorId", "Debe tener asignado un Proveedor.");
                }
                if (nuevoProducto.RubroSeleccionado < 1)
                {
                    ModelState.AddModelError("RubroSeleccionado", "Debe seleccionar un Rubro");
                }
                if (ModelState.IsValid)
                {
                    Producto producto = new Producto
                    {
                        Proveedor = await _context.Proveedores.FindAsync(nuevoProducto.ProveedorId),
                        Descripcion = nuevoProducto.Descripcion,
                        DescripcionAmpliada = nuevoProducto.DescripcionAmpliada,
                        Precio= nuevoProducto.Precio,
                        Oferta= nuevoProducto.Oferta,
                        Financiable= nuevoProducto.Financiable,
                        Rubro = await _context.Rubros.FindAsync(nuevoProducto.RubroSeleccionado),
                        Activo = true
                    };
                    await _context.Productos.AddAsync(producto);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Producto " + nuevoProducto.Descripcion + ".");
                    return RedirectToAction("_DetalleProveedor","Proveedor", new { @Id = nuevoProducto.ProveedorId } );
                }
                else
                {
                    return PartialView(CreateProductoDTO(nuevoProducto, nuevoProducto.ProveedorId));
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Producto. Intentelo nuevamente mas tarde.");
                return RedirectToAction("_DetalleProveedor", "Proveedor", new { @Id = nuevoProducto.ProveedorId });
            }
        }
        public ActionResult _Delete(int id)
        {
            Producto producto = _context.Productos.Find(id);
            if (producto != null)
            {
                ProductoDTO deleteProducto = new ProductoDTO();
                deleteProducto = CreateProductoDTO(deleteProducto,producto.Proveedor.Id, producto);
                return PartialView(deleteProducto);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al borrar el Producto. Intentelo nuevamente mas tarde.");
            return RedirectToAction("Index","Proveedor");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int ProductoId)
        {
            try
            {
                Producto producto = _context.Productos.Find(ProductoId);
                if (producto != null)
                {
                    producto.Activo = false;
                    _context.Productos.Update(producto);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Producto " + producto.Descripcion + ".");
                    return RedirectToAction("_DetalleProveedor", "Proveedor",new { @Id = producto.Proveedor.Id});
                }
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Producto. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Proveedor");
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Producto. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Proveedor");
            }
        }

        public ActionResult _Update(int id)
        {
            Producto producto = _context.Productos.Find(id);
            if (producto != null)
            {
                ProductoDTO editProducto = new ProductoDTO();
                editProducto = CreateProductoDTO(editProducto,producto.Proveedor.Id, producto);
                return PartialView(editProducto);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Producto. Intentelo nuevamente mas tarde.");
            return RedirectToAction("Index", "Proveedor");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Update([Bind("ProveedorId,ProductoId,Descripcion,RubroSeleccionado,DescripcionAmpliada,Precio,Oferta,Financiable")] ProductoDTO editProducto)
        {
            try
            {
                if (editProducto.ProveedorId < 1)
                {
                    ModelState.AddModelError("ProveedorId", "Debe tener asignado un Proveedor.");
                }
                if (editProducto.RubroSeleccionado < 1)
                {
                    ModelState.AddModelError("RubroSeleccionado", "Debe seleccionar un Rubro");
                }
                if (ModelState.IsValid)
                {
                    Producto producto = await _context.Productos.FindAsync(editProducto.ProductoId);
                    producto.Descripcion = editProducto.Descripcion;
                    producto.DescripcionAmpliada  = editProducto.DescripcionAmpliada;
                    producto.Precio = editProducto.Precio;
                    producto.Oferta = editProducto.Oferta;
                    producto.Financiable = editProducto.Financiable;
                    producto.Proveedor = await _context.Proveedores.FindAsync(editProducto.ProveedorId);
                    producto.Rubro = await _context.Rubros.FindAsync(editProducto.RubroSeleccionado);
                    producto.Activo = true;
                    _context.Productos.Update(producto);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Producto.");
                    return RedirectToAction("_DetalleProveedor", "Proveedor", new { @Id = editProducto.ProveedorId});
                }
                else
                {
                    return PartialView(CreateProductoDTO(editProducto,editProducto.ProveedorId));
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Producto. Intentelo nuevamente mas tarde.");
                return RedirectToAction("_DetalleProveedor", "Proveedor", new { @Id = editProducto.ProveedorId });
            }
        }

        public ActionResult _Image(int Id)
        {
            ProductoAdjuntosDTO product = new ProductoAdjuntosDTO();
            List<ProductoAdjuntos> adjuntos = _context.ProductoAdjuntos.Where(X=>X.Producto.Id==Id).ToList();
            //Producto producto = _context.Productos.Find(Id);
            product.ProductoId = Id;
            product.ProductoAdjuntos = adjuntos;
            /*
            if (producto != null)
            {
                if (producto.Foto != null)
                {
                    ViewBag.Foto = Convert.ToBase64String(producto.Foto);
                }
              
            }*/
            return PartialView(product);
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Imagen del Producto. Intentelo nuevamente mas tarde.");
            return RedirectToAction("Index", "Proveedor");
        }
        [HttpGet]
        public IActionResult _Financiacion(int id)
        {
            Producto p = _context.Productos.Find(id);
            ListFinanciacionDTO lista = new ListFinanciacionDTO();
            lista = CreateListFinanciacionDTO(lista, p);
            return PartialView(lista);
        }

        [HttpPost]
        public async Task<IActionResult> _Financiacion(ListFinanciacionDTO financiacion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Producto p = await _context.Productos.FindAsync(financiacion.ProductoId);
                    await _context.FinanciacionProductos.AddAsync(new Financiacion { 
                        CantidadCuotas=Convert.ToInt32(financiacion.CantidadCuotasAdd),
                        InteresesPorCuota=Convert.ToDecimal(financiacion.InteresCoutasAdd),
                        Producto=p
                    });
                    await _context.SaveChangesAsync();
                    financiacion.InteresCoutasAdd = null;
                    financiacion.CantidadCuotasAdd = null;
                    ModelState.Clear();
                    return PartialView(CreateListFinanciacionDTO(financiacion,p));
                }
                catch (System.Exception)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al Cargar la Finaciación del Producto. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Proveedor");
                }
            }
            else
            {
                Producto p = await _context.Productos.FindAsync(financiacion.ProductoId);
                return PartialView(CreateListFinanciacionDTO(financiacion, p));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFinanciacion(int Id)
        {
            Financiacion financiacion = await _context.FinanciacionProductos.FindAsync(Id);
            try
            {
                _context.FinanciacionProductos.Remove(financiacion);
                await _context.SaveChangesAsync();

                return Ok(HttpStatusCode.OK);
            }
            catch (System.Exception)
            {
                return Ok(HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        public async Task<ActionResult> _Image(ProductoAdjuntosDTO producto, List<IFormFile> FotoProducto)
        {
            try
            {
                Producto productoEdit = _context.Productos.Find(producto.ProductoId);
               
                foreach (var itemFotoProducto in FotoProducto)
                {
                    ProductoAdjuntos productoAdjuntos = new ProductoAdjuntos();
                    if (itemFotoProducto != null)
                    {
                        productoAdjuntos.Nombre = Path.GetFileNameWithoutExtension(itemFotoProducto.FileName);
                        productoAdjuntos.Extension = Path.GetExtension(itemFotoProducto.FileName);
                        using (var memoryStream = new MemoryStream())
                        {
                            await itemFotoProducto.CopyToAsync(memoryStream);
                            productoAdjuntos.Producto = productoEdit;
                            productoAdjuntos.Foto = memoryStream.ToArray();
                        }
                        _context.ProductoAdjuntos.Add(productoAdjuntos);
                        await _context.SaveChangesAsync();
                    }
                }
            

                AddPageAlerts(PageAlertType.Success, "Se cargo correctamente la Foto del Producto.");
                return RedirectToAction("_DetalleProveedor", "Proveedor", new { @Id = productoEdit.Proveedor.Id });
            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la Foto del Producto. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Proveedor");
            }
        }
        
        [HttpGet]
        public async Task<bool> EliminarFotoProducto(int Id)
        {
            try
            {
                ProductoAdjuntos adjunto = _context.ProductoAdjuntos.Find(Id);
                _context.ProductoAdjuntos.Remove(adjunto);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        public IActionResult _DescargaProductosExcel(int ProveedorId)
        {
            var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Plantillas/PlantillaProductos.xlsx"));
            using (var package = new ExcelPackage(memoryStream))
            {
                var workSheet = package.Workbook.Worksheets[1];
                var renglones = _context.Productos.Where(x => x.Proveedor.Id==ProveedorId && x.Activo);
                int linea = 2;
                foreach (var renglon in renglones)
                {
                    workSheet.Cells[linea, 1].Value = renglon.Proveedor.Nombre;
                    workSheet.Cells[linea, 2].Value = renglon.Rubro.Nombre;
                    workSheet.Cells[linea, 3].Value = renglon.Descripcion;
                    workSheet.Cells[linea, 4].Value = renglon.DescripcionAmpliada;
                    workSheet.Cells[linea, 5].Value = decimal.Round(renglon.Precio,2).ToString();
                    workSheet.Cells[linea, 6].Value = (renglon.Oferta != null) ? decimal.Round((decimal)renglon.Oferta, 2).ToString():"---" ;
                    workSheet.Cells[linea, 7].Value = (renglon.Financiable)? "SI":"NO" ;
                    linea = linea + 1;
                }
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Productos del Proveedor " + renglones.FirstOrDefault()?.Proveedor?.Nombre + ".xlsx");
            }
        }
        private ProductoDTO CreateProductoDTO(ProductoDTO productoDTO,int ProveedorId, Producto producto = null)
        {
            productoDTO.Rubros = _context.Proveedores.Find(ProveedorId).Rubros.Select(x=>x.Rubro).Where(n=>n.Activo).Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            productoDTO.ProveedorId = ProveedorId;
            if (producto != null)
            {
                productoDTO.ProductoId = producto.Id;
                productoDTO.Descripcion = producto.Descripcion;
                productoDTO.DescripcionAmpliada = producto.DescripcionAmpliada;
                productoDTO.Precio = producto.Precio;
                productoDTO.Oferta = producto.Oferta;
                productoDTO.Financiable = producto.Financiable;
                productoDTO.RubroSeleccionado = producto.Rubro.Id;                
            }
            return productoDTO;
        }

        private ListFinanciacionDTO CreateListFinanciacionDTO(ListFinanciacionDTO lista,Producto producto)
        {
            lista.PrecioProducto = producto.Precio;
            lista.ProductoId = producto.Id;
            lista.ProductoNombre = producto.Descripcion;
            foreach (Financiacion f in producto.FinanciacionProducto)
            {
                lista.ListaFinanciacion.Add(new FinanciacionDTO { 
                    FinaciacionId=f.Id,
                    CantidadCuotas=f.CantidadCuotas,
                    InteresCuota=f.InteresesPorCuota,
                    ValorCuota=decimal.Round((producto.Precio/f.CantidadCuotas)*(1+(f.InteresesPorCuota/100)),2)
                });
            }
            return lista;
        }

    }
}
