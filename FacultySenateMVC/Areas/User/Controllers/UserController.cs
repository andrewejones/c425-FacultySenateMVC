using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FacultySenateMVC.Helpers;

namespace FacultySenateMVC.Areas.User.Controllers
{
    [AuthorizeUser(Permission = "user")]
    public class UserController : Controller { }
}
