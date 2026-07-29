using System;
using System.Collections.Generic;

namespace DAL.DTOs
{
    public class SecurityFunctionDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Display { get; set; }
        public bool Show { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastEditTime { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string RoutesJson { get; set; }
        public List<string> SelectedRoutes { get; set; } = new List<string>();
    }

    public class SecurityFunctionCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> SelectedRoutes { get; set; } = new List<string>();
    }

    public class SecurityFunctionUpdateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Display { get; set; }
        public bool Show { get; set; }
        public List<string> SelectedRoutes { get; set; } = new List<string>();
    }

    public class SecurityFunctionRouteGroupDTO
    {
        public string ControllerName { get; set; }
        public string AreaName { get; set; }
        public List<string> Routes { get; set; } = new List<string>();
    }
}