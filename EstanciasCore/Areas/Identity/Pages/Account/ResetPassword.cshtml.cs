using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL.Data;
using DAL.Models;

namespace EstanciasCore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        public EstanciasContext Context;

        public ResetPasswordModel(UserManager<Usuario> userManager, EstanciasContext context)
        {
            _userManager = userManager;
            Context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "eMail:")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Contraseña")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirma Contraseña")]
            [Compare("Password", ErrorMessage = "La Contraseña y la Confirmacion Deben Coincidir")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = code
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }
            var socio = Context.Clientes.FirstOrDefault(x => x.Usuario.UserName == user.UserName);
            if (socio.Password==Input.Code)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, Input.Password);
                socio.Password = Input.Password;
                Context.Clientes.Update(socio);
                Context.SaveChanges();
                return RedirectToPage("./ResetPasswordConfirmation");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Token Invalido");
            }
            return Page();

        }
    }
}
