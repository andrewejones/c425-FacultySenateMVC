using System.Web.Mvc;

namespace FacultySenateMVC.Areas.User
{
    public class UserAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "User";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "User_nominate",
                "User/Nomination/Nominate/{electionid}/{committeeid}",
                new { controller = "Nomination", action = "Nominate" },
                new[] { "FacultySenateMVC.Areas.User.Controllers" }
            );

            context.MapRoute(
                "User_unnominate",
                "User/Nomination/Unnominate/{electionid}/{committeeid}",
                new { controller = "Nomination", action = "Unnominate" },
                new[] { "FacultySenateMVC.Areas.User.Controllers" }
            );

            context.MapRoute(
                "User_nominees",
                "User/Nomination/Nominees/{electionid}/{committeeid}",
                new { controller = "Nomination", action = "Nominees" },
                new[] { "FacultySenateMVC.Areas.User.Controllers" }
            );

            context.MapRoute(
                "User_reasons",
                "User/Nomination/Reasons/{electionid}/{committeeid}",
                new { controller = "Nomination", action = "Reasons" },
                new[] { "FacultySenateMVC.Areas.User.Controllers" }
            );

            context.MapRoute(
                "User_vote",
                "User/Voting/Vote/{electionid}/{committeeid}/{nomineeid}",
                new { controller = "Voting", action = "Vote" },
                new[] { "FacultySenateMVC.Areas.User.Controllers" }
            );

            context.MapRoute(
                "User_unvote",
                "User/Voting/Unvote/{electionid}/{committeeid}/{nomineeid}",
                new { controller = "Voting", action = "Unvote" },
                new[] { "FacultySenateMVC.Areas.User.Controllers" }
            );

            context.MapRoute(
                "User_voting_nominees",
                "User/Voting/Nominees/{electionid}",
                new { controller = "Voting", action = "Nominees" },
                new[] { "FacultySenateMVC.Areas.User.Controllers" }
            );

            context.MapRoute(
                "User_default",
                "User/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "FacultySenateMVC.Areas.User.Controllers" }
            );
        }
    }
}
