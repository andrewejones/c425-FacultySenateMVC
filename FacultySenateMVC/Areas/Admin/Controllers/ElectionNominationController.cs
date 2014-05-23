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
    public class ElectionNominationController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/ElectionNomination/

        public ActionResult Index()
        {
            var electionnominations = db.ElectionNominations.Include(e => e.CommitteeSeat).Include(e => e.Election).Include(e => e.Faculty)
                .OrderBy(e => e.ElectionId).ThenBy(e => e.Faculty.Last).ThenBy(e => e.Faculty.First);
            return View(electionnominations.ToList());
        }

        //
        // GET: /Admin/ElectionNomination/Details/5

        public ActionResult Details(int id = 0)
        {
            ElectionNomination electionnomination = db.ElectionNominations.Find(id);
            if (electionnomination == null)
            {
                return HttpNotFound();
            }
            return View(electionnomination);
        }

        //
        // GET: /Admin/ElectionNomination/Create

        public ActionResult Create()
        {
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id");
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id");
            //ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "First");
            ViewBag.FacultyId = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName");

            return View();
        }

        //
        // POST: /Admin/ElectionNomination/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ElectionNomination electionnomination)
        {
            if (ModelState.IsValid)
            {
                db.ElectionNominations.Add(electionnomination);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id", electionnomination.CommitteeSeatId);
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id", electionnomination.ElectionId);
            //ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "First", electionnomination.FacultyId);
            ViewBag.FacultyId = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName");
            
            return View(electionnomination);
        }

        //
        // GET: /Admin/ElectionNomination/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ElectionNomination electionnomination = db.ElectionNominations.Find(id);
            if (electionnomination == null)
            {
                return HttpNotFound();
            }
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id", electionnomination.CommitteeSeatId);
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id", electionnomination.ElectionId);
            //ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "First", electionnomination.FacultyId);
            ViewBag.FacultyId = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", electionnomination.FacultyId);
            
            return View(electionnomination);
        }

        //
        // POST: /Admin/ElectionNomination/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ElectionNomination electionnomination)
        {
            if (ModelState.IsValid)
            {
                db.Entry(electionnomination).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id", electionnomination.CommitteeSeatId);
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id", electionnomination.ElectionId);
            //ViewBag.FacultyId = new SelectList(db.Faculties, "Id", "First", electionnomination.FacultyId);
            ViewBag.FacultyId = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", electionnomination.FacultyId);
            
            return View(electionnomination);
        }

        //
        // GET: /Admin/ElectionNomination/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ElectionNomination electionnomination = db.ElectionNominations.Find(id);
            if (electionnomination == null)
            {
                return HttpNotFound();
            }
            return View(electionnomination);
        }

        //
        // POST: /Admin/ElectionNomination/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ElectionNomination electionnomination = db.ElectionNominations.Find(id);
            db.ElectionNominations.Remove(electionnomination);
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