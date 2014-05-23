using System.Web.Mvc;

namespace FacultySenateMVC.Areas.MembershipAndElections
{
    public class MembershipAndElectionsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MembershipAndElections";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                "MembershipAndElections_elect",
                "MembershipAndElections/ElectMember/Elect/{electionid}/{seatid}/{nomineeid}",
                new { controller = "ElectMember", action = "Elect" },
                new[] { "FacultySenateMVC.Areas.MembershipAndElections.Controllers" }
            );

            context.MapRoute(
                "MembershipAndElections_unelect",
                "MembershipAndElections/ElectMember/Unelect/{electionid}/{seatid}/{nomineeid}",
                new { controller = "ElectMember", action = "Unelect" },
                new[] { "FacultySenateMVC.Areas.MembershipAndElections.Controllers" }
            );

            context.MapRoute(
                "MembershipAndElections_default",
                "MembershipAndElections/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
