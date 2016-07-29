using HelpDesk.DAL;
using HelpDesk.Models;
using HelpDesk.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HelpDesk.Controllers
{
    public class ViewsController : HelpDeskApiController
    {
        protected HelpDeskContext db = new HelpDeskContext();

        [Route("api/views/activities/{activityId}")]
        [HttpGet]
        public List<ViewDTO> OfActivity(int activityId)
        {
            string startDateStr = HttpContext.Current.Request.QueryString["startDate"];
            string endDateStr = HttpContext.Current.Request.QueryString["endDate"];
            string withEntityKeysStr = HttpContext.Current.Request.QueryString["with"];

            IQueryable<ViewDTO> views = from v in db.Views
                            .Where(v => v.Activity.Id == activityId)
                                     select new ViewDTO
                                     {
                                         Id = v.Id,
                                         ViewedAt = v.ViewedAt,
                                         Activity = v.Activity,
                                         UserId = v.UserId,
                                         User = new UserDTO
                                         {
                                             Id = v.User.Id,
                                             FirstName = v.User.FirstName,
                                             LastName = v.User.LastName,
                                             UserName = v.User.UserName,
                                             IsAuthenticated = v.User.IsAuthenticated,
                                             Roles = db.Users
                                                         .Where(u => u.Id == v.User.Id)
                                                         .SelectMany(u => u.Roles)
                                                         .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                                                         .ToList()
                                         }
                                     };

            if (!string.IsNullOrEmpty(startDateStr))
            {
                DateTime startDate = Convert.ToDateTime(startDateStr);
                views = views.Where(v => v.ViewedAt >= startDate);
            }
            if (!string.IsNullOrEmpty(endDateStr))
            {
                DateTime endDate = Convert.ToDateTime(endDateStr);
                views = views.Where(v => v.ViewedAt <= endDate);
            }
            //if (!string.IsNullOrEmpty(withEntityKeysStr))
            //{
            //    string[] withEntitiesArray = withEntityKeysStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (var withEntity in withEntitiesArray)
            //    {
            //        switch (withEntity.ToLower())
            //        {
            //            case "users":
            //                foreach(var view in views){
            //                    view.User = db.Users.Find(view.UserId);
            //                }                          
            //                break;
            //        }
            //    }
            //}


            return views.ToList();
        } 

        [Route("api/views/modules/{moduleId}")]
        [HttpGet]
        public List<ViewDTO> OfModule(int moduleId)
        {
            string startDateStr = HttpContext.Current.Request.QueryString["startDate"];
            string endDateStr = HttpContext.Current.Request.QueryString["endDate"];
            string withEntityKeysStr = HttpContext.Current.Request.QueryString["with"];

            IQueryable<ViewDTO> views = from v in db.Views
                                .Where(v => v.Activity.ModuleId == moduleId)
                                select new ViewDTO
                                            {
                                                Id = v.Id,
                                                ViewedAt = v.ViewedAt,
                                                Activity = v.Activity,
                                                UserId = v.UserId,
                                                User = new UserDTO
                                                {
                                                    Id=v.User.Id,
                                                    FirstName=v.User.FirstName,
                                                    LastName=v.User.LastName,
                                                    UserName = v.User.UserName,
                                                    IsAuthenticated = v.User.IsAuthenticated,
                                                    Roles = db.Users
                                                                .Where(u => u.Id == v.User.Id)
                                                                .SelectMany(u => u.Roles)
                                                                .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                                                                .ToList()
                                                }
                                            };

            if (!string.IsNullOrEmpty(startDateStr))
            {
                DateTime startDate = Convert.ToDateTime(startDateStr);
                views = views.Where(v => v.ViewedAt >= startDate);
            }
            if (!string.IsNullOrEmpty(endDateStr))
            {
                DateTime endDate = Convert.ToDateTime(endDateStr);
                views = views.Where(v => v.ViewedAt <= endDate);
            }
            //if (!string.IsNullOrEmpty(withEntityKeysStr))
            //{
            //    string[] withEntitiesArray = withEntityKeysStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (var withEntity in withEntitiesArray)
            //    {
            //        switch (withEntity.ToLower())
            //        {
            //            case "users":
            //                foreach (var view in views)
            //                {
            //                    view.User = db.Users.Find(view.UserId);
            //                }
            //                break;
            //        }
            //    }
            //}

            return views.ToList();
        } 

        [Route("api/views/products/{productId}")]
        [HttpGet]
        public List<ViewDTO> OfProduct(int productId)
        {
            string startDateStr = HttpContext.Current.Request.QueryString["startDate"];
            string endDateStr = HttpContext.Current.Request.QueryString["endDate"];
            string withEntityKeysStr = HttpContext.Current.Request.QueryString["with"];

            IQueryable<ViewDTO> views = from v in db.Views.Where(v => v.Activity.Module.ProductId == productId)
                                        select new ViewDTO
                                        {
                                            Id = v.Id,
                                            ViewedAt = v.ViewedAt,
                                            Activity = v.Activity,
                                            UserId = v.UserId,
                                            User = new UserDTO
                                            {
                                                Id=v.User.Id,
                                                FirstName=v.User.FirstName,
                                                LastName=v.User.LastName,
                                                UserName = v.User.UserName,
                                                IsAuthenticated = v.User.IsAuthenticated,
                                                Roles = db.Users
                                                            .Where(u => u.Id == v.User.Id)
                                                            .SelectMany(u => u.Roles)
                                                            .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                                                            .ToList()
                                            }
                                        };
            if (!string.IsNullOrEmpty(startDateStr))
            {
                DateTime startDate = Convert.ToDateTime(startDateStr);
                views = views.Where(v => v.ViewedAt >= startDate);
            }
            if (!string.IsNullOrEmpty(endDateStr))
            {
                DateTime endDate = Convert.ToDateTime(endDateStr);
                endDate = endDate.AddDays(1.0).AddMilliseconds(-1.0);
                views = views.Where(v => v.ViewedAt <= endDate);
            } 
            //if (!string.IsNullOrEmpty(withEntityKeysStr))
            //{
            //    string[] withEntitiesArray = withEntityKeysStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (var withEntity in withEntitiesArray)
            //    {
            //        switch (withEntity.ToLower())
            //        {
            //            case "users":
            //                foreach (var view in views)
            //                {
                                
                               
            //                    //System.Diagnostics.Debug.WriteLine(view.User.Roles[0].Name);
            //                }
            //                break;
            //        }
            //    }
            //}
            return views.ToList();
        }
    }
}