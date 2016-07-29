using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Controllers
{
    [Authorize]
    public class UserAppController : Controller
    {
        //
        // GET: /UserApp/
        public ActionResult Index()
        {

            return View();
        }
	}
}