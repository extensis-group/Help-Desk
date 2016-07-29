using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HelpDesk.Models;
using HelpDesk.DAL;
using HelpDesk.Models.DTO;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web;

namespace HelpDesk.Controllers
{
    //[Authorize]
    public class ActivitiesController : HelpDeskApiController
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET api/Activities
        public IQueryable<ActivityDTO> GetActivity()
        {
            //string search = HttpContext.Current.Request.QueryString["search"];
            var activities = from a in db.Activities
                             select new ActivityDTO()
                             {
                                 Id = a.Id,
                                 Name = a.Name,
                                 Description = a.Description,
                                 ModuleName = a.Module.Name,
                                 ProductName = a.Module.Product.Name,
                                 TagNames = a.Tags.Select(t => t.Name).ToList()
                             };
            if (string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["search"]))
            {
                return activities;
            }
            List<ActivityDTO> results = new List<ActivityDTO>();

            string[] separators = { ",", ".", "!", "?", ";", ":", " " };
            string[] search = HttpContext.Current.Request.QueryString["search"].Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (search.Count() > 0)
            {
                foreach (var searchTerm in search)
                {
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        results.AddRange(activities.Where(
                                a => a.Name.Contains(searchTerm) ||
                                a.Description.Contains(searchTerm) ||
                                a.ModuleName.Contains(searchTerm) ||
                                a.ProductName.Contains(searchTerm) ||
                                a.TagNames.Contains(searchTerm)
                                ).ToList());

                    }  
                }
            }


            return results.AsQueryable().GroupBy(x => x.Id).Select(y => y.First()); ;
        }

        // GET api/Activities/5
        [ResponseType(typeof(Activity))]

        public IHttpActionResult GetActivity(int id)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return NotFound();
            }
            activity.Steps = activity.Steps.OrderBy(s => s.Order).ToList();

            bool createView = HttpContext.Current.Request.QueryString["createView"] != "false" || HttpContext.Current.Request.QueryString["createView"] == null;
            if (createView)
            {
                this.CreateView(activity);
            }

            string includeParam = HttpContext.Current.Request.QueryString["include"];
            if (!string.IsNullOrWhiteSpace(includeParam))
            {
                string[] separators = { "," };
                string[] includes = includeParam.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                foreach (var include in includes)
                {
                    if (include.ToLower() == "views")
                    {
                        db.Entry(activity).Collection(a => a.Views).Load();
                    }
                    if (include.ToLower() == "steps")
                    {
                        db.Entry(activity).Collection(a => a.Steps).Load();
                        activity.Steps = activity.Steps.OrderBy(s => s.Order).ToList();
                    }
                    if (include.ToLower() == "tags")
                    {
                        db.Entry(activity).Collection(a => a.Tags).Load();
                    }
                }
            }

            return Ok(activity);
        }

        // GET api/Activities/5/Views
        [ResponseType(typeof(Issue))]
        [Route("api/activities/{id}/views")]
        [HttpGet]
        public IQueryable<View> GetIssueViews(int id)
        {
            var views = db.Views.Where(v => v.ActivityId == id).AsQueryable();
            //var views = db.Views.Where(v => v.ActivityId == id).GroupBy(v => EntityFunctions.TruncateTime(v.ViewedAt)).ToList();

            return views;
        }

        // PUT api/Activities/5
        public IHttpActionResult PutActivity(int id, Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activity.Id)
            {
                return BadRequest();
            }

            db.Entry(activity).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Activities
        [ResponseType(typeof(Activity))]
        public async Task<IHttpActionResult> PostActivity(Activity activity)
        {
            Module module = new Module();  
            module = await db.Modules.FindAsync(activity.ModuleId);
            
            if (module == null)
            {
                ModelState.AddModelError("Module", "Module not found");

            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            activity.Module = module;
            //activity.User = _User;
            db.Activities.Add(activity);
            
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = activity.Id }, activity);
        }

        // DELETE api/Activities/5
        [ResponseType(typeof(Activity))]
        public async Task<IHttpActionResult> DeleteActivity(int id)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return NotFound();
            }
            await db.Entry(activity).Collection(a => a.Views).LoadAsync();
            foreach (var view in activity.Views.ToList())
            {
                db.Views.Remove(view);
            }
            db.Activities.Remove(activity);
            db.SaveChanges();

            return Ok(activity);
        }

        // Put api/activities/5/tags
        [ResponseType(typeof(Activity))]
        [Route("api/activities/{id}/tags/{tagId}")]
        [HttpPut]
        public async Task<IHttpActionResult> AttachTag(int id, int tagId)
        {
             
            Activity activity = await db.Activities.FindAsync(id);
            Tag tag = await db.Tags.FindAsync(tagId);
             
             
            if (activity == null || tag == null)
            {
                return NotFound();
            }
            activity.Tags.Add(tag);
            db.Entry(activity).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok(activity);

        }

        // Delete api/activities/5/tags
        [ResponseType(typeof(Activity))]
        [Route("api/activities/{id}/tags/{tagId}/detach")]
        [HttpDelete]
        public async Task<IHttpActionResult> DetachTag(int id, int tagId)
        {
            Activity activity = await db.Activities.FindAsync(id);
            Tag tag = await db.Tags.FindAsync(tagId);

            if (activity == null || tag == null)
            {
                return NotFound();
            }
            db.Entry(activity).Collection(a => a.Tags).Load();
            activity.Tags.Remove(tag);
            db.Entry(activity).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok(activity);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActivityExists(int id)
        {
            return db.Activities.Count(e => e.Id == id) > 0;
        }

    }
}