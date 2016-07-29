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
using System.Web;
using System.Net.Mail;

namespace HelpDesk.Controllers
{
    [Authorize]
    public class IssuesController : HelpDeskApiController
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET api/Issues
        public async Task<IQueryable<IssueDTO>> GetIssues()
        {
            var issues = from i in db.Issues
                        select new IssueDTO()
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Description = i.Description,
                            CreatedAt = i.CreatedAt,
                            UpdatedAt = i.UpdatedAt,
                            Solutions = i.Solutions.OrderByDescending(s => s.CreatedAt).ToList()
                        };
            string searchParam = HttpContext.Current.Request.QueryString["search"];
            if (string.IsNullOrWhiteSpace(searchParam))
            {
                return issues;
            }
            List<IssueDTO> results = new List<IssueDTO>();
            string[] separators = { ",", ".", "!", "?", ";", ":", " " };
            string[] search = HttpContext.Current.Request.QueryString["search"].Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (search.Count() > 0)
            {
                foreach (var searchTerm in search)
                {
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        results.AddRange(issues.Where(
                                i => i.Name.Contains(searchTerm) ||
                                i.Description.Contains(searchTerm) 
                             ).ToList());

                    }
                }
            }

            return results.AsQueryable().GroupBy(x => x.Id).Select(y => y.First()); ;
        }

        // GET api/Issues/5
        [ResponseType(typeof(Issue))]
        public async Task<IHttpActionResult> GetIssue(int id)
        {
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return NotFound();
            }
            await db.Entry(issue).Collection(i => i.Solutions).LoadAsync();
            if (issue.Solutions != null)
            {
                issue.Solutions = issue.Solutions.OrderByDescending(s => s.CreatedAt).ToList();
            }

            if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["include"]))
            {
                string[] separators = { "," };
                string[] includes = HttpContext.Current.Request.QueryString["include"].Split(separators, StringSplitOptions.RemoveEmptyEntries);

                foreach (var include in includes)
                {
                    if (include.ToLower() == "user")
                    {
                        issue.User = db.Users.Find(issue.UserId);
                    }
                }
            }
            this.CreateView(issue);
            return Ok(issue);
        }

        // GET api/Issues/5/Views
        [ResponseType(typeof(Issue))]
        [Route("api/issues/{id}/views")]
        [HttpGet]
        public IQueryable<View> GetIssueViews(int id)
        {
            var views = db.Views.Where(v => v.IssueId == id).AsQueryable();
            return views;
        }

        // PUT api/Issues/5
        public IHttpActionResult PutIssue(int id, Issue issue)
        {
            Step step = new Step();
            try
            {
                step = db.Steps.Find(issue.StepId);
            }
            catch
            {
                ModelState.AddModelError("Step", "Step not found");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != issue.Id)
            {
                return BadRequest();
            }

            issue.UpdatedAt = DateTime.Now; 
            db.Entry(issue).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(id))
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

        // POST api/Issues
        [ResponseType(typeof(Issue))]
        public async Task<IHttpActionResult> PostIssue(Issue issue)
        {
            Step step = new Step();
            try{
                step = db.Steps.Find(issue.StepId);
            }
            catch 
            {
                ModelState.AddModelError("Step", "Step not found");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            issue.CreatedAt = DateTime.Now;
            issue.UpdatedAt = DateTime.Now;
            issue.Step = step;
            var user = await this.GetApplicationUserAsync();

            issue.UserId = user.Id;
            //issue.User = user;
            //issue.User = _User;

            db.Issues.Add(issue);
            db.SaveChanges();

            //move to production, then do emailer
            //await notifyAdmin();

            return CreatedAtRoute("DefaultApi", new { id = issue.Id }, issue);
        }

        // DELETE api/Issues/5
        [ResponseType(typeof(Issue))]
        public IHttpActionResult DeleteIssue(int id)
        {
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return NotFound();
            }

            db.Issues.Remove(issue);
            db.SaveChanges();

            return Ok(issue);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IssueExists(int id)
        {
            return db.Issues.Count(e => e.Id == id) > 0;
        }

        private async Task<IHttpActionResult> notifyAdmin()
        {
            //var body = "<p>New user added</p>";
            //var message = new MailMessage();
            //message.To.Add(new MailAddress("name@gmail.com")); //replace with valid value
            //message.Subject = "New user added";
            //message.Body = body;
            //message.IsBodyHtml = true;
            //using (var smtp = new SmtpClient())
            //{
            //    await smtp.SendMailAsync(message);
            //    return Ok(message);
            //}
            MailMessage mail = new MailMessage();
            mail.To.Add("rstromberg@extensisgroup.com");
            mail.From = new MailAddress("rstromberg916@gmail.com");
            mail.Subject = "New issue posted";

            string Body = "Hi, this mail is to test sending mail" +
                          "using Gmail in ASP.NET";
            mail.Body = Body;

            mail.IsBodyHtml = true;
            SmtpClient sc = new SmtpClient("smtp.gmail.com");
            NetworkCredential nc = new NetworkCredential("rstromberg916", "5250stromberg");//username doesn't include @gmail.com
            sc.UseDefaultCredentials = false;
            sc.Credentials = nc;
            sc.EnableSsl = true;
            sc.Port = 587;
            try
            {
                await sc.SendMailAsync(mail);
            }
            catch (Exception e)
            {
                 
            }
            return Ok(mail);
        } 
    }
}