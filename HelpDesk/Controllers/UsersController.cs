using HelpDesk.Attributes;
using HelpDesk.DAL;
using HelpDesk.Models;
using HelpDesk.Models.DTO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;

namespace HelpDesk.Controllers
{
    public class UsersController : HelpDeskApiController
    {
        private HelpDeskContext db = new HelpDeskContext();
        //
        // GET: /Api/Users/
        [AuthorizeRole(Role = "Admin")]
        public IQueryable<UserDTO> GetUsers()
        {
           
            var appUsers = from u in db.Users
                           select new UserDTO()
                           {
                               Id = u.Id,
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               UserName = u.UserName,
                               IsAuthenticated = u.IsAuthenticated,
                               Roles = db.Users
                                            .Where(user => user.Id == u.Id)
                                            .SelectMany(user => user.Roles)
                                            .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                                            .ToList()
                           };
            foreach (var appUser in appUsers)
            {

                appUser.Roles = getUserRoles(appUser.Id);
                //foreach (var role in roles)
                //{
                //     
                //    appUser.Roles = ro
                //}
                 
            }
             
            return appUsers;
        }

        //
        // Post: /Api/Users/
        [Route("api/users/{userId}/authenticate")]
        [HttpPost]
        public IHttpActionResult Authenticate(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            user.IsAuthenticated = true;
            db.SaveChanges();
            
            return Ok(user);
        }

        //
        // Post: /Api/Users/
        [Route("api/users/{userId}/deauthenticate")]
        [HttpPost]
        public IHttpActionResult Deauthenticate(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            user.IsAuthenticated = false;
            db.SaveChanges();

            return Ok(user);
        }

        [Route("api/users/{userId}/role")]
        [HttpPost]
        public async Task<IHttpActionResult> AddRole(string userId, IdentityRole role)
        {
            ApplicationUser user = db.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.AddToRoleAsync(user.Id, role.Name);
            return Ok(user);
        }

        [Route("api/users/{userId}/role")]
        [HttpDelete]
        public async Task<IHttpActionResult> RemoveRole(string userId, IdentityRole role)
        {
            ApplicationUser user = db.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            await userManager.RemoveFromRoleAsync(user.Id, role.Name);
            return Ok(user);
        }

        //
        // Post: /Api/Users/5
        [Route("api/users/{userId}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteUser(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            db.Users.Remove(user);
            db.SaveChanges();


            return Ok(user);
        }
        //
        // Get: test
        [Route("api/users/{userId}/roles")]
        [HttpGet]
        public async Task<IHttpActionResult> Roles(string userId)
        {
            var roles = db.Users
                            .Where(u => u.Id == userId)
                            .SelectMany(u => u.Roles)
                            .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                            .ToList();
            
            return Ok(roles);
        }

        [Route("api/users/{userId}/views")]
        [HttpGet]
        public List<View> Views(string userId)
        {
            var views = db.Views
                            .Where(v => v.UserId == userId).ToList();

            return views;
        }

        protected List<IdentityRole> getUserRoles(string userId)
        {

            HelpDeskContext context = new HelpDeskContext();
            return context.Users
                            .Where(u => u.Id == userId)
                            .SelectMany(u => u.Roles)
                            .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                            .ToList();
        }


    }
}
