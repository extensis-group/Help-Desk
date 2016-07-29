using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Http;
using HelpDesk.DAL;

namespace HelpDesk.Tools
{
    public class UserInfo
    {
        static public ApplicationUser getAppUser()
        {

            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
            var user = HttpContext.Current.User;
             
            var userName = user.Identity.GetUserName();
            if (userName == null)
            {
                return null;
            }
            ApplicationUser appUser = userManager.FindByName(userName);

            return appUser;
        
        }

        static private string getAppUserId()
        {

            var user = HttpContext.Current.User;
            return user.Identity.GetUserId();

        }
        static public bool isAdmin()
        {
             

            HelpDeskContext context = new HelpDeskContext();
            //ApplicationUser user = getAppUser();
            string userId = getAppUserId();
            var roles = context.Users
                            .Where(u => u.Id == userId)
                            .SelectMany(u => u.Roles)
                            .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                            .ToList();

            foreach (var role in roles)
            {
                if (role.Name.ToLower() == "admin")
                {
                    return true;
                }
            }
            return false;
        }
    }
}