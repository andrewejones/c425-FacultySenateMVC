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
    public class CommitteeTypeController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/CommitteeType/

        public ActionResult Index()
        {
            return View(db.CommitteeTypes.ToList());
        }

        //
        // GET: /Admin/CommitteeType/Details/5

        public ActionResult Details(int id = 0)
        {
            CommitteeType committeetype = db.CommitteeTypes.Find(id);
            if (committeetype == null)
            {
                return HttpNotFound();
            }
            return View(committeetype);
        }

        //
        // GET: /Admin/CommitteeType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/CommitteeType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommitteeType committeetype)
        {
            if (ModelState.IsValid)
            {
                db.CommitteeTypes.Add(committeetype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(committeetype);
        }

        //
        // GET: /Admin/CommitteeType/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CommitteeType committeetype = db.CommitteeTypes.Find(id);
            if (committeetype == null)
            {
                return HttpNotFound();
            }
            return View(committeetype);
        }

        //
        // POST: /Admin/CommitteeType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CommitteeType committeetype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(committeetype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(committeetype);
        }

        //
        // GET: /Admin/CommitteeType/Delete/5

        public ActionResult Delete(int id = 0)
        {
            CommitteeType committeetype = db.CommitteeTypes.Find(id);
            if (committeetype == null)
            {
                return HttpNotFound();
            }
            return View(committeetype);
        }

        //
        // POST: /Admin/CommitteeType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommitteeType committeetype = db.CommitteeTypes.Find(id);
            db.CommitteeTypes.Remove(committeetype);
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