using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using HelpDesk.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using HelpDesk.DAL;
using System.Data.Entity;

namespace HelpDesk.Controllers
{
    public class HelpDeskApiController : ApiController
    {

        private HelpDeskContext db = new HelpDeskContext();
        protected UserManager<ApplicationUser> userManager;
        private UserStore<ApplicationUser> userStore;
        protected ApplicationUser _User;
        protected string[] separators = { ",", ".", "!", "?", ";", ":", " " };
        protected enum Entity { Users };
        protected Dictionary<string, Entity> EntityDict = new Dictionary<string,Entity> {
                                                                {"Users", Entity.Users}
                                                          };

        //public HelpDeskApiController()
        //    : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new HelpDeskContext())))
        //{

        //}
        public HelpDeskApiController()
        {
            this.userStore = new UserStore<ApplicationUser>(new HelpDeskContext());
            this.userManager = new UserManager<ApplicationUser>(this.userStore);

            this._User = this.userManager.FindByName(User.Identity.GetUserName());
        }

        protected async Task<ApplicationUser> GetApplicationUserAsync()
        {
            var userName = User.Identity.GetUserName();
            ApplicationUser user = await userManager.FindByNameAsync(userName);
            return user;
        }

        protected ApplicationUser GetApplicationUser()
        {
            var userName = User.Identity.GetUserName();
            ApplicationUser user = userManager.FindByName(userName);
            return user;
        }

        protected void CreateView(ViewedEntity item)
        {
            View view = new View();
            view.UserId = this._User == null ? null : this._User.Id;
            view.ActivityId = item is Activity ? item.Id : (int?)null;
            view.IssueId = item is Issue ? item.Id : (int?)null;

            db.Views.Add(view);

            db.SaveChanges();
        }

        //protected void joinEntities(IQueryable set, string[] withEntitiesArray){
        //    foreach (var withEntity in withEntitiesArray)
        //    {
        //        switch (withEntity.ToLower())
        //        {
        //            case "users":
        //                foreach (var item in set)
        //                {
        //                    item.User = db.Users.Find(item.UserId);
        //                }
        //                break;
        //        }
        //    }
        //}

    }
}
