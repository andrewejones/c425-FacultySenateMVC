using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacultySenateMVC.Areas.MembershipAndElections.Controllers
{
    public class CreateElectionController : MembershipAndElectionsController
    {

        public ActionResult Index()
        {
            return View();
        }

    }
}
