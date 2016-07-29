using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Models.DTO;
using Newtonsoft.Json;

namespace HelpDesk.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Module> Modules { get; set; }

    }
}