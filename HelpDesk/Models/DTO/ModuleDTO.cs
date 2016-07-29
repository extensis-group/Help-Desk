using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpDesk.Models.DTO
{
    public class ModuleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasActivities { get; set; }
    }
}