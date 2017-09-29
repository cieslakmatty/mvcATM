using AutomatedTellerMachine.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutomatedTellerMachine.Controllers {
    public class HomeController : Controller {

        private ApplicationDbContext db = new ApplicationDbContext();

        //[MyLoggingFilter]
        [Authorize]
        public ActionResult Index() {

            var userId = User.Identity.GetUserId();
            var checkingAccountId = db.CheckingAccounts.Where(c => c.ApplicationUserId == userId).First().Id;
            ViewBag.CheckingAccountId = checkingAccountId;

            var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = manager.FindById(userId);
            ViewBag.Pin = user.Pin;
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Having trouble? Send us a message.";

            return View();
        }

        [HttpPost]//attribute
        public ActionResult Contact(string message) {

            //TODO: send message to the HQ
            ViewBag.Message = "Thanks, we got your message.";

            return PartialView("_ContactThanks");
        }

        public ActionResult Foo() {
            return View("About");
        }

        public ActionResult Serial(string letterCase) {
            string serial = "ASPNETMVC5ATM1";
            if (letterCase == "lower") {
                return Content(serial.ToLower());
            }
            return Content(serial);
            //return new HttpStatusCodeResult(403);
            //return Json(new { name = "serial", value = serial }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index");
        }
    }
}