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
    public class FacultyRankController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/FacultyRank/

        public ActionResult Index()
        {
            return View(db.FacultyRanks.ToList());
        }

        //
        // GET: /Admin/FacultyRank/Details/5

        public ActionResult Details(int id = 0)
        {
            FacultyRank facultyrank = db.FacultyRanks.Find(id);
            if (facultyrank == null)
            {
                return HttpNotFound();
            }
            return View(facultyrank);
        }

        //
        // GET: /Admin/FacultyRank/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/FacultyRank/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FacultyRank facultyrank)
        {
            if (ModelState.IsValid)
            {
                db.FacultyRanks.Add(facultyrank);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(facultyrank);
        }

        //
        // GET: /Admin/FacultyRank/Edit/5

        public ActionResult Edit(int id = 0)
        {
            FacultyRank facultyrank = db.FacultyRanks.Find(id);
            if (facultyrank == null)
            {
                return HttpNotFound();
            }
            return View(facultyrank);
        }

        //
        // POST: /Admin/FacultyRank/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FacultyRank facultyrank)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facultyrank).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(facultyrank);
        }

        //
        // GET: /Admin/FacultyRank/Delete/5

        public ActionResult Delete(int id = 0)
        {
            FacultyRank facultyrank = db.FacultyRanks.Find(id);
            if (facultyrank == null)
            {
                return HttpNotFound();
            }
            return View(facultyrank);
        }

        //
        // POST: /Admin/FacultyRank/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FacultyRank facultyrank = db.FacultyRanks.Find(id);
            db.FacultyRanks.Remove(facultyrank);
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