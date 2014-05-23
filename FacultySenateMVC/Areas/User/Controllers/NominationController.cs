using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Models;
using FacultySenateMVC.Helpers;

namespace FacultySenateMVC.Areas.User.Controllers
{
    public class NominationController : UserController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        public ActionResult Details(int id = 0)
        {
            // check election exists
            Election election = db.Elections.Find(id);
            if (election == null)
            {
                return HttpNotFound();
            }

            // check in nomination phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.NominationStartDate || election.NominationEndDate < now)
            {
                return RedirectToAction("Index", "Home");
            }

            // create lists
            List<Committee> nom = new List<Committee>(), elig = new List<Committee>(), inelig = new List<Committee>();

            // populate lists
            ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).nom_getEligibilityLists((Faculty)Session["user"], election, nom, elig, inelig);

            // make lists accessible to View
            ViewData["election"] = election;
            ViewData["nominated"] = nom;
            ViewData["eligible"] = elig;
            ViewData["ineligible"] = inelig;

            return View();
        }

        public ActionResult Nominees(int electionid = 0, int committeeid = 0)
        {
            // check that election exists
            Election election = db.Elections.Find(electionid);
            if (election == null)
            {
                return HttpNotFound();
            }

            // check that committee exists
            Committee committee = db.Committees.Find(committeeid);
            if (committee == null)
            {
                return HttpNotFound();
            }

            List<ElectionNomination> nominees = new List<ElectionNomination>();
            nominees = ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).nom_getCommitteeNominees(election, committee);

            ViewData["election"] = election;
            ViewData["nominees"] = nominees;

            return View();
        }

        public ActionResult Reasons(int electionid = 0, int committeeid = 0)
        {
            // check that election exists
            Election election = db.Elections.Find(electionid);
            if (election == null)
            {
                return HttpNotFound();
            }

            // check that committee exists
            Committee committee = db.Committees.Find(committeeid);
            if (committee == null)
            {
                return HttpNotFound();
            }

            // check that we're in a nomination phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.NominationStartDate || election.NominationEndDate < now)
            {
                return HttpNotFound();
            }

            ViewData["reasons"] = ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).nom_getIneligibilityReasons((Faculty)Session["user"], election, committee);

            return View();
        }

        public ActionResult Nominate(int electionid = 0, int committeeid = 0) 
        {
            // check that election exists
            Election election = db.Elections.Find(electionid);
            if (election == null)
            {
                return HttpNotFound();
            }

            // check that committee exists
            Committee committee = db.Committees.Find(committeeid);
            if (committee == null)
            {
                return HttpNotFound();
            }

            // check that we're in nomination phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.NominationStartDate || election.NominationEndDate < now)
            {
                return RedirectToAction("Index", "Home"); // redirect home if not
            }

            ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).nom_nominate((Faculty)Session["user"], election, committee);

            return RedirectToAction("Details", "Nomination", new { id = electionid });
        }

        public ActionResult Unnominate(int electionid = 0, int committeeid = 0)
        {
            // check that election exists
            Election election = db.Elections.Find(electionid);
            if (election == null)
            {
                return HttpNotFound();
            }

            // check that committee exists
            Committee committee = db.Committees.Find(committeeid);
            if (committee == null)
            {
                return HttpNotFound();
            }

            // check that we're in nomination phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.NominationStartDate || election.NominationEndDate < now)
            {
                return RedirectToAction("Index", "Home"); // redirect home if not
            }

            ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).nom_unnominate((Faculty)Session["user"], election, committee);

            return RedirectToAction("Details", "Nomination", new { id = electionid }); // redirect to nominations page
        }

    }
}
