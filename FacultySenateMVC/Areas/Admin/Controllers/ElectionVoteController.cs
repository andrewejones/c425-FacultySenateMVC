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
    public class ElectionVoteController : AdminController
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        //
        // GET: /Admin/ElectionVote/

        public ActionResult Index()
        {
            var electionvotes = db.ElectionVotes.Include(e => e.Committee).Include(e => e.Election).Include(e => e.Faculty_Nominee).Include(e => e.Faculty_Voter)
                .OrderBy(e => e.ElectionId).ThenBy(e => e.Committee.Name).ThenBy(e => e.Faculty_Nominee.Last).ThenBy(e => e.Faculty_Nominee.First);
            return View(electionvotes.ToList());
        }

        //
        // GET: /Admin/ElectionVote/Details/5

        public ActionResult Details(int id = 0)
        {
            ElectionVote electionvote = db.ElectionVotes.Find(id);
            if (electionvote == null)
            {
                return HttpNotFound();
            }
            return View(electionvote);
        }

        //
        // GET: /Admin/ElectionVote/Create

        public ActionResult Create()
        {
            ViewBag.CommitteeId = new SelectList(db.Committees.OrderBy(c => c.Name), "Id", "Name");
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id");
            //ViewBag.FacultyId_Nominee = new SelectList(db.Faculties, "Id", "First");
            ViewBag.FacultyId_Nominee = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName");
            //ViewBag.FacultyId_Voter = new SelectList(db.Faculties, "Id", "First");
            ViewBag.FacultyId_Voter = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName");

            return View();
        }

        //
        // POST: /Admin/ElectionVote/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ElectionVote electionvote)
        {
            if (ModelState.IsValid)
            {
                db.ElectionVotes.Add(electionvote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CommitteeId = new SelectList(db.Committees.OrderBy(c => c.Name), "Id", "Name", electionvote.CommitteeId);
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id", electionvote.ElectionId);
            //ViewBag.FacultyId_Nominee = new SelectList(db.Faculties, "Id", "First", electionvote.FacultyId_Nominee);
            ViewBag.FacultyId_Nominee = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", electionvote.FacultyId_Nominee);
           // ViewBag.FacultyId_Voter = new SelectList(db.Faculties, "Id", "First", electionvote.FacultyId_Voter);
            ViewBag.FacultyId_Voter = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", electionvote.FacultyId_Voter);

            return View(electionvote);
        }

        //
        // GET: /Admin/ElectionVote/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ElectionVote electionvote = db.ElectionVotes.Find(id);
            if (electionvote == null)
            {
                return HttpNotFound();
            }
            ViewBag.CommitteeId = new SelectList(db.Committees, "Id", "Name", electionvote.CommitteeId);
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id", electionvote.ElectionId);
            //ViewBag.FacultyId_Nominee = new SelectList(db.Faculties, "Id", "First", electionvote.FacultyId_Nominee);
            ViewBag.FacultyId_Nominee = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", electionvote.FacultyId_Nominee);
            // ViewBag.FacultyId_Voter = new SelectList(db.Faculties, "Id", "First", electionvote.FacultyId_Voter);
            ViewBag.FacultyId_Voter = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", electionvote.FacultyId_Voter);

            return View(electionvote);
        }

        //
        // POST: /Admin/ElectionVote/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ElectionVote electionvote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(electionvote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CommitteeId = new SelectList(db.Committees.OrderBy(c => c.Name), "Id", "Name", electionvote.CommitteeId);
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Id", electionvote.ElectionId);
            //ViewBag.FacultyId_Nominee = new SelectList(db.Faculties, "Id", "First", electionvote.FacultyId_Nominee);
            ViewBag.FacultyId_Nominee = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", electionvote.FacultyId_Nominee);
            // ViewBag.FacultyId_Voter = new SelectList(db.Faculties, "Id", "First", electionvote.FacultyId_Voter);
            ViewBag.FacultyId_Voter = new SelectList(db.Faculties.OrderBy(f => f.Last).ThenBy(f => f.First).Select(f => new { Id = f.Id, FullName = f.Last + ", " + f.First }), "Id", "FullName", electionvote.FacultyId_Voter);

            return View(electionvote);
        }

        //
        // GET: /Admin/ElectionVote/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ElectionVote electionvote = db.ElectionVotes.Find(id);
            if (electionvote == null)
            {
                return HttpNotFound();
            }
            return View(electionvote);
        }

        //
        // POST: /Admin/ElectionVote/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ElectionVote electionvote = db.ElectionVotes.Find(id);
            db.ElectionVotes.Remove(electionvote);
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