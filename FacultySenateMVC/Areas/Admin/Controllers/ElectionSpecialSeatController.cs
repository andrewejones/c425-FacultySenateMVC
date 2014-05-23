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
    public class ElectionSpecialSeatController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/ElectionSpecialSeat/

        public ActionResult Index()
        {
            var electionspecialseats = db.ElectionSpecialSeats.Include(e => e.CommitteeSeat).Include(e => e.Election);
            return View(electionspecialseats.ToList());
        }

        //
        // GET: /Admin/ElectionSpecialSeat/Details/5

        public ActionResult Details(int id = 0)
        {
            ElectionSpecialSeat electionspecialseat = db.ElectionSpecialSeats.Find(id);
            if (electionspecialseat == null)
            {
                return HttpNotFound();
            }
            return View(electionspecialseat);
        }

        //
        // GET: /Admin/ElectionSpecialSeat/Create

        public ActionResult Create()
        {
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id");
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id");
            return View();
        }

        //
        // POST: /Admin/ElectionSpecialSeat/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ElectionSpecialSeat electionspecialseat)
        {
            if (ModelState.IsValid)
            {
                db.ElectionSpecialSeats.Add(electionspecialseat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id", electionspecialseat.CommitteeSeatId);
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id", electionspecialseat.ElectionId);
            return View(electionspecialseat);
        }

        //
        // GET: /Admin/ElectionSpecialSeat/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ElectionSpecialSeat electionspecialseat = db.ElectionSpecialSeats.Find(id);
            if (electionspecialseat == null)
            {
                return HttpNotFound();
            }
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id", electionspecialseat.CommitteeSeatId);
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id", electionspecialseat.ElectionId);
            return View(electionspecialseat);
        }

        //
        // POST: /Admin/ElectionSpecialSeat/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ElectionSpecialSeat electionspecialseat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(electionspecialseat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CommitteeSeatId = new SelectList(db.CommitteeSeats, "Id", "Id", electionspecialseat.CommitteeSeatId);
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id", electionspecialseat.ElectionId);
            return View(electionspecialseat);
        }

        //
        // GET: /Admin/ElectionSpecialSeat/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ElectionSpecialSeat electionspecialseat = db.ElectionSpecialSeats.Find(id);
            if (electionspecialseat == null)
            {
                return HttpNotFound();
            }
            return View(electionspecialseat);
        }

        //
        // POST: /Admin/ElectionSpecialSeat/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ElectionSpecialSeat electionspecialseat = db.ElectionSpecialSeats.Find(id);
            db.ElectionSpecialSeats.Remove(electionspecialseat);
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