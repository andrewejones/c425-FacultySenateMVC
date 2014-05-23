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
    public class CommitteeController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/Committee/

        public ActionResult Index()
        {
            var committees = db.Committees.Include(c => c.CommitteeType);
            return View(committees.ToList());
        }

        //
        // GET: /Admin/Committee/Details/5

        public ActionResult Details(int id = 0)
        {
            Committee committee = db.Committees.Find(id);
            if (committee == null)
            {
                return HttpNotFound();
            }
            return View(committee);
        }

        //
        // GET: /Admin/Committee/Create

        public ActionResult Create()
        {
            ViewBag.CommitteeTypeId = new SelectList(db.CommitteeTypes, "Id", "Name");
            return View();
        }

        //
        // POST: /Admin/Committee/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Committee committee)
        {
            if (ModelState.IsValid)
            {
                db.Committees.Add(committee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CommitteeTypeId = new SelectList(db.CommitteeTypes, "Id", "Name", committee.CommitteeTypeId);
            return View(committee);
        }

        //
        // GET: /Admin/Committee/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Committee committee = db.Committees.Find(id);
            if (committee == null)
            {
                return HttpNotFound();
            }
            ViewBag.CommitteeTypeId = new SelectList(db.CommitteeTypes, "Id", "Name", committee.CommitteeTypeId);
            return View(committee);
        }

        //
        // POST: /Admin/Committee/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Committee committee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(committee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CommitteeTypeId = new SelectList(db.CommitteeTypes, "Id", "Name", committee.CommitteeTypeId);
            return View(committee);
        }

        //
        // GET: /Admin/Committee/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Committee committee = db.Committees.Find(id);
            if (committee == null)
            {
                return HttpNotFound();
            }
            return View(committee);
        }

        //
        // POST: /Admin/Committee/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Committee committee = db.Committees.Find(id);
            db.Committees.Remove(committee);
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