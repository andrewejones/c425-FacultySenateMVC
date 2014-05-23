using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Models;

namespace FacultySenateMVC.Areas.Admin.Controllers
{
    public class CommitteeSeatController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/CommitteeSeat/

        public ActionResult Index()
        {
            var committeeseats = db.CommitteeSeats.Include(c => c.Committee).Include(c => c.FacultySchool)
                .OrderBy(s => s.Committee.Name).ThenByDescending(s => s.Active).ThenBy(s => s.FacultySchoolId ?? Int16.MaxValue).ThenBy(s => s.StaggerYear ?? Int16.MaxValue);
            return View(committeeseats.ToList());
        }

        //
        // GET: /Admin/CommitteeSeat/Details/5

        public ActionResult Details(int id = 0)
        {
            CommitteeSeat committeeseat = db.CommitteeSeats.Find(id);
            if (committeeseat == null)
            {
                return HttpNotFound();
            }
            return View(committeeseat);
        }

        //
        // GET: /Admin/CommitteeSeat/Create

        public ActionResult Create()
        {
            ViewBag.CommitteeId = new SelectList(db.Committees.OrderBy(c => c.Name), "Id", "Name");
            ViewBag.FacultySchoolId = new SelectList(db.FacultySchools, "Id", "Abbreviation");
            return View();
        }

        //
        // POST: /Admin/CommitteeSeat/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommitteeSeat committeeseat)
        {
            if (ModelState.IsValid)
            {
                db.CommitteeSeats.Add(committeeseat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CommitteeId = new SelectList(db.Committees.OrderBy(c => c.Name), "Id", "Name", committeeseat.CommitteeId);
            ViewBag.FacultySchoolId = new SelectList(db.FacultySchools, "Id", "Abbreviation", committeeseat.FacultySchoolId);
            return View(committeeseat);
        }

        //
        // GET: /Admin/CommitteeSeat/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CommitteeSeat committeeseat = db.CommitteeSeats.Find(id);
            if (committeeseat == null)
            {
                return HttpNotFound();
            }
            ViewBag.CommitteeId = new SelectList(db.Committees.OrderBy(c => c.Name), "Id", "Name", committeeseat.CommitteeId);
            ViewBag.FacultySchoolId = new SelectList(db.FacultySchools, "Id", "Abbreviation", committeeseat.FacultySchoolId);
            return View(committeeseat);
        }

        //
        // POST: /Admin/CommitteeSeat/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CommitteeSeat committeeseat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(committeeseat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CommitteeId = new SelectList(db.Committees.OrderBy(c => c.Name), "Id", "Name", committeeseat.CommitteeId);
            ViewBag.FacultySchoolId = new SelectList(db.FacultySchools, "Id", "Abbreviation", committeeseat.FacultySchoolId);
            return View(committeeseat);
        }

        //
        // GET: /Admin/CommitteeSeat/Delete/5

        public ActionResult Delete(int id = 0)
        {
            CommitteeSeat committeeseat = db.CommitteeSeats.Find(id);
            if (committeeseat == null)
            {
                return HttpNotFound();
            }
            return View(committeeseat);
        }

        //
        // POST: /Admin/CommitteeSeat/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommitteeSeat committeeseat = db.CommitteeSeats.Find(id);
            db.CommitteeSeats.Remove(committeeseat);
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