using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using HelpDesk.Attributes;

namespace HelpDesk.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [Index]
        [Index("NameIndex", IsUnique = true)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual ICollection<Activity> Activities { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Issue> Issues { get; set; }
    }
}