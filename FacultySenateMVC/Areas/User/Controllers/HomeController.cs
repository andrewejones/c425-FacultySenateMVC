using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Models;

namespace FacultySenateMVC.Areas.User.Controllers
{
    public class HomeController : UserController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        public ActionResult Index()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            // create list of current elections
            ViewData["currelections"] = db.Elections.Where(e => (e.NominationStartDate < now && now < e.NominationEndDate) || (e.VotingStartDate < now && now < e.VotingEndDate)).ToList();

            // create list of next elections
            ViewData["nextelections"] = db.Elections.Where(e => now < e.NominationStartDate).ToList();

            return View();
        }

    }
}
