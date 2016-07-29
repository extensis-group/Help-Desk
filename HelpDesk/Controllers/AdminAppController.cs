using HelpDesk.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Controllers
{
    [Authorize]
    [AuthorizeRole(Role="Admin")]
    public class AdminAppController : Controller
    {
        //
        // GET: /AdminApp/
        public ActionResult Index()
        {
            return View();
        }
	}
}