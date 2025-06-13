using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Cors;
using DAL.Data;
using DAL.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Commons.Identity.Services;
using EstanciasCore.Services;
using System.Net.Http;
using EstanciasCore.API.Controllers;
using DAL.Models.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace EstanciasCore.Controllers
{
    [Route("api/[controller]")]
    public class MProductosController : BaseApiController
    {
        private readonly UserService<Usuario> _userManager;
        private readonly NotificacionAPIService _notificacionAPIService;
        public EstanciasContext _context;

        public MProductosController(EstanciasContext context, UserService<Usuario> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("TraeProveedores")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeProveedoresDTO TraeProveedores([FromBody] MTraeProveedoresDTO uat)
        {
            /*var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }*/
            uat.Status = 200;
            uat.Mensaje = "Listado Proveedores";
            IEnumerable<Proveedor> Proveedores;
            if (uat.ProveedorId!=null && uat.ProveedorId != 0)
            {
                Proveedores= _context.Proveedores.Where(x => x.Activo == true && x.Id==uat.ProveedorId);
            }
            else
            {
                Proveedores = _context.Proveedores.Where(x => x.Activo == true);
            }            
            List<ProveedoresDTO> lista = new List<ProveedoresDTO>();
            foreach (var Proveedor in Proveedores )
            {
                var renglon = new ProveedoresDTO();
                renglon.Id = Proveedor.Id;
                renglon.Nombre = Proveedor.Nombre;
                renglon.CUIT = Proveedor.CUIT.ToString();
                renglon.RazonSocial = Proveedor.RazonSocial;
                renglon.Domicilio = Proveedor.Domicilio.ToString();
                renglon.Foto = Proveedor.Foto;
                lista.Add(renglon);
            }
            uat.Proveedores  = lista;
            return uat;
        }
        [HttpPost]
        [Route("TraeClientes")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeClientesDTO TraeClientes([FromBody] MTraeClientesDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            uat.Status = 200;
            uat.Mensaje = "Listado Clientes";
            IEnumerable<Clientes> Clientes;
            if (uat.ClientesId.Count()>0)
            {
                Clientes = _context.Clientes.Where(x => x.ClienteValidado == true && x.Persona!=null && uat.ClientesId.Contains(x.Id));
            }
            else
            {
                Clientes = _context.Clientes.Where(x => x.ClienteValidado == true && x.Persona != null);
            }
            List<MClienteDTO> lista = new List<MClienteDTO>();
            foreach (var c in Clientes)
            {
                var renglon = new MClienteDTO();
                renglon.Id = c.Id;
                if (c.Persona != null)
                {
                    renglon.Nombre = c.Persona.GetNombreCompleto();
                    renglon.NroDocumento = c.Persona.NroDocumento;
                }
                renglon.Telefono = c.Telefono;
                renglon.Celular = c.Celular;
                renglon.Domicilio = c.Domicilio;
                renglon.Mail = c.Usuario?.Mail;
                renglon.LocalidadId = c.Localidad?.Id;
                renglon.Localidad = c.Localidad?.Descripcion;
                renglon.DeviceId = c.Usuario?.DeviceId;
                lista.Add(renglon);
            }
            uat.Clientes = lista;
            return uat;
        }
        [HttpPost]
        [Route("TraeRubros")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeRubrosDTO TraeRubros([FromBody] MTraeRubrosDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            uat.Status = 200;
            uat.Mensaje = "Listado Rubros";
            var Rubros = _context.Rubros.Where(x => x.Activo == true);
            List<RubrosDTO> lista = new List<RubrosDTO>();
            foreach (var Rubro in Rubros)
            {
                lista.Add(new RubrosDTO { 
                    Id=Rubro.Id,
                    Nombre=Rubro.Nombre,
                    Descripcion=Rubro.Descripcion
                });
            }
            uat.Rubros = lista;
            return uat;
        }
        [HttpPost]
        [Route("TraeProductos")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeProductosDTO TraeProductos([FromBody] MTraeProductosDTO uat)
        {
            /*var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }*/
            uat.Status = 200;
            uat.Mensaje = "Listado Productos";
            if (uat.ProductoId != null && uat.ProductoId != 0)
            {
               //Método Viejo
               //uat.Productos = _context.Productos
               //    .Where(x => x.Precio > 0 && x.Id == uat.ProductoId)
               //    .Select(x => new MProductosDTO { Descripcion = x.Descripcion, Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "", ProveedorId = (x.Proveedor != null) ? x.Proveedor.Id : 0, ProveedorNombre = (x.Proveedor != null) ? x.Proveedor.Nombre : "", DescripcionAmpliada = x.DescripcionAmpliada, Foto = x.Foto, Id = x.Id, Precio = x.Precio, Activo=x.Activo })
               //    .ToList();

                var query = (from producto in _context.Productos
                             join proveedor in _context.Proveedores
                                 on producto.Proveedor.Id equals proveedor.Id
                             join rubro in _context.Rubros
                                on producto.Rubro.Id equals rubro.Id
                             where producto.Precio > 0 && producto.Id == uat.ProductoId
                             select new MProductosDTO { Descripcion = producto.Descripcion, Rubro = (producto.Rubro != null) ? producto.Rubro.Nombre : "", ProveedorId = (producto.Proveedor != null) ? producto.Proveedor.Id : 0, ProveedorNombre = (producto.Proveedor != null) ? producto.Proveedor.Nombre : "", DescripcionAmpliada = producto.DescripcionAmpliada, Id = producto.Id, Precio = producto.Precio, Activo = producto.Activo });
                uat.Productos = query.ToList();
            }else if (uat.ProveedorId != null)
            {
                if (uat.ProductoId != null && uat.ProductoId != 0)
                {
                    //uat.Productos = _context.Productos
                    //   .Where(x => x.Precio > 0 && x.Id == uat.ProductoId && x.Proveedor.Id == uat.ProveedorId)
                    //   .Select(x => new MProductosDTO { Descripcion = x.Descripcion, Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "", ProveedorId = (x.Proveedor != null) ? x.Proveedor.Id : 0, ProveedorNombre = (x.Proveedor != null) ? x.Proveedor.Nombre : "", DescripcionAmpliada = x.DescripcionAmpliada, Foto = x.Foto, Id = x.Id, Precio = x.Precio, Activo = x.Activo })
                    //   .ToList();

                    var query = (from producto in _context.Productos
                                 join proveedor in _context.Proveedores
                                     on producto.Proveedor.Id equals proveedor.Id
                                 join rubro in _context.Rubros
                                    on producto.Rubro.Id equals rubro.Id
                                 where producto.Precio > 0 && producto.Id == uat.ProductoId && producto.Proveedor.Id == uat.ProveedorId
                                 select new MProductosDTO { Descripcion = producto.Descripcion, Rubro = (producto.Rubro != null) ? producto.Rubro.Nombre : "", ProveedorId = (producto.Proveedor != null) ? producto.Proveedor.Id : 0, ProveedorNombre = (producto.Proveedor != null) ? producto.Proveedor.Nombre : "", DescripcionAmpliada = producto.DescripcionAmpliada, Id = producto.Id, Precio = producto.Precio, Activo = producto.Activo });
                    uat.Productos = query.ToList();
                }
                else if(uat.Cantidad!=null && uat.Cantidad != 0)
                {
                    //uat.Productos = _context.Productos
                    //    .Where(x => x.Precio > 0 && x.Proveedor.Id == uat.ProveedorId).Take((int)uat.Cantidad)
                    //    .Select(x => new MProductosDTO { Descripcion = x.Descripcion, Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "", ProveedorId = (x.Proveedor != null) ? x.Proveedor.Id : 0, ProveedorNombre = (x.Proveedor != null) ? x.Proveedor.Nombre : "", DescripcionAmpliada = x.DescripcionAmpliada, Foto = x.Foto, Id = x.Id, Precio = x.Precio, Activo = x.Activo })
                    //    .ToList();

                    var query = (from producto in _context.Productos
                                 join proveedor in _context.Proveedores
                                     on producto.Proveedor.Id equals proveedor.Id
                                 join rubro in _context.Rubros
                                    on producto.Rubro.Id equals rubro.Id
                                 where producto.Precio > 0 && producto.Proveedor.Id == uat.ProveedorId
                                 select new MProductosDTO { Descripcion = producto.Descripcion, Rubro = (producto.Rubro != null) ? producto.Rubro.Nombre : "", ProveedorId = (producto.Proveedor != null) ? producto.Proveedor.Id : 0, ProveedorNombre = (producto.Proveedor != null) ? producto.Proveedor.Nombre : "", DescripcionAmpliada = producto.DescripcionAmpliada, Id = producto.Id, Precio = producto.Precio, Activo = producto.Activo });
                    uat.Productos = query.Take((int)uat.Cantidad).ToList();
                }
                else
                {
                    //uat.Productos = _context.Productos
                    //    .Where(x => x.Precio > 0 && x.Proveedor.Id == uat.ProveedorId)
                    //    .Select(x => new MProductosDTO { Descripcion = x.Descripcion, Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "", ProveedorId = (x.Proveedor != null) ? x.Proveedor.Id : 0, ProveedorNombre = (x.Proveedor != null) ? x.Proveedor.Nombre : "", DescripcionAmpliada = x.DescripcionAmpliada, Foto = x.Foto, Id = x.Id, Precio = x.Precio, Activo = x.Activo })
                    //    .ToList();

                    var query = (from producto in _context.Productos
                                 join proveedor in _context.Proveedores
                                     on producto.Proveedor.Id equals proveedor.Id
                                 join rubro in _context.Rubros
                                    on producto.Rubro.Id equals rubro.Id
                                 where producto.Precio > 0 && producto.Proveedor.Id == uat.ProveedorId
                                 select new MProductosDTO { Descripcion = producto.Descripcion, Rubro = (producto.Rubro != null) ? producto.Rubro.Nombre : "", ProveedorId = (producto.Proveedor != null) ? producto.Proveedor.Id : 0, ProveedorNombre = (producto.Proveedor != null) ? producto.Proveedor.Nombre : "", DescripcionAmpliada = producto.DescripcionAmpliada, Id = producto.Id, Precio = producto.Precio, Activo = producto.Activo });
                    uat.Productos = query.ToList();
                }
            }
            else if (uat.ClienteId != null)
            {

                if (uat.ProductoId != null && uat.ProductoId != 0)
                {
                    //uat.Productos = _context.ProductosClientes.Where(x =>x.Id==uat.ProductoId && x.Cliente.Id == uat.ClienteId && x.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id)
                    //    .Select(x => new MProductosDTO
                    //    {
                    //        Descripcion = x.Descripcion,
                    //        Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "",
                    //        ClienteId = (x.Cliente != null) ? x.Cliente.Id : 0,
                    //        ClienteNombre = (x.Cliente != null && x.Cliente.Persona != null) ? x.Cliente.Persona.GetNombreCompleto() : "",
                    //        ClienteMail = (x.Cliente != null && x.Cliente.Usuario != null) ? x.Cliente.Usuario.Mail : "",
                    //        ClienteCel = (x.Cliente != null) ? x.Cliente.Celular : "",
                    //        DescripcionAmpliada = x.DescripcionAmpliada,
                    //        Foto = x.Foto,
                    //        Id = x.Id,
                    //        Precio = x.Precio,
                    //        Activo = x.Activo
                    //    })
                    //    .ToList();


                    var query = (from productoCli in _context.ProductosClientes
                                 join cliente in _context.Clientes
                                     on productoCli.Cliente.Id equals cliente.Id
                                 join rubro in _context.Rubros
                                    on productoCli.Rubro.Id equals rubro.Id
                                 join localidad in _context.Localidad
                                    on productoCli.Cliente.Localidad.Id equals localidad.Id
                                 //where productoCli.Id == uat.ProductoId && productoCli.Cliente.Id == uat.ClienteId && productoCli.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id
                                 where productoCli.Id == uat.ProductoId && productoCli.Cliente.Id == uat.ClienteId
                                 select new MProductosDTO
                                 {
                                     Descripcion = productoCli.Descripcion,
                                     Rubro = (productoCli.Rubro != null) ? productoCli.Rubro.Nombre : "",
                                     ClienteId = (productoCli.Cliente != null) ? productoCli.Cliente.Id : 0,
                                     ClienteNombre = (productoCli.Cliente != null && productoCli.Cliente.Persona != null) ? productoCli.Cliente.Persona.GetNombreCompleto() : "",
                                     ClienteMail = (productoCli.Cliente != null && productoCli.Cliente.Usuario != null) ? productoCli.Cliente.Usuario.Mail : "",
                                     ClienteCel = (productoCli.Cliente != null) ? productoCli.Cliente.Celular : "",
                                     DescripcionAmpliada = productoCli.DescripcionAmpliada,
                                     Id = productoCli.Id,
                                     Precio = productoCli.Precio,
                                     Activo = productoCli.Activo
                                 });
                    uat.Productos = query.ToList();

                }
                else if(uat.Cantidad!=null && uat.ProductoId != 0)
                {
                    //uat.Productos = _context.ProductosClientes.Where(x => x.Cliente.Id == uat.ClienteId && x.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id)
                    //    .Take((int)uat.Cantidad)
                    //    .Select(x => new MProductosDTO
                    //    {
                    //        Descripcion = x.Descripcion,
                    //        Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "",
                    //        ClienteId = (x.Cliente != null) ? x.Cliente.Id : 0,
                    //        ClienteNombre = (x.Cliente != null && x.Cliente.Persona != null) ? x.Cliente.Persona.GetNombreCompleto() : "",
                    //        ClienteMail = (x.Cliente != null && x.Cliente.Usuario != null) ? x.Cliente.Usuario.Mail : "",
                    //        ClienteCel = (x.Cliente != null) ? x.Cliente.Celular : "",
                    //        DescripcionAmpliada = x.DescripcionAmpliada,
                    //        Foto = x.Foto,
                    //        Id = x.Id,
                    //        Precio = x.Precio,
                    //        Activo = x.Activo
                    //    })
                    //    .ToList();

                    var query = (from productoCli in _context.ProductosClientes
                                 join cliente in _context.Clientes
                                     on productoCli.Cliente.Id equals cliente.Id
                                 join rubro in _context.Rubros
                                    on productoCli.Rubro.Id equals rubro.Id
                                 join localidad in _context.Localidad
                                    on productoCli.Cliente.Localidad.Id equals localidad.Id
                                 //where productoCli.Cliente.Id == uat.ClienteId && productoCli.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id
                                 where productoCli.Cliente.Id == uat.ClienteId
                                 select new MProductosDTO
                                 {
                                     Descripcion = productoCli.Descripcion,
                                     Rubro = (productoCli.Rubro != null) ? productoCli.Rubro.Nombre : "",
                                     ClienteId = (productoCli.Cliente != null) ? productoCli.Cliente.Id : 0,
                                     ClienteNombre = (productoCli.Cliente != null && productoCli.Cliente.Persona != null) ? productoCli.Cliente.Persona.GetNombreCompleto() : "",
                                     ClienteMail = (productoCli.Cliente != null && productoCli.Cliente.Usuario != null) ? productoCli.Cliente.Usuario.Mail : "",
                                     ClienteCel = (productoCli.Cliente != null) ? productoCli.Cliente.Celular : "",
                                     DescripcionAmpliada = productoCli.DescripcionAmpliada,
                                     Id = productoCli.Id,
                                     Precio = productoCli.Precio,
                                     Activo = productoCli.Activo
                                 });
                    uat.Productos = query.Take((int)uat.Cantidad).ToList();
                }
                else
                {
                    //uat.Productos = _context.ProductosClientes.Where(x =>x.Cliente.Id == uat.ClienteId && x.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id)
                    //    .Select(x => new MProductosDTO
                    //    {
                    //        Descripcion = x.Descripcion,
                    //        Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "",
                    //        ClienteId = (x.Cliente != null) ? x.Cliente.Id : 0,
                    //        ClienteNombre = (x.Cliente != null && x.Cliente.Persona != null) ? x.Cliente.Persona.GetNombreCompleto() : "",
                    //        ClienteMail = (x.Cliente != null && x.Cliente.Usuario != null) ? x.Cliente.Usuario.Mail : "",
                    //        ClienteCel = (x.Cliente != null) ? x.Cliente.Celular : "",
                    //        DescripcionAmpliada = x.DescripcionAmpliada,
                    //        Foto = x.Foto,
                    //        Id = x.Id,
                    //        Precio = x.Precio,
                    //        Activo = x.Activo
                    //    })
                    //    .ToList();

                    var query = (from productoCli in _context.ProductosClientes
                                 join cliente in _context.Clientes
                                     on productoCli.Cliente.Id equals cliente.Id
                                 join rubro in _context.Rubros
                                    on productoCli.Rubro.Id equals rubro.Id
                                 join localidad in _context.Localidad
                                    on productoCli.Cliente.Localidad.Id equals localidad.Id
                                 //where productoCli.Cliente.Id == uat.ClienteId && productoCli.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id
                                 where productoCli.Cliente.Id == uat.ClienteId
                                 select new MProductosDTO
                                 {
                                     Descripcion = productoCli.Descripcion,
                                     Rubro = (productoCli.Rubro != null) ? productoCli.Rubro.Nombre : "",
                                     ClienteId = (productoCli.Cliente != null) ? productoCli.Cliente.Id : 0,
                                     ClienteNombre = (productoCli.Cliente != null && productoCli.Cliente.Persona != null) ? productoCli.Cliente.Persona.GetNombreCompleto() : "",
                                     ClienteMail = (productoCli.Cliente != null && productoCli.Cliente.Usuario != null) ? productoCli.Cliente.Usuario.Mail : "",
                                     ClienteCel = (productoCli.Cliente != null) ? productoCli.Cliente.Celular : "",
                                     DescripcionAmpliada = productoCli.DescripcionAmpliada,
                                     Id = productoCli.Id,
                                     Precio = productoCli.Precio,
                                     Activo = productoCli.Activo
                                 });
                    uat.Productos = query.ToList();
                }
            }
            else
            {
                if(uat.Cantidad!=null && uat.Cantidad != 0)
                {
                   // uat.Productos = _context.Productos
                   //.Where(x => x.Precio > 0)
                   //.Take((int)uat.Cantidad)
                   //.Select(x => new MProductosDTO { Descripcion = x.Descripcion, Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "", ProveedorId = (x.Proveedor != null) ? x.Proveedor.Id : 0, ProveedorNombre = (x.Proveedor != null) ? x.Proveedor.Nombre : "", DescripcionAmpliada = x.DescripcionAmpliada, Foto = x.Foto, Id = x.Id, Precio = x.Precio, Activo = x.Activo })
                   //.ToList();
                   // uat.Productos.AddRange(_context.ProductosClientes.Where(x => x.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id)
                   //     .Take((int)uat.Cantidad)
                   //     .Select(x => new MProductosDTO
                   //     {
                   //         Descripcion = x.Descripcion,
                   //         Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "",
                   //         ClienteId = (x.Cliente != null) ? x.Cliente.Id : 0,
                   //         ClienteNombre = (x.Cliente != null && x.Cliente.Persona != null) ? x.Cliente.Persona.GetNombreCompleto() : "",
                   //         ClienteMail = (x.Cliente != null && x.Cliente.Usuario != null) ? x.Cliente.Usuario.Mail : "",
                   //         ClienteCel = (x.Cliente != null) ? x.Cliente.Celular : "",
                   //         DescripcionAmpliada = x.DescripcionAmpliada,
                   //         Foto = x.Foto,
                   //         Id = x.Id,
                   //         Precio = x.Precio,
                   //         Activo = x.Activo
                   //     })
                   //     .ToList());

                    var query = (from producto in _context.Productos
                                 join proveedor in _context.Proveedores
                                     on producto.Proveedor.Id equals proveedor.Id
                                 join rubro in _context.Rubros
                                    on producto.Rubro.Id equals rubro.Id
                                 where producto.Precio > 0
                                 select new MProductosDTO { Descripcion = producto.Descripcion, Rubro = (producto.Rubro != null) ? producto.Rubro.Nombre : "", ProveedorId = (producto.Proveedor != null) ? producto.Proveedor.Id : 0, ProveedorNombre = (producto.Proveedor != null) ? producto.Proveedor.Nombre : "", DescripcionAmpliada = producto.DescripcionAmpliada, Id = producto.Id, Precio = producto.Precio, Activo = producto.Activo });
                    uat.Productos = query.Take((int)uat.Cantidad).ToList();


                    var query2 = (from productoCli in _context.ProductosClientes
                                 join cliente in _context.Clientes
                                     on productoCli.Cliente.Id equals cliente.Id
                                 join rubro in _context.Rubros
                                    on productoCli.Rubro.Id equals rubro.Id
                                 join localidad in _context.Localidad
                                    on productoCli.Cliente.Localidad.Id equals localidad.Id
                                 //where productoCli.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id
                                 select new MProductosDTO
                                 {
                                     Descripcion = productoCli.Descripcion,
                                     Rubro = (productoCli.Rubro != null) ? productoCli.Rubro.Nombre : "",
                                     ClienteId = (productoCli.Cliente != null) ? productoCli.Cliente.Id : 0,
                                     ClienteNombre = (productoCli.Cliente != null && productoCli.Cliente.Persona != null) ? productoCli.Cliente.Persona.GetNombreCompleto() : "",
                                     ClienteMail = (productoCli.Cliente != null && productoCli.Cliente.Usuario != null) ? productoCli.Cliente.Usuario.Mail : "",
                                     ClienteCel = (productoCli.Cliente != null) ? productoCli.Cliente.Celular : "",
                                     DescripcionAmpliada = productoCli.DescripcionAmpliada,
                                     Id = productoCli.Id,
                                     Precio = productoCli.Precio,
                                     Activo = productoCli.Activo
                                 });

                    uat.Productos.AddRange(query2.Take((int)uat.Cantidad).ToList());

                }
                else
                {
                    // uat.Productos = _context.Productos
                    //.Where(x => x.Precio > 0)
                    //.Select(x => new MProductosDTO { Descripcion = x.Descripcion, Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "", ProveedorId = (x.Proveedor != null) ? x.Proveedor.Id : 0, ProveedorNombre = (x.Proveedor != null) ? x.Proveedor.Nombre : "", DescripcionAmpliada = x.DescripcionAmpliada, Foto = x.Foto, Id = x.Id, Precio = x.Precio, Activo = x.Activo })
                    //.ToList();
                    // uat.Productos.AddRange(_context.ProductosClientes.Where(x => x.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id)
                    //     .Select(x => new MProductosDTO
                    //     {
                    //         Descripcion = x.Descripcion,
                    //         Rubro = (x.Rubro != null) ? x.Rubro.Nombre : "",
                    //         ClienteId = (x.Cliente != null) ? x.Cliente.Id : 0,
                    //         ClienteNombre = (x.Cliente != null && x.Cliente.Persona != null) ? x.Cliente.Persona.GetNombreCompleto() : "",
                    //         ClienteMail = (x.Cliente != null && x.Cliente.Usuario != null) ? x.Cliente.Usuario.Mail : "",
                    //         ClienteCel = (x.Cliente != null) ? x.Cliente.Celular : "",
                    //         DescripcionAmpliada = x.DescripcionAmpliada,
                    //         Foto = x.Foto,
                    //         Id = x.Id,
                    //         Precio = x.Precio,
                    //         Activo = x.Activo
                    //     })
                    //     .ToList());

                    /*****************************************************/

                    var query = (from producto in _context.Productos
                                 join proveedor in _context.Proveedores
                                     on producto.Proveedor.Id equals proveedor.Id
                                 join rubro in _context.Rubros
                                    on producto.Rubro.Id equals rubro.Id
                                 join tipoproductos  in _context.TipoProducto
                                    on producto.TipoProducto.Id equals tipoproductos.Id
                                 where producto.Precio > 0
                                 select new MProductosDTO { Descripcion = producto.Descripcion, Rubro = (producto.Rubro != null) ? producto.Rubro.Nombre : "", ProveedorId = (producto.Proveedor != null) ? producto.Proveedor.Id : 0, ProveedorNombre = (producto.Proveedor != null) ? producto.Proveedor.Nombre : "", DescripcionAmpliada = producto.DescripcionAmpliada, Id = producto.Id, Precio = producto.Precio, Activo = producto.Activo , TipoProducto = (producto.TipoProducto != null) ? producto.TipoProducto.Descripcion : "" });
                    uat.Productos = query.ToList();


                    var query2 = (from productoCli in _context.ProductosClientes
                                  join cliente in _context.Clientes
                                      on productoCli.Cliente.Id equals cliente.Id
                                  join rubro in _context.Rubros
                                     on productoCli.Rubro.Id equals rubro.Id
                                  join localidad in _context.Localidad
                                     on productoCli.Cliente.Localidad.Id equals localidad.Id
                                  //where productoCli.Cliente.Localidad.Guarnicion.Id == Uat.Cliente.Localidad.Guarnicion.Id
                                  select new MProductosDTO
                                  {
                                      Descripcion = productoCli.Descripcion,
                                      Rubro = (productoCli.Rubro != null) ? productoCli.Rubro.Nombre : "",
                                      ClienteId = (productoCli.Cliente != null) ? productoCli.Cliente.Id : 0,
                                      ClienteNombre = (productoCli.Cliente != null && productoCli.Cliente.Persona != null) ? productoCli.Cliente.Persona.GetNombreCompleto() : "",
                                      ClienteMail = (productoCli.Cliente != null && productoCli.Cliente.Usuario != null) ? productoCli.Cliente.Usuario.Mail : "",
                                      ClienteCel = (productoCli.Cliente != null) ? productoCli.Cliente.Celular : "",
                                      DescripcionAmpliada = productoCli.DescripcionAmpliada,
                                      Id = productoCli.Id,
                                      Precio = productoCli.Precio,
                                      Activo = productoCli.Activo
                                  });

                    uat.Productos.AddRange(query2.ToList());                    
                }                
            }
            if (uat.Productos!=null)
            {

                foreach (var itemProductos in uat.Productos)
                {
                    itemProductos.Fotos = _context.ProductoAdjuntos.Where(x => x.Producto.Id==itemProductos.Id).Select(x => x.Foto).ToList();
                    itemProductos.Talles = _context.TallesProductos.Where(x => x.Producto.Id == itemProductos.Id).Select(x => x.Talles).ToList();
                }
            }
            return uat;
        }

        [HttpPost]
        [Route("TraeProductosPorRubro")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeProductosPorRubroDTO TraeProductosPorRubro([FromBody] MTraeProductosPorRubroDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            uat.Status = 200;
            uat.Mensaje = "Listado Productos";

            uat.Productos = _context.Productos.Where(x=>x.Rubro.Id==uat.RubroId)
            .Where(x => x.Precio > 0)
            .Select(x => new MProductosDTO { Descripcion = x.Descripcion, DescripcionAmpliada = x.DescripcionAmpliada, Id = x.Id, Precio = x.Precio, Activo = x.Activo })
            .ToList();

            foreach (var itemProductos in uat.Productos)
            {
                itemProductos.Fotos = _context.ProductoAdjuntos.Where(x => x.Producto.Id==itemProductos.Id).Select(x => x.Foto).ToList();
            }

            return uat;
        }

        [HttpPost]
        [Route("TraeProductosComprados")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeComprasProductos TraeProductosComprados([FromBody] MTraeComprasProductos uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (Uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            uat.Status = 200;
            uat.Mensaje = "Listado Productos";

            uat.Compras = _context.ComprasProductos.Where(x => x.Cliente.Id == Uat.Cliente.Id)           
            .Select(x => new MComprasDTO
            {
                Estado = x.Estado,
                FechaCompra = x.FechaCompra,
                FechaAnulacion = x.FechaAnulacion,
                Id = x.Id,
                TipoCompra = x.TipoCompra,
                Producto = new MProductosDTO()
                {
                    Id = x.Producto.Id,
                    Descripcion = x.Producto.Descripcion,
                    DescripcionAmpliada = x.Producto.DescripcionAmpliada,
                    Precio = x.Producto.Precio,
                }
            }).ToList();

            foreach (var itemProductos in uat.Compras)
            {
                itemProductos.Producto.Fotos = _context.ProductoAdjuntos.Where(x => x.Producto.Id==itemProductos.Producto.Id).Select(x => x.Foto).ToList();
            }
            return uat;
        }



        [HttpPost]
        [Route("CompraPorDebito")]
        [EnableCors("CorsPolicy")]
        public IActionResult ComprarProductoDebito([FromBody] MCompraProductoDTO CompraProducto)
        {
            try
            {
                Billetera billeteraPagadora = TraeBilletera(TraeUsuarioUAT(CompraProducto.UAT));

                Producto producto = _context.Productos.FirstOrDefault(x => x.Id == CompraProducto.ProductoId);

                if (billeteraPagadora.ChequeaDebito(producto.Precio))
                {

                    Compras compra = new Compras();

                    compra.Cliente = billeteraPagadora.Cliente;
                    compra.Producto = producto;
                    compra.FechaCompra = DateTime.Now;
                    compra.Estado = EstadoCompra.Efectuado;
                    compra.TipoCompra = TipoCompra.Debito;

                    MovimientoBilletera movimientoBilletera = new MovimientoBilletera();
                    //Agregar Movimiento Billetera

                    billeteraPagadora.Saldo -= producto.Precio;
                    _context.Update(billeteraPagadora);
                    _context.ComprasProductos.Add(compra);
                    #region Precompra Productos
                    ProductoPrecompra productoPrecomprado = _context.ProductosPrecompras.Where(x => x.Id == producto.Id).FirstOrDefault();
                    if (productoPrecomprado != null)
                    {
                        productoPrecomprado.Estado = EstadoPrecompra.CompraEfectuada;
                        productoPrecomprado.FechaConfirmacion = DateTime.Now;
                        _context.Update(productoPrecomprado);
                    }
                    #endregion
                    _context.SaveChanges();
                    _notificacionAPIService.Envia_Push(billeteraPagadora.Cliente.Usuario.DeviceId, "Compra de Producto", $"Ha abonado ${producto.Precio} desde su billetera para la compra de un Producto");
                    return new JsonResult(new RespuestaAPI { Status = 200, UAT = CompraProducto.UAT, Mensaje = "Compra ejecutada satifactoriamente" });
                }
                else
                {
                    return new JsonResult(new RespuestaAPI { Status = 400, UAT = CompraProducto.UAT, Mensaje = "El monto supera su saldo y el Producto no es Financiable" });
                }
            }
            catch (Exception e)
            {
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = CompraProducto.UAT, Mensaje = "Error al Comprar el Producto" });
            }
        }

        [HttpPost]
        [Route("Financiar")]
        [EnableCors("CorsPolicy")]
        public MSolicitaPrestamoProductoDTO FinanciarProducto([FromBody] MSolicitaPrestamoProductoDTO uat)
        {
            try
            {
                Billetera billeteraPagadora = TraeBilletera(TraeUsuarioUAT(uat.UAT));
                Producto producto = _context.Productos.FirstOrDefault(x => x.Id == uat.ProductoId);
                
                if (producto.Financiable)
                {
                    var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                    if (uat == null)
                    {
                        uat.Status = 500;
                        uat.Mensaje = "UAT Invalida";
                        return uat;
                    }
                    
                    if (uat.FirmaOlografica == null)
                    {
                        uat.Status = 500;
                        uat.Mensaje = "Debe Subir Firma Olografica";
                        return uat;
                    }
                    if (uat.FotoSosteniendoDNI == null)
                    {
                        uat.Status = 500;
                        uat.Mensaje = "Debe Subir Foto Sosteniendo DNI";
                        return uat;
                    }
                    if (uat.FotoDNIAnverso == null)
                    {
                        uat.Status = 500;
                        uat.Mensaje = "Debe Subir DNI Anverso";
                        return uat;
                    }
                    if (uat.FotoDNIReverso == null)
                    {
                        uat.Status = 500;
                        uat.Mensaje = "Debe Subir DNI Reverso";
                        return uat;
                    }
                    uat.Status = 200;
                    uat.Mensaje = "Solicitud Realizada Correctamente!!!";
                    Compras compra = new Compras();

                    compra.Cliente = billeteraPagadora.Cliente;
                    compra.Producto = producto;
                    compra.FechaCompra = DateTime.Now;
                    compra.Estado = EstadoCompra.Pendiente;
                    compra.TipoCompra = TipoCompra.Financiado;

                    MovimientoBilletera movimientoBilletera = new MovimientoBilletera();
                    //Agregar Movimiento Billetera
                    _context.Update(billeteraPagadora);
                    _context.ComprasProductos.Add(compra);
                    _context.SaveChanges();
                    return uat;                   
                }
                else
                {
                    uat.Mensaje = "El Producto no es Financiable";
                    uat.Status = 500;
                    return uat;
                }

                
            }
            catch (Exception e)
            {
                uat.Mensaje = "Error de API";
                uat.Status = 500;
                return uat;
            }
        }

        public string LoginCGE(Empresas empresa)
        {
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        return login.UAT;
                    }
                }
                return response.ToString();
            }
        }
        [HttpPost]
        [Route("AgregarProducto")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MProducto AddProducto([FromBody] MProducto uat)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                if (Uat == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "UAT Invalida";
                    return uat;
                }
                if (uat.RubroId == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar el Rubro del Producto.";
                    return uat;
                }
                if (uat.ClienteId != null)
                {
                    ProductoCliente producto = new ProductoCliente
                    {
                        Descripcion=uat.Descripcion.Trim(),
                        Foto=uat.Foto,
                        Cliente= _context.Clientes.Find(uat.ClienteId),
                        Rubro= _context.Rubros.Find(uat.RubroId),
                        Activo=true,
                        DescripcionAmpliada=uat.DescripcionAmpliada.Trim(),
                        Precio=uat.Precio,
                        Oferta=uat.Oferta
                    };
                    _context.ProductosClientes.Add(producto);
                    _context.SaveChanges();
                    uat.ProductoId = producto.Id;
                    uat.Status = 200;
                    uat.Mensaje = "Producto Creado Correctamente";                    
                    return uat;
                }
                else if (uat.ProveedorId != null)
                {
                    Producto producto = new Producto
                    {
                        Descripcion = uat.Descripcion.Trim(),
                        Foto = uat.Foto,
                        Proveedor = _context.Proveedores.Find(uat.ProveedorId),
                        Rubro = _context.Rubros.Find(uat.RubroId),
                        Activo = true,
                        DescripcionAmpliada = uat.DescripcionAmpliada.Trim(),
                        Precio = uat.Precio,
                        Oferta = uat.Oferta,
                    };
                    _context.Productos.Add(producto);
                    _context.SaveChanges();
                    uat.ProductoId = producto.Id;
                    uat.Status = 200;
                    uat.Mensaje = "Producto Creado Correctamente";
                    return uat;
                }
                else
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar si es un Producto de un Proveedor o de un cliente.";
                    return uat;
                }
                
            }
            catch(Exception)
            {
                uat.Status = 500;
                uat.Mensaje = "Error en la Petición";
                return uat;
            }            
        }

        [HttpPost]
        [Route("EditarProducto")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MProducto EditarProducto([FromBody] MProducto uat)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                if (Uat == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "UAT Invalida";
                    return uat;
                }
                if (uat.ProductoId == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar el Id del Producto.";
                    return uat;
                }
                if (uat.RubroId == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar el Rubro del Producto.";
                    return uat;
                }
                if (uat.ClienteId != null)
                {
                    ProductoCliente producto = _context.ProductosClientes.Find(uat.ProductoId);
                    producto.Descripcion = uat.Descripcion.Trim();
                    producto.Foto = uat.Foto;
                    producto.Cliente = _context.Clientes.Find(uat.ClienteId);
                    producto.Rubro = _context.Rubros.Find(uat.RubroId);
                    producto.Activo = true;
                    producto.DescripcionAmpliada = uat.DescripcionAmpliada.Trim();
                    producto.Precio = uat.Precio;
                    producto.Oferta = uat.Oferta;
                    _context.ProductosClientes.Update(producto);
                    _context.SaveChanges();
                    uat.Status = 200;
                    uat.Mensaje = "Producto Modificado Correctamente";
                    return uat;
                }
                else if (uat.ProveedorId != null)
                {
                    Producto producto = _context.Productos.Find(uat.ProductoId);
                    producto.Descripcion = uat.Descripcion.Trim();
                    producto.Foto = uat.Foto;
                    producto.Proveedor = _context.Proveedores.Find(uat.ProveedorId);
                    producto.Rubro = _context.Rubros.Find(uat.RubroId);
                    producto.Activo = true;
                    producto.DescripcionAmpliada = uat.DescripcionAmpliada.Trim();
                    producto.Precio = uat.Precio;
                    producto.Oferta = uat.Oferta;
                    _context.Productos.Update(producto);
                    _context.SaveChanges();
                    uat.Status = 200;
                    uat.Mensaje = "Producto Modificado Correctamente";
                    return uat;
                }
                else
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar si es un Producto de un Proveedor o de un cliente.";
                    return uat;
                }

            }
            catch (Exception)
            {
                uat.Status = 500;
                uat.Mensaje = "Error en la Petición";
                return uat;
            }
        }
        [HttpPost]
        [Route("BorrarProducto")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MProducto BorrarProducto([FromBody] MProducto uat)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                if (Uat == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "UAT Invalida";
                    return uat;
                }
                if (uat.ProductoId == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar el Id del Producto.";
                    return uat;
                }
                if (uat.ClienteId != null)
                {
                    ProductoCliente producto = _context.ProductosClientes.Find(uat.ProductoId);
                    producto.Activo = false;
                    _context.ProductosClientes.Update(producto);
                    _context.SaveChanges();
                    uat.Status = 200;
                    uat.Mensaje = "Producto Eliminado Correctamente";
                    return uat;
                }
                else if (uat.ProveedorId != null)
                {
                    Producto producto = _context.Productos.Find(uat.ProductoId);
                    producto.Activo = false;
                    _context.Productos.Update(producto);
                    _context.SaveChanges();
                    uat.Status = 200;
                    uat.Mensaje = "Producto Eliminado Correctamente";
                    return uat;
                }
                else
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar si es un Producto de un Proveedor o de un cliente.";
                    return uat;
                }

            }
            catch (Exception)
            {
                uat.Status = 500;
                uat.Mensaje = "Error en la Petición";
                return uat;
            }
        }

        [HttpPost]
        [Route("RegistrarContactoCliente")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MProductoClienteCompra RegistraProductoContacto([FromBody] MProductoClienteCompra uat)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                if (Uat == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "UAT Invalida";
                    return uat;
                }
                if (uat.Productos.Count()<=0)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar los Id's de los Productos.";
                    return uat;
                }
                List<ProductoClienteContacto> contactos = new List<ProductoClienteContacto>();
                foreach (int p in uat.Productos)
                {
                    contactos.Add(new ProductoClienteContacto
                    {
                        ClienteComprador=Uat.Cliente,
                        Producto=_context.ProductosClientes.Find(p),
                        FechaContacto=DateTime.Now
                    });
                }
                _context.ProductosClientesContactos.AddRange(contactos);
                _context.SaveChanges();
                uat.Status = 200;
                uat.Mensaje = "Se registro el Contacto de los Productos Correctamente";
                return uat;
            }
            catch (Exception)
            {
                uat.Status = 500;
                uat.Mensaje = "Error en la Petición";
                return uat;
            }
        }

        [HttpPost]
        [Route("ObtenerContactosCliente")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MListaProductosClienteCompra ObtenerProductoContacto([FromBody] MListaProductosClienteCompra lista)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == lista.UAT);
                if (Uat == null)
                {
                    lista.Status = 500;
                    lista.Mensaje = "UAT Invalida";
                    return lista;
                }
                lista.Productos = _context.ProductosClientesContactos.Where(x=>x.ClienteComprador.Id==Uat.Cliente.Id).Select(x => new MProductosClientesComprados
                {
                    FechaInteres=x.FechaContacto.ToShortDateString(),
                    Descripcion = x.Producto.Descripcion,
                    Rubro = (x.Producto.Rubro != null) ? x.Producto.Rubro.Nombre : "",
                    ClienteId = (x.Producto.Cliente != null) ? x.Producto.Cliente.Id : 0,
                    ClienteNombre = (x.Producto.Cliente != null && x.Producto.Cliente.Persona != null) ? x.Producto.Cliente.Persona.GetNombreCompleto() : "",
                    ClienteMail = (x.Producto.Cliente != null && x.Producto.Cliente.Usuario != null) ? x.Producto.Cliente.Usuario.Mail : "",
                    ClienteCel = (x.Producto.Cliente != null) ? x.Producto.Cliente.Celular : "",
                    DescripcionAmpliada = x.Producto.DescripcionAmpliada,
                    Foto = x.Producto.Foto,
                    Id = x.Id,
                    Precio = x.Producto.Precio
                }).ToList();
                lista.Status = 200;
                lista.Mensaje = "Lista de Productos de Clientes Contactados";
                return lista;
            }
            catch (Exception)
            {
                lista.Status = 500;
                lista.Mensaje = "Error en la Petición";
                return lista;
            }
        }
        [HttpPost]
        [Route("ObtenerClientesInteresados")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MListaProductosClienteCompra ObtenerClientesInteresados([FromBody] MListaProductosClienteCompra lista)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == lista.UAT);
                if (Uat == null)
                {
                    lista.Status = 500;
                    lista.Mensaje = "UAT Invalida";
                    return lista;
                }
                lista.Productos = _context.ProductosClientesContactos.Where(x => x.Producto.Cliente.Id == Uat.Cliente.Id).Select(x => new MProductosClientesComprados
                {
                    FechaInteres = x.FechaContacto.ToShortDateString(),
                    Descripcion = x.Producto.Descripcion,
                    Rubro = (x.Producto.Rubro != null) ? x.Producto.Rubro.Nombre : "",
                    ClienteId = (x.ClienteComprador != null) ? x.ClienteComprador.Id : 0,
                    ClienteNombre = (x.ClienteComprador != null && x.ClienteComprador.Persona != null) ? x.ClienteComprador.Persona.GetNombreCompleto() : "",
                    ClienteMail = (x.ClienteComprador != null && x.ClienteComprador.Usuario != null) ? x.ClienteComprador.Usuario.Mail : "",
                    ClienteCel = (x.ClienteComprador != null) ? x.ClienteComprador.Celular : "",
                    DescripcionAmpliada = x.Producto.DescripcionAmpliada,
                    Foto = x.Producto.Foto,
                    Id = x.Id,
                    Precio = x.Producto.Precio
                }).ToList();
                lista.Status = 200;
                lista.Mensaje = "Lista de Clientes Interesados";
                return lista;
            }
            catch (Exception)
            {
                lista.Status = 500;
                lista.Mensaje = "Error en la Petición";
                return lista;
            }
        }
        [HttpPost]
        [Route("RegistraPrecompraProductos")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MProductosPrecompras RegistraPrecompraProductos([FromBody] MProductosPrecompras uat)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                if (Uat == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "UAT Invalida";
                    return uat;
                }
                if (uat.Productos.Count() <= 0)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar los Id's de los Productos.";
                    return uat;
                }
                List<ProductoPrecompra> productos = new List<ProductoPrecompra>();
                foreach (int p in uat.Productos)
                {
                    productos.Add(new ProductoPrecompra
                    {
                        Cliente = Uat.Cliente,
                        Producto = _context.Productos.Find(p),
                        FechaPrecompra = DateTime.Now,
                        Estado=EstadoPrecompra.Precompra
                    });
                }
                _context.ProductosPrecompras.AddRange(productos);
                _context.SaveChanges();
                uat.Status = 200;
                uat.Mensaje = "Se registro la Precompra de los Productos Correctamente";
                return uat;
            }
            catch (Exception)
            {
                uat.Status = 500;
                uat.Mensaje = "Error en la Petición";
                return uat;
            }
        }
        [HttpPost]
        [Route("RegistraAnulacionPrecompraProductos")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MProductosPrecompras RegistraAnulacionPrecompraProductos([FromBody] MProductosPrecompras uat)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
                if (Uat == null)
                {
                    uat.Status = 500;
                    uat.Mensaje = "UAT Invalida";
                    return uat;
                }
                if (uat.Productos.Count() <= 0)
                {
                    uat.Status = 500;
                    uat.Mensaje = "Debe informar los Id's de los Productos.";
                    return uat;
                }
                List<ProductoPrecompra> productos = _context.ProductosPrecompras.Where(x => x.Estado == EstadoPrecompra.Precompra && uat.Productos.Contains(x.Producto.Id)).ToList();
                foreach (ProductoPrecompra p in productos)
                {
                    p.Estado = EstadoPrecompra.Anulado;
                    p.FechaAnulacion = DateTime.Now;
                }
                _context.SaveChanges();
                uat.Status = 200;
                uat.Mensaje = "Se registro la anulación de la Precompra de los Productos Correctamente";
                return uat;
            }
            catch (Exception)
            {
                uat.Status = 500;
                uat.Mensaje = "Error en la Petición";
                return uat;
            }
        }
        [HttpPost]
        [Route("TraeProductosPrecompra")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeProductosPrecompraDTO TraeProductosPrecomprados([FromBody] MTraeProductosPrecompraDTO model)
        {
            try
            {
                var Uat = _context.UAT.FirstOrDefault(x => x.Token == model.UAT);
                if (Uat == null)
                {
                    model.Status = 500;
                    model.Mensaje = "UAT Invalida";
                    return model;
                }
                model.Status = 200;
                model.Mensaje = "Listado Productos";

                model.Productos = _context.ProductosPrecompras.Where(x => x.Cliente.Id == Uat.Cliente.Id)
                .Select(x => new MProductoPrecompraDTO { Id=x.Producto.Id,ProveedorId=x.Producto.Proveedor.Id,ProveedorNombre=x.Producto.Proveedor.Nombre,Descripcion = x.Producto.Descripcion, DescripcionAmpliada = x.Producto.DescripcionAmpliada, Foto = x.Producto.Foto, Precio = x.Producto.Precio,FechaPrecompra=x.FechaPrecompra, Estado = x.Estado })
                .ToList();

                return model;
            }
            catch (Exception)
            {
                model.Status = 500;
                model.Mensaje = "Error en la Petición";
                return model;
            }
        }
    }
}