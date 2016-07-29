using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace HelpDesk.Models
{
    public class Step
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        [MaxLength(10000)]
        [Required]
        [AllowHtml]
        public string Description { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage="Order Must be Between 1 and 100")]
        public int Order { get; set; }
        
        [Required]
        public int ActivityId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual Activity Activity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Issue> Issues { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

    }
}