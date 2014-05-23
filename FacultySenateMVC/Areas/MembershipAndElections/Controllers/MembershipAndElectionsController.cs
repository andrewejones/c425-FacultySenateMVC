using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Helpers;

namespace FacultySenateMVC.Areas.MembershipAndElections.Controllers
{
    [AuthorizeUser(Permission = "admin")]
    public class MembershipAndElectionsController : Controller { }
}
