using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpDesk.Models.DTO
{
    public class TagDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}