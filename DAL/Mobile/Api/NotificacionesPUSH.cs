using Commons.Identity;
using Commons.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DAL.Models.API
{
    public class Push
    {
        public string app_id { get; set; }
        public List<string> include_player_ids { get; set; }
        public Idiomas headings { get; set; }
        public Idiomas contents { get; set; }
        public string android_group { get; set; }
    }

    public class Idiomas
    {
        public string en { get; set; }
        public string es { get; set; }
    }

}