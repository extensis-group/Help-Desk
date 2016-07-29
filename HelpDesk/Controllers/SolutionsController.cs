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
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.Controllers
{
    [Authorize]
    public class SolutionsController : HelpDeskApiController
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET api/Solutions
        public IQueryable<SolutionDTO> GetSolutions()
        {
            var solutions = from s in db.Solutions
                        select new SolutionDTO()
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Description = s.Description,
                            CreatedAt = s.CreatedAt,
                            UpdatedAt = s.UpdatedAt
                        };
            return solutions;
        }

        // GET api/Solutions/5
        [ResponseType(typeof(Solution))]
        public IHttpActionResult GetSolution(int id)
        {
            Solution solution = db.Solutions.Find(id);
           
            if (solution == null)
            {
                return NotFound();
            }
            
            return Ok(solution);
        }

        // PUT api/Solutions/5
        public IHttpActionResult PutSolution(int id, Solution solution)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != solution.Id)
            {
                return BadRequest();
            }

            db.Entry(solution).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SolutionExists(id))
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

        // POST api/Solutions
        [ResponseType(typeof(Solution))]
        public IHttpActionResult PostSolution(Solution solution)
        {
            var issue = db.Issues.Find(solution.IssueId);
            if (issue == null)
            {
                ModelState.AddModelError("Issue", "Issue not found");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            solution.IsCorrect = true;
            solution.CreatedAt = DateTime.Now;
            solution.UpdatedAt = DateTime.Now;
            solution.Issue = issue;
            //solution.User = _User;
            db.Solutions.Add(solution);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                ModelState.AddModelError("Error saving changes", e);
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("DefaultApi", new { id = solution.Id }, solution);
        }

        // DELETE api/Solutions/5
        [ResponseType(typeof(Solution))]
        public IHttpActionResult DeleteSolution(int id)
        {
            Solution solution = db.Solutions.Find(id);
            if (solution == null)
            {
                return NotFound();
            }

            db.Solutions.Remove(solution);
            db.SaveChanges();

            return Ok(solution);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SolutionExists(int id)
        {
            return db.Solutions.Count(e => e.Id == id) > 0;
        }
    }
}