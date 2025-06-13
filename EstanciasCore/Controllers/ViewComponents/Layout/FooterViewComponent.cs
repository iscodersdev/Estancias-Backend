using Microsoft.AspNetCore.Mvc;

namespace Commons.Controllers.ViewComponents.Layout
{
    public class FooterViewComponent : ViewComponent
    {

        public FooterViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            return View("LayoutFooter");
        }
    }
}
