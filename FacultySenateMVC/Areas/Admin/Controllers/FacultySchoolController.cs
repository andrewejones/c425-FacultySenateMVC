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
    public class FacultySchoolController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/FacultySchool/

        public ActionResult Index()
        {
            return View(db.FacultySchools.ToList());
        }

        //
        // GET: /Admin/FacultySchool/Details/5

        public ActionResult Details(int id = 0)
        {
            FacultySchool facultyschool = db.FacultySchools.Find(id);
            if (facultyschool == null)
            {
                return HttpNotFound();
            }
            return View(facultyschool);
        }

        //
        // GET: /Admin/FacultySchool/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/FacultySchool/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FacultySchool facultyschool)
        {
            if (ModelState.IsValid)
            {
                db.FacultySchools.Add(facultyschool);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(facultyschool);
        }

        //
        // GET: /Admin/FacultySchool/Edit/5

        public ActionResult Edit(int id = 0)
        {
            FacultySchool facultyschool = db.FacultySchools.Find(id);
            if (facultyschool == null)
            {
                return HttpNotFound();
            }
            return View(facultyschool);
        }

        //
        // POST: /Admin/FacultySchool/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FacultySchool facultyschool)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facultyschool).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(facultyschool);
        }

        //
        // GET: /Admin/FacultySchool/Delete/5

        public ActionResult Delete(int id = 0)
        {
            FacultySchool facultyschool = db.FacultySchools.Find(id);
            if (facultyschool == null)
            {
                return HttpNotFound();
            }
            return View(facultyschool);
        }

        //
        // POST: /Admin/FacultySchool/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FacultySchool facultyschool = db.FacultySchools.Find(id);
            db.FacultySchools.Remove(facultyschool);
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