using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commons.Models;
using Commons.Identity;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using EstanciasCore.Controllers;

namespace EstanciasCore.Controllers
{
    public class SecurityRolesController : EstanciasCoreController
    {
        private readonly UserManager<Usuario> _userManager;

        public SecurityRolesController(EstanciasContext context, UserManager<Usuario> userManager) : base(context)
        {
            _userManager = userManager;
            breadcumb.Add(new Message() { DisplayName = "Seguridad" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Roles" });
            ViewBag.Breadcrumb = breadcumb;
            var roles = _context.Set<CommonsRole>().ToList();

            // Get all functions assigned to roles
            var roleFunctions = _context.Set<CommonsRoleFunction>()
                                        .Include(rf => rf.Role)
                                        .Include(rf => rf.Function)
                                        .Where(rf => rf.DeletedDate == null)
                                        .ToList();

            var roleFunctionsDict = roles.ToDictionary(
                r => r.Id,
                r => roleFunctions.Where(rf => rf.Role != null && rf.Role.Id == r.Id && rf.Function != null)
                                  .Select(rf => rf.Function.Name)
                                  .ToList()
            );

            ViewBag.RoleFunctions = roleFunctionsDict;
            return View(roles);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> _Create(CommonsRole role)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    role.Id = Guid.NewGuid().ToString();
                    role.NormalizedName = role.Name?.ToUpper();
                    role.CreatedDate = DateTime.Now;
                    role.Enabled = true;
                    role.Show = true;
                    await _context.Set<CommonsRole>().AddAsync(role);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargó correctamente el Rol " + role.ShowName + ".");
                    return RedirectToAction("Index");
                }
                return PartialView(role);
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Rol. " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        public IActionResult _Update(string id)
        {
            var role = _context.Set<CommonsRole>().Find(id);
            if (role == null) return NotFound();
            return PartialView(role);
        }

        [HttpPost]
        public async Task<IActionResult> _Update(CommonsRole role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingRole = await _context.Set<CommonsRole>().FindAsync(role.Id);
                    if (existingRole == null) return NotFound();
                    
                    existingRole.Name = role.Name;
                    existingRole.NormalizedName = role.Name?.ToUpper();
                    existingRole.ShowName = role.ShowName;
                    existingRole.Description = role.Description;
                    existingRole.Enabled = role.Enabled;
                    existingRole.Show = role.Show;
                    
                    _context.Set<CommonsRole>().Update(existingRole);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modificó correctamente el Rol.");
                    return RedirectToAction("Index");
                }
                return PartialView(role);
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Rol. " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var role = await _context.Set<CommonsRole>().FindAsync(id);
                if (role != null)
                {
                    // Remove role functions
                    var roleFuncs = _context.Set<CommonsRoleFunction>()
                                            .Include(rf => rf.Role)
                                            .Where(rf => rf.Role.Id == id)
                                            .ToList();

                    _context.Set<CommonsRoleFunction>().RemoveRange(roleFuncs);

                    // Remove user roles
                    var userRoles = _context.UserRoles.Where(ur => ur.RoleId == id).ToList();
                    _context.UserRoles.RemoveRange(userRoles);

                    _context.Set<CommonsRole>().Remove(role);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Rol " + role.ShowName + ".");
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Rol. " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpGet("/SecurityRoles/_Assign/{id?}")]
        public async Task<IActionResult> _Assign(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("El ID de usuario es requerido.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Usuario no encontrado.");

            var allRoles = _context.Set<CommonsRole>().Where(r => r.Enabled).ToList();
            var assignedRoleIds = _context.UserRoles.Where(ur => ur.UserId == id).Select(ur => ur.RoleId).ToList();

            ViewBag.User = user;
            ViewBag.AssignedRoleIds = assignedRoleIds;
            ViewBag.Roles = allRoles;

            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> _Assign(string userId, List<string> selectedRoleIds)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                var currentRoles = _context.UserRoles.Where(ur => ur.UserId == userId).ToList();

                // Remove roles not selected
                var toRemove = currentRoles.Where(ur => selectedRoleIds == null || !selectedRoleIds.Contains(ur.RoleId)).ToList();
                _context.UserRoles.RemoveRange(toRemove);

                // Add newly selected roles
                if (selectedRoleIds != null)
                {
                    foreach (var roleId in selectedRoleIds)
                    {
                        if (!currentRoles.Any(ur => ur.RoleId == roleId))
                        {
                            _context.UserRoles.Add(new IdentityUserRole<string> { UserId = userId, RoleId = roleId });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se actualizaron correctamente los roles para el usuario " + user.UserName + ".");
                return Redirect("/Administracion/Usuarios");
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al asignar los roles. " + ex.Message);
                return Redirect("/Administracion/Usuarios");
            }
        }

        public async Task<IActionResult> _AssignFunctions(string id)
        {
            var role = await _context.Set<CommonsRole>().FindAsync(id);
            if (role == null) return NotFound();

            var allFunctions = _context.Set<CommonsFunction>().Where(f => f.DeletedDate == null).ToList();
            var assignedFuncIds = _context.Set<CommonsRoleFunction>()
                                           .Include(rf => rf.Role)
                                           .Include(rf => rf.Function)
                                           .Where(rf => rf.Role.Id == id && rf.DeletedDate == null)
                                           .Select(rf => rf.Function.Id)
                                           .ToList();

            ViewBag.Role = role;
            ViewBag.AssignedFuncIds = assignedFuncIds;
            ViewBag.Functions = allFunctions;

            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> _AssignFunctions(string roleId, List<string> selectedFunctionIds)
        {
            try
            {
                var role = await _context.Set<CommonsRole>().FindAsync(roleId);
                if (role == null) return NotFound();

                var currentFuncs = _context.Set<CommonsRoleFunction>()
                                            .Include(rf => rf.Role)
                                            .Include(rf => rf.Function)
                                            .Where(rf => rf.Role.Id == roleId && rf.DeletedDate == null)
                                            .ToList();

                // Remove functions not selected
                var toRemove = currentFuncs.Where(rf => selectedFunctionIds == null || !selectedFunctionIds.Contains(rf.Function.Id)).ToList();
                _context.Set<CommonsRoleFunction>().RemoveRange(toRemove);

                // Add newly selected functions
                if (selectedFunctionIds != null)
                {
                    foreach (var funcId in selectedFunctionIds)
                    {
                        if (!currentFuncs.Any(rf => rf.Function.Id == funcId))
                        {
                            var function = await _context.Set<CommonsFunction>().FindAsync(funcId);
                            _context.Set<CommonsRoleFunction>().Add(new CommonsRoleFunction
                            {
                                Id = Guid.NewGuid().ToString(),
                                Role = role,
                                Function = function,
                                CreationDate = DateTime.Now
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se asignaron correctamente las funciones al rol " + role.ShowName + ".");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al asignar las funciones. " + ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}
