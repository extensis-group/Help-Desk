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

namespace HelpDesk.Controllers
{
    [Authorize]
    public class ModulesController : ApiController
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET api/Module
        public IQueryable<ModuleDTO> GetModules()
        {
            var modules = from m in db.Modules
                           select new ModuleDTO()
                           {
                               Id = m.Id,
                               Name = m.Name,
                               HasActivities = m.Activities.Count > 0
                           };
            return modules;
        }

        // GET api/Module/5
        [ResponseType(typeof(Module))]
        public IHttpActionResult GetModule(int id)
        {
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return NotFound();
            }
            db.Entry(module).Collection(m => m.Activities).Load();
            foreach (var activity in module.Activities)
            {
                db.Entry(activity).Collection(a => a.Tags).Load();
            }
            return Ok(module);
        }

        // PUT api/Module/5
        public IHttpActionResult PutModule(int id, Module module)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != module.Id)
            {
                return BadRequest();
            }

            db.Entry(module).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(id))
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

        // POST api/Module
        [ResponseType(typeof(Module))]
        public IHttpActionResult PostModule(Module module)
        {
            Product product = new Product();
            try
            {
                product = db.Products.Find(module.ProductId);
            }
            catch
            {
                ModelState.AddModelError("Product", "Could not find product with given id");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            module.Product = product;
            db.Modules.Add(module);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = module.Id }, module);
        }

        // DELETE api/Module/5
        [ResponseType(typeof(Module))]
        public IHttpActionResult DeleteModule(int id)
        {
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return NotFound();
            }

            db.Modules.Remove(module);
            db.SaveChanges();

            return Ok(module);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ModuleExists(int id)
        {
            return db.Modules.Count(e => e.Id == id) > 0;
        }
    }
}