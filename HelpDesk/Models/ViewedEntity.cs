using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public abstract class ViewedEntity
    {
        public ViewedEntity()
        {
            this.Views = new List<View>();
        }
        public int Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<View> Views { get; set; }
    }
}