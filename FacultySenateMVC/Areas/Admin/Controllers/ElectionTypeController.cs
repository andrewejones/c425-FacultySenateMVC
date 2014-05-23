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
    public class ElectionTypeController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/ElectionType/

        public ActionResult Index()
        {
            return View(db.ElectionTypes.ToList());
        }

        //
        // GET: /Admin/ElectionType/Details/5

        public ActionResult Details(int id = 0)
        {
            ElectionType electiontype = db.ElectionTypes.Find(id);
            if (electiontype == null)
            {
                return HttpNotFound();
            }
            return View(electiontype);
        }

        //
        // GET: /Admin/ElectionType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/ElectionType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ElectionType electiontype)
        {
            if (ModelState.IsValid)
            {
                db.ElectionTypes.Add(electiontype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(electiontype);
        }

        //
        // GET: /Admin/ElectionType/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ElectionType electiontype = db.ElectionTypes.Find(id);
            if (electiontype == null)
            {
                return HttpNotFound();
            }
            return View(electiontype);
        }

        //
        // POST: /Admin/ElectionType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ElectionType electiontype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(electiontype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(electiontype);
        }

        //
        // GET: /Admin/ElectionType/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ElectionType electiontype = db.ElectionTypes.Find(id);
            if (electiontype == null)
            {
                return HttpNotFound();
            }
            return View(electiontype);
        }

        //
        // POST: /Admin/ElectionType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ElectionType electiontype = db.ElectionTypes.Find(id);
            db.ElectionTypes.Remove(electiontype);
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