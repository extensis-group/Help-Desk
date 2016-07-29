using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.Models
{
    public class Solution
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(500)]
        [Required]
        public string Description { get; set; }
        [DefaultValue(true)]
        public bool IsCorrect { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int IssueId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual Issue Issue { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

    }
}