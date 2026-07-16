using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace EstanciasCore.Services
{
    public class DisableLibraryControllersConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            // Find and remove precompiled controllers from the Commons library
            // that we have overridden locally to prevent AmbiguousMatchException
            var controllersToRemove = application.Controllers
                .Where(c => c.ControllerType.Namespace != null && 
                            c.ControllerType.Namespace.StartsWith("Commons.") && 
                            (c.ControllerName.Contains("SecurityRoles") || c.ControllerName.Contains("SecurityFunctions")))
                .ToList();

            foreach (var controller in controllersToRemove)
            {
                application.Controllers.Remove(controller);
            }
        }
    }
}
