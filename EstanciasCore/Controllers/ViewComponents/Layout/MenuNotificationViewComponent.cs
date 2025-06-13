using System.Collections.Generic;
using Commons.Models;
using Microsoft.AspNetCore.Mvc;

namespace Commons.Controllers.ViewComponents.Layout
{
    public class MenuNotificationViewComponent : ViewComponent
    {

        public MenuNotificationViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            var messages = GetData();
            return View("LayoutMenuNotification", messages);
        }

        private List<Message> GetData()
        {
            var messages = new List<Message>();
            messages.Add(new Message
            {
                Id = 1,
                FontAwesomeIcon = "fa fa-users text-aqua",
                ShortDesc = "5 new members joined today",
                URLPath = "#",
            });

            return messages;
        }
    }
}
