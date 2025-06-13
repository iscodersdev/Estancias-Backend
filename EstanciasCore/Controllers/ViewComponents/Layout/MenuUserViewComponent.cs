using Microsoft.AspNetCore.Mvc;

namespace Commons.Controllers.ViewComponents.Layout
{
    public class MenuUserViewComponent : ViewComponent
    {

        public MenuUserViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            return View("LayoutMenuUser");
        }
    }
}
