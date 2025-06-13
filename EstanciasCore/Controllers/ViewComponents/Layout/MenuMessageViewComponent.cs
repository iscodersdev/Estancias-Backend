using Commons.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DAL.Data;

namespace EstanciasCore.Controllers
{
    public class MenuMessageViewComponent : ViewComponent
    {

        public EstanciasContext Context;
        public MenuMessageViewComponent(EstanciasContext context) 
        {
            Context = context;
        }

        public IViewComponentResult Invoke(string filter)
        {
            var messages = GetData();
            return View("LayoutMenuMessage", messages);
        }

        private List<Message> GetData()
        {
            var messages = new List<Message>();

            messages.Add(new Message
            {
                Id = 1,
                UserId = "",
                DisplayName = "Support Team",
                AvatarURL = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTRxQ-XnyKmKP11SQacp2IutD8Njt89ze2EIIN9AAtrEP5g83jI",
                ShortDesc = "Why not buy a new awesome theme?",
                TimeSpan = "5 mins",
                URLPath = "#",
            });

            messages.Add(new Message
            {
                Id = 1,
                UserId = "",
                DisplayName = "Ken",
                AvatarURL = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTRxQ-XnyKmKP11SQacp2IutD8Njt89ze2EIIN9AAtrEP5g83jI",
                ShortDesc = "For approval",
                TimeSpan = "15 mins",
                URLPath = "#",
            });

            return messages;
        }
    }
}
