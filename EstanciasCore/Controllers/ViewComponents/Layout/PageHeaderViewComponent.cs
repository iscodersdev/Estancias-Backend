using Microsoft.AspNetCore.Mvc;

namespace Commons.Controllers.ViewComponents.Layout
{
    public class PageHeaderViewComponent : ViewComponent
    {

        public PageHeaderViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            return View("LayoutPageHeader");
        }
    }
}
