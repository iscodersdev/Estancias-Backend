using System.Collections.Generic;
using Commons.Models;
using Microsoft.AspNetCore.Mvc;

namespace Commons.Controllers.ViewComponents.Layout
{
    public class MenuTaskViewComponent : ViewComponent
    {

        public MenuTaskViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            var messages = GetData();
            return View("LayoutMenuTask", messages);
        }

        private List<Message> GetData()
        {
            var messages = new List<Message>();
            messages.Add(new Message
            {
                Id = 1,
                ShortDesc = "Design some buttons",
                URLPath = "#",
                Percentage = 20,
            });

            return messages;
        }
    }
}
