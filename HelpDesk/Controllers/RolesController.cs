using HelpDesk.DAL;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using HelpDesk.Models.DTO;

namespace HelpDesk.Controllers
{
    public class RolesController : HelpDeskApiController
    {
        private HelpDeskContext db = new HelpDeskContext();
        private RoleManager<IdentityRole> roleManager;

        public RolesController() : base()
        {
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        
        }

        // GET api/Roles
        public IQueryable<IdentityRole> GetRoles()
        {
            return db.Roles;
        
        }

        [ResponseType(typeof(IdentityRole))]
        [Route("api/roles/{id}")]
        [HttpGet]
        public IHttpActionResult GetRole(string id)
        {
            var role = db.Roles.Find(id);
            var roleDTO = new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                Users = (from user in db.Users
                         where user.Roles.Any(ur => ur.RoleId == id)
                         where user.IsAuthenticated.Equals(true)
                         select new UserDTO
                         {
                             Id = user.Id,
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             UserName = user.UserName,
                             IsAuthenticated = user.IsAuthenticated
                         }
                        ).ToList()
            };
            return Ok(roleDTO);
        }

        [ResponseType(typeof(IdentityRole))]
        public async Task<IHttpActionResult> PostRole(IdentityRole role)
        {
            if (roleManager.RoleExists(role.Name))
            {
                ModelState.AddModelError("Name", "Role name exists");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newRole = new IdentityRole(role.Name);
            var roleResult = roleManager.Create(newRole);

            await db.SaveChangesAsync();



            return Ok(newRole);

        }

        [Route("api/roles/{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutRole(string id, IdentityRole role)
        {
            if (role.Name.ToLower().Equals("admin"))
            {
                ModelState.AddModelError("Admin", "Cannot change admin role");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!RoleExists(id))
            {
                return NotFound();
            }

            db.Entry(role).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok(role);
        }

        // DELETE api/Roles/5
        [ResponseType(typeof(IdentityRole))]
        [Route("api/roles/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteRole(string id)
        {

            IdentityRole role = db.Roles.Find(id);
            if (role.Name.ToLower().Equals("admin"))
            {
                ModelState.AddModelError("Admin", "Cannot change admin role");
                return BadRequest(ModelState);
            }
            if (role == null)
            {
                return NotFound();
            }

            db.Roles.Remove(role);
            await db.SaveChangesAsync();

            return Ok(role);
        }

        private bool RoleExists(string id)
        {
            return db.Roles.Count(e => e.Id == id) > 0;
        }
    }
}
