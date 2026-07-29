using Commons.Identity;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using EstanciasCore.API.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstanciasCore.Endpoints
{
    [TypeFilter(typeof(EndpointUatAuthorizeAttribute))]
    [Route("endpoint/security-roles")]
    [ApiController]
    public class SecurityRolesEndpointController : ControllerBase
    {
        private readonly EstanciasContext _context;
        private readonly UserManager<Usuario> _userManager;

        public SecurityRolesEndpointController(
            EstanciasContext context,
            UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Equivalente al Index() del controller MVC.
        // GET: endpoint/security-roles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _context.Set<CommonsRole>().ToListAsync();

            var roleFunctions = await _context.Set<CommonsRoleFunction>()
                .Include(rf => rf.Role)
                .Include(rf => rf.Function)
                .Where(rf => rf.DeletedDate == null)
                .ToListAsync();

            var data = roles
                .Select(role => MapRole(
                    role,
                    roleFunctions
                        .Where(rf =>
                            rf.Role != null &&
                            rf.Role.Id == role.Id &&
                            rf.Function != null)
                        .Select(rf => rf.Function.Name)
                        .ToList()
                ))
                .ToList();

            return Ok(new
            {
                ok = true,
                data = data
            });
        }

        // Equivalente al GET _Update(string id) del controller MVC.
        // GET: endpoint/security-roles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var role = await _context.Set<CommonsRole>().FindAsync(id);

            if (role == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el Rol."
                });
            }

            return Ok(new
            {
                ok = true,
                data = MapRole(role, new List<string>())
            });
        }

        // Equivalente al POST _Create() del controller MVC.
        // POST: endpoint/security-roles
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SecurityRoleCreateDTO request)
        {
            try
            {
                var role = CreateCommonsRoleInstance();
                role.Name = request != null ? request.Name : null;
                role.ShowName = request != null ? request.ShowName : null;
                role.Description = request != null ? request.Description : null;

                TryValidateModel(role);
                ModelState.Remove("Id");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "Los datos del Rol no son válidos.",
                        errors = ModelState
                    });
                }

                role.Id = Guid.NewGuid().ToString();
                role.NormalizedName = role.Name != null
                    ? role.Name.ToUpper()
                    : null;
                role.CreatedDate = DateTime.Now;
                role.Enabled = true;
                role.Show = true;

                await _context.Set<CommonsRole>().AddAsync(role);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se cargó correctamente el Rol " + role.ShowName + ".",
                    data = MapRole(role, new List<string>())
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al cargar el Rol. " + ex.Message
                });
            }
        }

        // Equivalente al POST _Update() del controller MVC.
        // PUT: endpoint/security-roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            string id,
            [FromBody] SecurityRoleUpdateDTO request)
        {
            try
            {
                var roleToValidate = CreateCommonsRoleInstance();
                roleToValidate.Id = id;
                roleToValidate.Name = request != null ? request.Name : null;
                roleToValidate.ShowName = request != null ? request.ShowName : null;
                roleToValidate.Description = request != null ? request.Description : null;
                roleToValidate.Enabled = request != null && request.Enabled;
                roleToValidate.Show = request != null && request.Show;

                TryValidateModel(roleToValidate);

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "Los datos del Rol no son válidos.",
                        errors = ModelState
                    });
                }

                var existingRole = await _context.Set<CommonsRole>().FindAsync(id);

                if (existingRole == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró el Rol."
                    });
                }

                existingRole.Name = request.Name;
                existingRole.NormalizedName = request.Name != null
                    ? request.Name.ToUpper()
                    : null;
                existingRole.ShowName = request.ShowName;
                existingRole.Description = request.Description;
                existingRole.Enabled = request.Enabled;
                existingRole.Show = request.Show;

                _context.Set<CommonsRole>().Update(existingRole);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se modificó correctamente el Rol.",
                    data = MapRole(existingRole, new List<string>())
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al modificar el Rol. " + ex.Message
                });
            }
        }

        // Equivalente al Delete(string id) del controller MVC.
        // Mantiene el borrado físico del rol y elimina previamente sus relaciones.
        // DELETE: endpoint/security-roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var role = await _context.Set<CommonsRole>().FindAsync(id);

                if (role != null)
                {
                    var roleFuncs = await _context.Set<CommonsRoleFunction>()
                        .Include(rf => rf.Role)
                        .Where(rf => rf.Role.Id == id)
                        .ToListAsync();

                    _context.Set<CommonsRoleFunction>().RemoveRange(roleFuncs);

                    var userRoles = await _context.UserRoles
                        .Where(ur => ur.RoleId == id)
                        .ToListAsync();

                    _context.UserRoles.RemoveRange(userRoles);
                    _context.Set<CommonsRole>().Remove(role);

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        ok = true,
                        message = "Se eliminó correctamente el Rol " + role.ShowName + "."
                    });
                }

                return Ok(new
                {
                    ok = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al eliminar el Rol. " + ex.Message
                });
            }
        }

        // Equivalente al GET _Assign(string id) del controller MVC.
        // GET: endpoint/security-roles/users/{userId}/roles
        [HttpGet("users/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "El ID de usuario es requerido."
                });
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "Usuario no encontrado."
                });
            }

            var allRoles = await _context.Set<CommonsRole>()
                .Where(role => role.Enabled)
                .ToListAsync();

            var assignedRoleIds = await _context.UserRoles
                .Where(userRole => userRole.UserId == userId)
                .Select(userRole => userRole.RoleId)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                user = new SecurityRoleUserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName
                },
                assignedRoleIds = assignedRoleIds,
                roles = allRoles
                    .Select(role => MapRole(role, new List<string>()))
                    .ToList()
            });
        }

        // Equivalente al POST _Assign(string userId, List<string> selectedRoleIds).
        // POST: endpoint/security-roles/users/{userId}/roles
        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> AssignUserRoles(
            string userId,
            [FromBody] SecurityRoleAssignmentDTO request)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "Usuario no encontrado."
                    });
                }

                var selectedRoleIds = request != null
                    ? request.SelectedRoleIds
                    : null;

                var currentRoles = await _context.UserRoles
                    .Where(userRole => userRole.UserId == userId)
                    .ToListAsync();

                var toRemove = currentRoles
                    .Where(userRole =>
                        selectedRoleIds == null ||
                        !selectedRoleIds.Contains(userRole.RoleId))
                    .ToList();

                _context.UserRoles.RemoveRange(toRemove);

                if (selectedRoleIds != null)
                {
                    foreach (var roleId in selectedRoleIds)
                    {
                        if (!currentRoles.Any(userRole => userRole.RoleId == roleId))
                        {
                            _context.UserRoles.Add(new IdentityUserRole<string>
                            {
                                UserId = userId,
                                RoleId = roleId
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se actualizaron correctamente los roles para el usuario " + user.UserName + "."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al asignar los roles. " + ex.Message
                });
            }
        }

        // Equivalente al GET _AssignFunctions(string id) del controller MVC.
        // GET: endpoint/security-roles/{roleId}/functions
        [HttpGet("{roleId}/functions")]
        public async Task<IActionResult> GetRoleFunctions(string roleId)
        {
            var role = await _context.Set<CommonsRole>().FindAsync(roleId);

            if (role == null)
            {
                return NotFound(new
                {
                    ok = false,
                    message = "No se encontró el Rol."
                });
            }

            var allFunctions = await _context.Set<CommonsFunction>()
                .Where(function => function.DeletedDate == null)
                .ToListAsync();

            var assignedFunctionIds = await _context.Set<CommonsRoleFunction>()
                .Include(roleFunction => roleFunction.Role)
                .Include(roleFunction => roleFunction.Function)
                .Where(roleFunction =>
                    roleFunction.Role.Id == roleId &&
                    roleFunction.DeletedDate == null)
                .Select(roleFunction => roleFunction.Function.Id)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                role = MapRole(role, new List<string>()),
                assignedFunctionIds = assignedFunctionIds,
                functions = allFunctions
                    .Select(MapFunction)
                    .ToList()
            });
        }

        // Equivalente al POST _AssignFunctions(
        // string roleId, List<string> selectedFunctionIds).
        // POST: endpoint/security-roles/{roleId}/functions
        [HttpPost("{roleId}/functions")]
        public async Task<IActionResult> AssignRoleFunctions(
            string roleId,
            [FromBody] SecurityFunctionAssignmentDTO request)
        {
            try
            {
                var role = await _context.Set<CommonsRole>().FindAsync(roleId);

                if (role == null)
                {
                    return NotFound(new
                    {
                        ok = false,
                        message = "No se encontró el Rol."
                    });
                }

                var selectedFunctionIds = request != null
                    ? request.SelectedFunctionIds
                    : null;

                var currentFunctions = await _context.Set<CommonsRoleFunction>()
                    .Include(roleFunction => roleFunction.Role)
                    .Include(roleFunction => roleFunction.Function)
                    .Where(roleFunction =>
                        roleFunction.Role.Id == roleId &&
                        roleFunction.DeletedDate == null)
                    .ToListAsync();

                var toRemove = currentFunctions
                    .Where(roleFunction =>
                        selectedFunctionIds == null ||
                        !selectedFunctionIds.Contains(roleFunction.Function.Id))
                    .ToList();

                _context.Set<CommonsRoleFunction>().RemoveRange(toRemove);

                if (selectedFunctionIds != null)
                {
                    foreach (var functionId in selectedFunctionIds)
                    {
                        if (!currentFunctions.Any(roleFunction =>
                            roleFunction.Function.Id == functionId))
                        {
                            var function = await _context.Set<CommonsFunction>()
                                .FindAsync(functionId);

                            _context.Set<CommonsRoleFunction>().Add(
                                new CommonsRoleFunction
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Role = role,
                                    Function = function,
                                    CreationDate = DateTime.Now
                                }
                            );
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    ok = true,
                    message = "Se asignaron correctamente las funciones al rol " + role.ShowName + "."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    message = "Hubo un error al asignar las funciones. " + ex.Message
                });
            }
        }

        // CommonsRole tiene el constructor sin parámetros no público.
        // Se crea mediante reflection para conservar la misma entidad y validaciones del MVC.
        private static CommonsRole CreateCommonsRoleInstance()
        {
            return (CommonsRole)Activator.CreateInstance(
                typeof(CommonsRole),
                true
            );
        }

        private static SecurityRoleDTO MapRole(
            CommonsRole role,
            List<string> functionNames)
        {
            return new SecurityRoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName,
                ShowName = role.ShowName,
                Description = role.Description,
                Enabled = role.Enabled,
                Show = role.Show,
                CreatedDate = role.CreatedDate,
                FunctionNames = functionNames ?? new List<string>()
            };
        }

        private static SecurityRoleFunctionDTO MapFunction(
            CommonsFunction function)
        {
            return new SecurityRoleFunctionDTO
            {
                Id = function.Id,
                Name = function.Name,
                Description = function.Description,
                Display = function.Display,
                Show = function.Show
            };
        }
    }
}