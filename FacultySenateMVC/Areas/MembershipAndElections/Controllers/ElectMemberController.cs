using FacultySenateMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Helpers;

namespace FacultySenateMVC.Areas.MembershipAndElections.Controllers
{
    public class ElectMemberController : MembershipAndElectionsController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        public ActionResult Index()
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            // create list of past elections
            ViewData["pastelections"] = db.Elections.Where(e => e.VotingEndDate < now).OrderByDescending(e => e.VotingEndDate).ToList();

            // create list of current elections
            ViewData["currelections"] = db.Elections.Where(e => (e.NominationStartDate < now && now < e.NominationEndDate) || (e.VotingStartDate < now && now < e.VotingEndDate)).ToList();

            return View();
        }

        public ActionResult Details(int id = 0)
        {
            // check election exists
            Election election = db.Elections.Find(id);
            if (election == null)
            {
                return HttpNotFound();
            }
            
            // check it's after voting phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.VotingEndDate)
            {
                return RedirectToAction("Index", "ElectMember");
            }
            
            // get list of committee nominees
            ElecInfo elecInfo = ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).elect_getCommitteeNominees(election);

            // make accessible to view
            ViewData["elecInfo"] = elecInfo;

            return View();
        }

        public ActionResult Elect(int electionid = 0, int seatid = 0, int nomineeid = 0)
        {
            // check that election exists
            Election election = db.Elections.Find(electionid);
            if (election == null)
            {
                return HttpNotFound();
            }

            // check that committee exists
            CommitteeSeat seat = db.CommitteeSeats.Find(seatid);
            if (seat == null)
            {
                return HttpNotFound();
            }

            // check that nominee exists
            Faculty nominee = db.ElectionNominations.Where(n => n.ElectionId == electionid && n.CommitteeSeatId == seatid && n.FacultyId == nomineeid).FirstOrDefault().Faculty;
            if (nominee == null)
            {
                return HttpNotFound();
            }

            // check that it's after voting phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.VotingEndDate)
            {
                return RedirectToAction("Index", "ElectMember");
            }

            // do the electing
            ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).elect_elect(nominee, seat, election, null);

            // redirect to details page
            return RedirectToAction("Details", "ElectMember", new { id = electionid });
        }

        public ActionResult Unelect(int electionid = 0, int seatid = 0, int nomineeid = 0)
        {
            // check that election exists
            Election election = db.Elections.Find(electionid);
            if (election == null)
            {
                return HttpNotFound();
            }

            // check that committee exists
            CommitteeSeat seat = db.CommitteeSeats.Find(seatid);
            if (seat == null)
            {
                return HttpNotFound();
            }

            // check that nominee exists
            Faculty nominee = db.ElectionNominations.Where(n => n.ElectionId == electionid && n.CommitteeSeatId == seatid && n.FacultyId == nomineeid).FirstOrDefault().Faculty;
            if (nominee == null)
            {
                return HttpNotFound();
            }

            // check that it's after voting phase
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            if (now < election.VotingEndDate)
            {
                return RedirectToAction("Index", "ElectMember");
            }

            // do the unelecting
            ElectionTypeHelperFactory.Instance.getElecTypeEligChecker(election).elect_unelect(nominee, seat, election);

            // redirect to details page
            return RedirectToAction("Details", "ElectMember", new { id = electionid });
        }

    }

}
