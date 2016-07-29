using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult RegistrationComplete()
        {
            ViewBag.Message = "Registration Complete.";
            return View();
        }


        public ActionResult Index()
        {

            return RedirectToAction("Index", "UserApp");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}