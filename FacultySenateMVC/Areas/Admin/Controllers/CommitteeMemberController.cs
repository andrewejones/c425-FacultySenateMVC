using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Models;
using System.Data.Objects.SqlClient;

namespace FacultySenateMVC.Areas.Admin.Controllers
{
    public class CommitteeMemberController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/CommitteeMember/

        public ActionResult Index()
        {
            var committeemembers = db.CommitteeMembers.Include(c => c.CommitteeSeat).Include(c => c.Faculty)
                .OrderBy(c => c.CommitteeSeat.Committee.Name).ThenBy(c => c.CommitteeSeat.FacultySchoolId ?? Int16.MaxValue).ThenBy(c => c.CommitteeSeatId).ThenBy(c => c.StartDate);
            return View(committeemembers.ToList());
        }

        //
        // GET: /Admin/CommitteeMember/Details/5

        public ActionResult Details(int id = 0)
        {
            CommitteeMember committeemember = db.CommitteeMembers.Find(id);
            if (committeemember == null)
            {
                return HttpNotFound();
            }
            return View(committeemember);
        }

        //
        // GET: /Admin/CommitteeMember/Create

        public ActionResult Create()
        {
            //ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id");
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats.OrderBy(s => s.Committee.Name).ThenBy(s => s.FacultySchoolId ?? Int16.MaxValue).ThenBy(s => s.Id).Select(s => new { Id = s.Id, CommSeat = s.Committee.Name + "  -  " + (s.FacultySchool.Abbreviation ?? "At-Large") + "  -  " + SqlFunctions.StringConvert((double)(s.StaggerYear ?? 0)).Trim() }), "Id", "CommSeat");
            //ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "First");
            ViewBag.FacultyId = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName");

            return View();
        }

        //
        // POST: /Admin/CommitteeMember/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommitteeMember committeemember)
        {
            if (ModelState.IsValid)
            {
                db.CommitteeMembers.Add(committeemember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id", committeemember.CommitteeSeatId);
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats.OrderBy(s => s.Committee.Name).ThenBy(s => s.FacultySchoolId ?? Int16.MaxValue).ThenBy(s => s.Id).Select(s => new { Id = s.Id, CommSeat = s.Committee.Name + "  -  " + (s.FacultySchool.Abbreviation ?? "At-Large") + "  -  " + SqlFunctions.StringConvert((double)(s.StaggerYear ?? 0)).Trim() }), "Id", "CommSeat");
            //ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "First", committeemember.FacultyId);
            ViewBag.FacultyId = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName");

            return View(committeemember);
        }

        //
        // GET: /Admin/CommitteeMember/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CommitteeMember committeemember = db.CommitteeMembers.Find(id);
            if (committeemember == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id", committeemember.CommitteeSeatId);
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats.OrderBy(s => s.Committee.Name).ThenBy(s => s.FacultySchoolId ?? Int16.MaxValue).ThenBy(s => s.Id).Select(s => new { Id = s.Id, CommSeat = s.Committee.Name + "  -  " + (s.FacultySchool.Abbreviation ?? "At-Large") + "  -  " + SqlFunctions.StringConvert((double)(s.StaggerYear ?? 0)).Trim() }), "Id", "CommSeat", committeemember.CommitteeSeatId);
            //ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "First", committeemember.FacultyId);
            ViewBag.FacultyId = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", committeemember.FacultyId);

            return View(committeemember);
        }

        //
        // POST: /Admin/CommitteeMember/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CommitteeMember committeemember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(committeemember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id", committeemember.CommitteeSeatId);
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats.OrderBy(s => s.Committee.Name).ThenBy(s => s.FacultySchoolId ?? Int16.MaxValue).ThenBy(s => s.Id).Select(s => new { Id = s.Id, CommSeat = s.Committee.Name + "  -  " + (s.FacultySchool.Abbreviation ?? "At-Large") + "  -  " + SqlFunctions.StringConvert((double)(s.StaggerYear ?? 0)).Trim() }), "Id", "CommSeat", committeemember.CommitteeSeatId);
            //ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "First", committeemember.FacultyId);
            ViewBag.FacultyId = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", committeemember.FacultyId);

            return View(committeemember);
        }

        //
        // GET: /Admin/CommitteeMember/Delete/5

        public ActionResult Delete(int id = 0)
        {
            CommitteeMember committeemember = db.CommitteeMembers.Find(id);
            if (committeemember == null)
            {
                return HttpNotFound();
            }
            return View(committeemember);
        }

        //
        // POST: /Admin/CommitteeMember/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommitteeMember committeemember = db.CommitteeMembers.Find(id);
            db.CommitteeMembers.Remove(committeemember);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}