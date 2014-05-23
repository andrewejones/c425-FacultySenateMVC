using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacultySenateMVC.Helpers
{
    public class AuthorizeUser : AuthorizeAttribute
    {
        public String Permission { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            Boolean isValid = false;

            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            String username = (String) httpContext.Session["uname"];

            if (username != null)
            {
                if (Permission.Equals("admin") && username.Equals("admin"))
                {
                    isValid = true;
                }
                else if (Permission.Equals("user") && !username.Equals("admin"))
                {
                    isValid = true;
                }
            }

            return /*base.AuthorizeCore(httpContext) &&*/ isValid;
        }

        // This should be implemented...
        /*protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);

            filterContext.Result = new RedirectResult(urlHelper.Action("Index", "Home"));
        }*/

    }
}