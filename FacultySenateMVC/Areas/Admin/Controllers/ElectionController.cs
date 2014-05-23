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
    public class ElectionController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/Election/

        public ActionResult Index()
        {
            var elections = db.Elections.Include(e => e.ElectionType);
            return View(elections.ToList());
        }

        //
        // GET: /Admin/Election/Details/5

        public ActionResult Details(int id = 0)
        {
            Election election = db.Elections.Find(id);
            if (election == null)
            {
                return HttpNotFound();
            }
            return View(election);
        }

        //
        // GET: /Admin/Election/Create

        public ActionResult Create()
        {
            ViewBag.ElectionTypeId = new SelectList(db.ElectionTypes, "Id", "Name");
            return View();
        }

        //
        // POST: /Admin/Election/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Election election)
        {
            if (ModelState.IsValid)
            {
                db.Elections.Add(election);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ElectionTypeId = new SelectList(db.ElectionTypes, "Id", "Name", election.ElectionTypeId);
            return View(election);
        }

        //
        // GET: /Admin/Election/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Election election = db.Elections.Find(id);
            if (election == null)
            {
                return HttpNotFound();
            }
            ViewBag.ElectionTypeId = new SelectList(db.ElectionTypes, "Id", "Name", election.ElectionTypeId);
            return View(election);
        }

        //
        // POST: /Admin/Election/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Election election)
        {
            if (ModelState.IsValid)
            {
                db.Entry(election).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ElectionTypeId = new SelectList(db.ElectionTypes, "Id", "Name", election.ElectionTypeId);
            return View(election);
        }

        //
        // GET: /Admin/Election/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Election election = db.Elections.Find(id);
            if (election == null)
            {
                return HttpNotFound();
            }
            return View(election);
        }

        //
        // POST: /Admin/Election/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Election election = db.Elections.Find(id);
            db.Elections.Remove(election);
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