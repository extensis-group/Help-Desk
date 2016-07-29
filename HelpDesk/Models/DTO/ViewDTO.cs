using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpDesk.Models.DTO
{
    public class ViewDTO
    {
        public int Id { get; set; }
        public DateTime ViewedAt { get; set; }
        public string UserId { get; set; }
        public UserDTO User { get; set; }
        public Activity Activity { get; set; }
        public Issue Issue { get; set; }

    }
}