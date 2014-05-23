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
    public class FacultyDisciplineController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/FacultyDiscipline/

        public ActionResult Index()
        {
            var facultydisciplines = db.FacultyDisciplines.Include(f => f.FacultySchool)
                .OrderBy(f => f.FacultySchool.Name).ThenBy(f => f.Name);
            return View(facultydisciplines.ToList());
        }

        //
        // GET: /Admin/FacultyDiscipline/Details/5

        public ActionResult Details(int id = 0)
        {
            FacultyDiscipline facultydiscipline = db.FacultyDisciplines.Find(id);
            if (facultydiscipline == null)
            {
                return HttpNotFound();
            }
            return View(facultydiscipline);
        }

        //
        // GET: /Admin/FacultyDiscipline/Create

        public ActionResult Create()
        {
            ViewBag.FacultySchoolId = new SelectList(db.FacultySchools, "Id", "Name");
            return View();
        }

        //
        // POST: /Admin/FacultyDiscipline/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FacultyDiscipline facultydiscipline)
        {
            if (ModelState.IsValid)
            {
                db.FacultyDisciplines.Add(facultydiscipline);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FacultySchoolId = new SelectList(db.FacultySchools, "Id", "Name", facultydiscipline.FacultySchoolId);
            return View(facultydiscipline);
        }

        //
        // GET: /Admin/FacultyDiscipline/Edit/5

        public ActionResult Edit(int id = 0)
        {
            FacultyDiscipline facultydiscipline = db.FacultyDisciplines.Find(id);
            if (facultydiscipline == null)
            {
                return HttpNotFound();
            }
            ViewBag.FacultySchoolId = new SelectList(db.FacultySchools, "Id", "Name", facultydiscipline.FacultySchoolId);
            return View(facultydiscipline);
        }

        //
        // POST: /Admin/FacultyDiscipline/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FacultyDiscipline facultydiscipline)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facultydiscipline).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FacultySchoolId = new SelectList(db.FacultySchools, "Id", "Name", facultydiscipline.FacultySchoolId);
            return View(facultydiscipline);
        }

        //
        // GET: /Admin/FacultyDiscipline/Delete/5

        public ActionResult Delete(int id = 0)
        {
            FacultyDiscipline facultydiscipline = db.FacultyDisciplines.Find(id);
            if (facultydiscipline == null)
            {
                return HttpNotFound();
            }
            return View(facultydiscipline);
        }

        //
        // POST: /Admin/FacultyDiscipline/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FacultyDiscipline facultydiscipline = db.FacultyDisciplines.Find(id);
            db.FacultyDisciplines.Remove(facultydiscipline);
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