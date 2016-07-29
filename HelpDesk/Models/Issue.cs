using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using HelpDesk.Models.DTO;
using System.Web.Mvc;

namespace HelpDesk.Models
{
    public class Issue : ViewedEntity
    {
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(10000)]
        [Required]
        public string Description { get; set; }
        [DefaultValue(false)]
        public bool WasSeenByAdmin { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int StepId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual Step Step { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Solution> Solutions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual ICollection<Tag> Tags { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //[JsonIgnore]
        public virtual ApplicationUser User { get; set; }
    }
}