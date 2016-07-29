using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpDesk.Models.DTO
{
    public class ActivityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ModuleName { get; set; }
        public string ProductName { get; set; }
        public List<string> TagNames { get; set; }
    }
}