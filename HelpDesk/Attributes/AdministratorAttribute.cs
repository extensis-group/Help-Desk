using HelpDesk.DAL;
using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Attributes
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public string Role { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            return hasRole(httpContext.User.Identity.GetUserId());
        }

        protected bool hasRole(string userId)
        {

            var roles = getUserRoles(userId);

            foreach (var role in roles)
            {
                if (role.Name == Role)
                {
                    return true;
                }
            }
            return false;
        }

        protected List<IdentityRole>getUserRoles(string userId){

            HelpDeskContext context = new HelpDeskContext();
            return context.Users
                            .Where(u => u.Id == userId)
                            .SelectMany(u => u.Roles)
                            .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                            .ToList();
        }

        //protected ApplicationUser getAppUser(string userName)
        //{
        //    UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
        //    return userManager.FindByName(userName);
        //}

        
    }
}