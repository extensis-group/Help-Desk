using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpDesk.Models.DTO
{
    public class RoleDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserDTO> Users { get; set; }
    }
}