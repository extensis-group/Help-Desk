using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;

namespace HelpDesk.Models
{
    public class View
    {
        public View()
        {
            ViewedAt = DateTime.Now;
            
        }

        public int Id { get; set; }
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime ViewedAt { get; set; }

        //TODO: Finish relation when users are added
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? ActivityId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual Activity Activity { get; set; }

        public int? IssueId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual Issue Issue { get; set; }

    }
}