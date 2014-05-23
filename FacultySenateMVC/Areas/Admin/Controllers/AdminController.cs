using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Helpers;

namespace FacultySenateMVC.Areas.Admin.Controllers
{
    [AuthorizeUser(Permission = "admin")]
    public abstract class AdminController : Controller { }
}
