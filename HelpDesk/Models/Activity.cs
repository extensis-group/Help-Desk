using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.Models
{
    public class Activity : ViewedEntity
    {
        public Activity() : base()
        {
            this.Steps = new List<Step>();
            this.Tags = new List<Tag>();
        }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        [MaxLength(250)]
        [Required]
        public string Description { get; set; }

        [Required]
        public int ModuleId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore] 
        public virtual Module Module { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Step> Steps { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Tag> Tags { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
        
    }
}