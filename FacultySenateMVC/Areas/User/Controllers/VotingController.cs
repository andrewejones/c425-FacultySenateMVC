using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Models;
using FacultySenateMVC.Helpers;

namespace FacultySenateMVC.Areas.User.Controllers
{
    public class VotingController : UserController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        public ActionResult Details(int id = 0)
        {
            // check that election exists
            Election election = db.Elections.Find(id);
            if (election == null)
            {
                return HttpNotFound();
            }

            // check that we're in election phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.VotingStartDate || election.VotingEndDate < now)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["election"] = election;
            ViewData["nominees"] = ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).vote_getVotableNominees((Faculty)Session["user"], election);

            return View();
        }
        
        public ActionResult Vote(int electionid = 0, int committeeid = 0, int nomineeid = 0)
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

            // check that nominee exists
            Faculty nominee = db.Faculties.Find(nomineeid);
            if (nominee == null)
            {
                return HttpNotFound();
            }

            // check that we're in election phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.VotingStartDate || election.VotingEndDate < now)
            {
                return RedirectToAction("Index", "Home"); // redirect home if not
            }

            ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).vote_vote((Faculty)Session["user"], nominee, committee, election);

            return RedirectToAction("Details", "Voting", new { id = electionid });
        }

        public ActionResult Unvote(int electionid = 0, int committeeid = 0, int nomineeid = 0)
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

            // check that nominee exists
            Faculty nominee = db.Faculties.Find(nomineeid);
            if (nominee == null)
            {
                return HttpNotFound();
            }

            // check that we're in election phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.VotingStartDate || election.VotingEndDate < now)
            {
                return RedirectToAction("Index", "Home"); // redirect home if not
            }

            Faculty user = (Faculty)Session["user"];

            ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).vote_unvote((Faculty)Session["user"], nominee, committee, election);

            return RedirectToAction("Details", "Voting", new { id = electionid });
        }
        
        public ActionResult Nominees(int electionid = 0)
        {
            // check that election exists
            Election election = db.Elections.Find(electionid);
            if (election == null)
            {
                return HttpNotFound();
            }

            ViewData["election"] = election;
            ViewData["nominees"] = ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).vote_getAllNominees(election);

            return View();
        }

    }
}
