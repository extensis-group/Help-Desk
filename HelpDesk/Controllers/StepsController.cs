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
using System.Threading.Tasks;

namespace HelpDesk.Controllers
{
    [Authorize]
    public class StepsController : HelpDeskApiController
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET api/Steps
        public IQueryable<StepDTO> GetSteps()
        {
            var steps = from s in db.Steps
                           select new StepDTO()
                           {
                               Id = s.Id,
                               Name = s.Name,
                               Description = s.Description,
                               Order = s.Order
                           };
            return steps;
        }

        // GET api/Steps/5
        [ResponseType(typeof(Step))]
        public IHttpActionResult GetStep(int id)
        {
            Step step = db.Steps.Find(id);
            if (step == null)
            {
                return NotFound();
            }
            db.Entry(step).Collection(s => s.Issues).Load();
            return Ok(step);
        }

        // PUT api/Steps/5
        public async Task<IHttpActionResult> PutStep(int id, Step step)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != step.Id)
            {
                return BadRequest();
            }
            step.Activity = await db.Activities.FindAsync(step.ActivityId);
            db.Entry(step).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StepExists(id))
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

        // POST api/Steps
        [ResponseType(typeof(Step))]
        public IHttpActionResult PostStep(Step step)
        {
            var activity =  db.Activities.Find(step.ActivityId);
            if (activity == null)
            {
                ModelState.AddModelError("Activity", "Activity not found");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            step.Activity = activity;
            //step.User = _User;
            db.Steps.Add(step);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error saving changes", e);
                return BadRequest(ModelState);
            }
            return CreatedAtRoute("DefaultApi", new { id = step.Id }, step);
        }

        // DELETE api/Steps/5
        [ResponseType(typeof(Step))]
        public IHttpActionResult DeleteStep(int id)
        {
            Step step = db.Steps.Find(id);
            if (step == null)
            {
                return NotFound();
            }

            db.Steps.Remove(step);
            db.SaveChanges();

            return Ok(step);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StepExists(int id)
        {
            return db.Steps.Count(e => e.Id == id) > 0;
        }
    }
}