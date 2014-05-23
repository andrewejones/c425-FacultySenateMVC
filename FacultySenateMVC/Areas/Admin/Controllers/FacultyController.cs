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
    public class FacultyController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/Faculty/

        public ActionResult Index()
        {
            var faculties = db.Faculties.Include(f => f.FacultyDiscipline).Include(f => f.FacultyRank)
                .OrderByDescending(f => f.Active).ThenBy(f => f.Last).ThenBy(f => f.First);
            return View(faculties.ToList());
        }

        //
        // GET: /Admin/Faculty/Details/5

        public ActionResult Details(int id = 0)
        {
            Faculty faculty = db.Faculties.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            return View(faculty);
        }

        //
        // GET: /Admin/Faculty/Create

        public ActionResult Create()
        {
            ViewBag.FacultyDisciplineId = new SelectList(db.FacultyDisciplines.OrderBy(d => d.FacultySchool.Abbreviation).ThenBy(d => d.Name).Select(d => new { Id = d.Id, Description = d.FacultySchool.Abbreviation + " - " + d.Name }), "Id", "Description");
            ViewBag.FacultyRankId = new SelectList(db.FacultyRanks, "Id", "Name");
            return View();
        }

        //
        // POST: /Admin/Faculty/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                db.Faculties.Add(faculty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.FacultyDisciplineId = new SelectList(db.FacultyDisciplines, "Id", "Name", faculty.FacultyDisciplineId);
            ViewBag.FacultyDisciplineId = new SelectList(db.FacultyDisciplines.OrderBy(d => d.FacultySchool.Abbreviation).ThenBy(d => d.Name).Select(d => new { Id = d.Id, Description = d.FacultySchool.Abbreviation + " - " + d.Name }), "Id", "Description", faculty.FacultyDisciplineId);
            ViewBag.FacultyRankId = new SelectList(db.FacultyRanks, "Id", "Name", faculty.FacultyRankId);
            return View(faculty);
        }

        //
        // GET: /Admin/Faculty/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Faculty faculty = db.Faculties.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            //ViewBag.FacultyDisciplineId = new SelectList(db.FacultyDisciplines, "Id", "Name", faculty.FacultyDisciplineId);
            ViewBag.FacultyDisciplineId = new SelectList(db.FacultyDisciplines.OrderBy(d => d.FacultySchool.Abbreviation).ThenBy(d => d.Name).Select(d => new { Id = d.Id, Description = d.FacultySchool.Abbreviation + " - " + d.Name }), "Id", "Description", faculty.FacultyDisciplineId);
            ViewBag.FacultyRankId = new SelectList(db.FacultyRanks, "Id", "Name", faculty.FacultyRankId);
            return View(faculty);
        }

        //
        // POST: /Admin/Faculty/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(faculty).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.FacultyDisciplineId = new SelectList(db.FacultyDisciplines, "Id", "Name", faculty.FacultyDisciplineId);
            ViewBag.FacultyDisciplineId = new SelectList(db.FacultyDisciplines.OrderBy(d => d.FacultySchool.Abbreviation).ThenBy(d => d.Name).Select(d => new { Id = d.Id, Description = d.FacultySchool.Abbreviation + " - " + d.Name }), "Id", "Description", faculty.FacultyDisciplineId);
            ViewBag.FacultyRankId = new SelectList(db.FacultyRanks, "Id", "Name", faculty.FacultyRankId);
            return View(faculty);
        }

        //
        // GET: /Admin/Faculty/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Faculty faculty = db.Faculties.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            return View(faculty);
        }

        //
        // POST: /Admin/Faculty/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Faculty faculty = db.Faculties.Find(id);
            db.Faculties.Remove(faculty);
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