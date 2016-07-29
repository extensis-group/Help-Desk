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
using System.Web;

namespace HelpDesk.Controllers
{

    public class TagsController : ApiController
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET api/Tags
        public IQueryable<TagDTO> GetTags()
        {

            string search = HttpContext.Current.Request.QueryString["search"];
            //DbSet<Tag> tagSet = db.Tags;
            //if(search != null){
            //    var tags = from t in tagSet 
            //                where t.Name.Contains(search)
            //               select new TagDTO()
            //               {
            //                   Id = t.Id,
            //                   Name = t.Name
            //               };
            //}
            var tags = from t in db.Tags
                        select new TagDTO()
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Activities = t.Activities
                        };
            if (!string.IsNullOrWhiteSpace(search))
            {
                tags = tags.Where(t => t.Name.Contains(search));
            }
            return tags;
        }

        // GET api/Tags/5
        [ResponseType(typeof(Tag))]
        public IHttpActionResult GetTag(int id)
        {
            Tag tag = db.Tags.Find(id);
            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }

        // PUT api/Tags/5
        public IHttpActionResult PutTag(int id, Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tag.Id)
            {
                return BadRequest();
            }

            db.Entry(tag).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
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

        // POST api/Tags
        [ResponseType(typeof(Tag))]
        public IHttpActionResult PostTag(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
             
            db.Tags.Add(tag);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error",e.InnerException.InnerException);
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("DefaultApi", new { id = tag.Id }, tag);
        }

        // DELETE api/Tags/5
        [ResponseType(typeof(Tag))]
        public IHttpActionResult DeleteTag(int id)
        {
            Tag tag = db.Tags.Find(id);
            if (tag == null)
            {
                return NotFound();
            }

            db.Tags.Remove(tag);
            db.SaveChanges();

            return Ok(tag);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TagExists(int id)
        {
            return db.Tags.Count(e => e.Id == id) > 0;
        }
    }
}