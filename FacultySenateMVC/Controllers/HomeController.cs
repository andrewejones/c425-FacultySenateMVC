using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Models;

namespace FacultySenateMVC.Controllers
{
    public class HomeController : Controller
    {
        private FacultySenateDBEntities db = new FacultySenateDBEntities();

        public ActionResult Index()
        {
            // maybe we should redirect to proper Area if they are logged in instead of login page no matter what...?
            return RedirectToAction("Login", "Home"); // redirect to login page
        }

        public ActionResult Login()
        {
            // clear session if user is attempting to login as someone else...
            // maybe we should redirect to proper Area if they are logged in instead of going to login page and clearing session...?
            Session.Clear(); // delete session vars
            Session.Abandon(); // delete session vars
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            // THIS SECTION WILL CHANGE when the actual login script is used, as will AuthorizeUser

            // get username from form
            String username = (String) form["username"];
            String password = (String) form["password"];

            // look for user in DB
            Faculty user = db.Faculties.Where(f => f.Email == username).FirstOrDefault();

            // check creds
            if (password == "fse123")
            {
                if (username == "admin") // user is admin
                {
                    Session["uname"] = "admin";
                    Session.Remove("user"); // clear user data if it existed
                    return RedirectToAction("Index", "Home", new { Area = "Admin" }); // redirect to admin page
                }
                else if (user != null) // user is faculty
                {
                    Session["uname"] = user.Email.ToLower();
                    Session["user"] = user; // store user object for later
                    return RedirectToAction("Index", "Home", new { Area = "User" }); // redirect to account page
                }
            }

            return RedirectToAction("Login", "Home"); // redirect to login page
        }

        public ActionResult Logout()
        {
            Session.Clear(); // delete session vars
            Session.Abandon(); // delete session vars
            return RedirectToAction("Login", "Home"); // redirect to Login
        }

    }
}
