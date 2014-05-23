using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FacultySenateMVC.Models;

namespace FacultySenateMVC.Helpers
{

    public sealed class CommEligCheckerFactory
    {
        private static readonly CommEligCheckerFactory instance = new CommEligCheckerFactory();

        static CommEligCheckerFactory() { }
        private CommEligCheckerFactory() { }

        public static CommEligCheckerFactory Instance
        {
            get { return instance; }
        }

        public CommEligChecker getCommEligChecker(Committee committee)
        {
            // check for null parameter
            if (committee == null)
            {
                throw new ArgumentNullException("committee");
            }


            // Senate Committees
            if (committee.Name == "Senate")
            {
                return CommEligChecker_Senate.Instance;
            }
            else if (committee.Name == "Academic Policies Committee")
            {
                return CommEligChecker_AcadPol.Instance;
            }
            else if (committee.Name == "Admissions and Readmissions")
            {
                return CommEligChecker_Admiss.Instance;
            }
            else if (committee.Name == "Faculty Development Committee")
            {
                return CommEligChecker_FacDev.Instance;
            }
            else if (committee.Name == "Honors Program Committee")
            {
                return CommEligChecker_HonProg.Instance;
            }
            else if (committee.Name == "Learning with Technology Committee")
            {
                return CommEligChecker_LrnWTech.Instance;
            }
            else if (committee.Name == "International Education Committee")
            {
                return CommEligChecker_IntEd.Instance;
            }
            else if (committee.Name == "Library Committee")
            {
                return CommEligChecker_LibCom.Instance;
            }
            else if (committee.Name == "Long Range Academic Planning Committee")
            {
                return CommEligChecker_LngRng.Instance;
            }
            else if (committee.Name == "Membership and Elections Committee")
            {
                return CommEligChecker_MandE.Instance;
            }
            else if (committee.Name == "University Academic Assessment Committee")
            {
                return CommEligChecker_UniAcadAss.Instance;
            }
            else if (committee.Name == "Faculty Financial Affairs Committee")
            {
                return CommEligChecker_FacFinAff.Instance;
            }
            else if (committee.Name == "Honors Convocation")
            {
                return CommEligChecker_HonConv.Instance;
            }
            else if (committee.Name == "Academic Freedom and Tenure Committee")
            {
                return CommEligChecker_AcadFree.Instance;
            }
            else if (committee.Name == "Faculty Awards and Recognition Committee")
            {
                return CommEligChecker_FacAward.Instance;
            }
            else if (committee.Name == "Undergraduate Curriculum Committee")
            {
                return CommEligChecker_UndCurr.Instance;
            }
            else if (committee.Name == "Faculty Welfare Committee")
            {
                return CommEligChecker_FacWelf.Instance;
            }
            else if (committee.Name == "Promotions")
            {
                return CommEligChecker_Promo.Instance;
            }
            else if (committee.Name == "Faculty Retrenchment Committee")
            {
                return CommEligChecker_FacRetr.Instance;
            }
            else if (committee.Name == "Faculty Hearing Committee")
            {
                return CommEligChecker_FacHear.Instance;
            }
            else if (committee.Name == "Faculty Mediation Committee")
            {
                return CommEligChecker_FacMed.Instance;
            }
            else if (committee.Name == "Retrenchment Appeals Committee")
            {
                return CommEligChecker_RetrApp.Instance;
            }

            // Consortium Committees
            else if (committee.Name == "Consortium Coordinating Committee")
            {
                return CommEligChecker_ConsCoord.Instance;
            }
            else if (committee.Name == "Traffic, Safety, Buildings and Grounds Committee")
            {
                return CommEligChecker_TrafSaf.Instance;
            }
            else if (committee.Name == "Cultural Affairs Committee")
            {
                return CommEligChecker_CultAff.Instance;
            }
            else if (committee.Name == "Facilities Management Advisory Committee")
            {
                return CommEligChecker_FacMan.Instance;
            }
            else if (committee.Name == "Committee on Cultural Diversity")
            {
                return CommEligChecker_CultDiv.Instance;
            }
            else if (committee.Name == "Fiscal Advisory Committee")
            {
                return CommEligChecker_FiscAdv.Instance;
            }
            else if (committee.Name == "Government Relations Committee")
            {
                return CommEligChecker_GovRel.Instance;
            }
            else if (committee.Name == "The Committee on Wellness")
            {
                return CommEligChecker_Well.Instance;
            }
            else if (committee.Name == "Information Technology Committee")
            {
                return CommEligChecker_IT.Instance;
            }

            // "Other" Committees
            else if (committee.Name == "Maryland Higher Education Commission Faculty Advisory Committee")
            {
                return CommEligChecker_MHEC.Instance;
            }
            else if (committee.Name == "Council of University System Faculty")
            {
                return CommEligChecker_CouncUni.Instance;
            }
            else if (committee.Name == "Athletics")
            {
                return CommEligChecker_Athl.Instance;
            }
            else if (committee.Name == "Graduate Council")
            {
                return CommEligChecker_Grad.Instance;
            }
            else if (committee.Name == "USM Diversity Network's Faculty Initiatives Committee")
            {
                return CommEligChecker_USMDiv.Instance;
            }
            // error if we get this far
            else
            {
                throw new ArgumentException("Invalid committee; rules not yet defined", "committee");
            }
        }
    }

    public abstract class CommEligChecker
    {
        public abstract Boolean getEligibility(Faculty user, Election election);
        public abstract Boolean getEligibility(Faculty user, Election election, List<String> reasons);

        // return true if tenured
        protected Boolean tenureChecker(Faculty user)
        {
            return user.Tenure == true;
        }
        protected Boolean tenureChecker(Faculty user, List<String> reasons)
        {
            if (user.Tenure == true)
                return true;

            reasons.Add("Must be tenured");
            return false;
        }
        
        // return true if graduate
        protected Boolean graduateChecker(Faculty user)
        {
            return user.Graduate == true;
        }
        protected Boolean graduateChecker(Faculty user, List<String> reasons)
        {
            if (user.Graduate == true)
                return true;

            reasons.Add("Must be a graduate professor");
            return false;
        }

        // return true if NOT from library
        protected Boolean notLibraryChecker(Faculty user)
        {
            return user.FacultyDiscipline.FacultySchool.Name != "Library";
        }
        protected Boolean notLibraryChecker(Faculty user, List<String> reasons)
        {
            if (user.FacultyDiscipline.FacultySchool.Name != "Library")
                return true;

            reasons.Add("Library faculty may not serve");
            return false;
        }

        // return true if NOT already on Hearing, Mediation, Appeals or Retrenchment
        protected Boolean notHMARChecker(Faculty user, Election election)
        {
            FacultySenateDBEntities db = new FacultySenateDBEntities();

            if ((from mem in db.CommitteeMembers
                 where mem.Id == user.Id &&
                       election.TermStartDate < mem.EndDate &&
                       (mem.CommitteeSeat.Committee.Name == "Faculty Hearing Committee" ||
                       mem.CommitteeSeat.Committee.Name == "Faculty Mediation Committee" ||
                       mem.CommitteeSeat.Committee.Name == "Retrenchment Appeals Committee" ||
                       mem.CommitteeSeat.Committee.Name == "Faculty Retrenchment Committee")
                 select mem).Count() >= 1)
            {
                return false;
            }

            return true;
        }
        protected Boolean notHMARChecker(Faculty user, Election election, List<String> reasons)
        {
            FacultySenateDBEntities db = new FacultySenateDBEntities();

            if ((from mem in db.CommitteeMembers
                 where mem.Id == user.Id &&
                       election.TermStartDate < mem.EndDate &&
                       (mem.CommitteeSeat.Committee.Name == "Faculty Hearing Committee" ||
                       mem.CommitteeSeat.Committee.Name == "Faculty Mediation Committee" ||
                       mem.CommitteeSeat.Committee.Name == "Retrenchment Appeals Committee" ||
                       mem.CommitteeSeat.Committee.Name == "Faculty Retrenchment Committee")
                 select mem).Count() >= 1)
            {
                reasons.Add("May not serve concurrently on Hearing, Mediation, Retrenchment, and/or Appeals");
                return false;
            }

            return true;
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    // Senate Committees
    /// ///////////////////////////////////////////////////////////////////////

    // Senate
    public class CommEligChecker_Senate : CommEligChecker
    {
        private static readonly CommEligChecker_Senate instance = new CommEligChecker_Senate();

        static CommEligChecker_Senate() { }
        private CommEligChecker_Senate() { }
        public static CommEligChecker_Senate Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Academic Policies
    public class CommEligChecker_AcadPol : CommEligChecker
    {
        private static readonly CommEligChecker_AcadPol instance = new CommEligChecker_AcadPol();

        static CommEligChecker_AcadPol() { }
        private CommEligChecker_AcadPol() { }
        public static CommEligChecker_AcadPol Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Admissions and Readmissions
    public class CommEligChecker_Admiss : CommEligChecker
    {
        private static readonly CommEligChecker_Admiss instance = new CommEligChecker_Admiss();

        static CommEligChecker_Admiss() { }
        private CommEligChecker_Admiss() { }
        public static CommEligChecker_Admiss Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Faculty Development
    public class CommEligChecker_FacDev : CommEligChecker
    {
        private static readonly CommEligChecker_FacDev instance = new CommEligChecker_FacDev();

        static CommEligChecker_FacDev() { }
        private CommEligChecker_FacDev() { }
        public static CommEligChecker_FacDev Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Honors Program
    public class CommEligChecker_HonProg : CommEligChecker
    {
        private static readonly CommEligChecker_HonProg instance = new CommEligChecker_HonProg();

        static CommEligChecker_HonProg() { }
        private CommEligChecker_HonProg() { }
        public static CommEligChecker_HonProg Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Learning with Technology
    public class CommEligChecker_LrnWTech : CommEligChecker
    {
        private static readonly CommEligChecker_LrnWTech instance = new CommEligChecker_LrnWTech();

        static CommEligChecker_LrnWTech() { }
        private CommEligChecker_LrnWTech() { }
        public static CommEligChecker_LrnWTech Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // International Education
    public class CommEligChecker_IntEd : CommEligChecker
    {
        private static readonly CommEligChecker_IntEd instance = new CommEligChecker_IntEd();

        static CommEligChecker_IntEd() { }
        private CommEligChecker_IntEd() { }
        public static CommEligChecker_IntEd Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Library
    public class CommEligChecker_LibCom : CommEligChecker
    {
        private static readonly CommEligChecker_LibCom instance = new CommEligChecker_LibCom();

        static CommEligChecker_LibCom() { }
        private CommEligChecker_LibCom() { }
        public static CommEligChecker_LibCom Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Long Range Academic Planning
    public class CommEligChecker_LngRng : CommEligChecker
    {
        private static readonly CommEligChecker_LngRng instance = new CommEligChecker_LngRng();

        static CommEligChecker_LngRng() { }
        private CommEligChecker_LngRng() { }
        public static CommEligChecker_LngRng Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Membership and Elections
    public class CommEligChecker_MandE : CommEligChecker
    {
        private static readonly CommEligChecker_MandE instance = new CommEligChecker_MandE();

        static CommEligChecker_MandE() { }
        private CommEligChecker_MandE() { }
        public static CommEligChecker_MandE Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // University Academic Assessment
    public class CommEligChecker_UniAcadAss : CommEligChecker
    {
        private static readonly CommEligChecker_UniAcadAss instance = new CommEligChecker_UniAcadAss();

        static CommEligChecker_UniAcadAss() { }
        private CommEligChecker_UniAcadAss() { }
        public static CommEligChecker_UniAcadAss Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Faculty Financial Affairs
    public class CommEligChecker_FacFinAff : CommEligChecker
    {
        private static readonly CommEligChecker_FacFinAff instance = new CommEligChecker_FacFinAff();

        static CommEligChecker_FacFinAff() { }
        private CommEligChecker_FacFinAff() { }
        public static CommEligChecker_FacFinAff Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Honors Convocation
    public class CommEligChecker_HonConv : CommEligChecker
    {
        private static readonly CommEligChecker_HonConv instance = new CommEligChecker_HonConv();

        static CommEligChecker_HonConv() { }
        private CommEligChecker_HonConv() { }
        public static CommEligChecker_HonConv Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Academic Freedom and Tenure
    public class CommEligChecker_AcadFree : CommEligChecker
    {
        private static readonly CommEligChecker_AcadFree instance = new CommEligChecker_AcadFree();

        static CommEligChecker_AcadFree() { }
        private CommEligChecker_AcadFree() { }
        public static CommEligChecker_AcadFree Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election)
        {
            return tenureChecker(user);
        }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            return tenureChecker(user, reasons);
        }
    }

    // Faculty Awards and Recognition
    public class CommEligChecker_FacAward : CommEligChecker
    {
        private static readonly CommEligChecker_FacAward instance = new CommEligChecker_FacAward();

        static CommEligChecker_FacAward() { }
        private CommEligChecker_FacAward() { }
        public static CommEligChecker_FacAward Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election)
        {
            return tenureChecker(user);
        }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            return tenureChecker(user, reasons);
        }
    }

    // Undergraduate Curriculum
    public class CommEligChecker_UndCurr : CommEligChecker
    {
        private static readonly CommEligChecker_UndCurr instance = new CommEligChecker_UndCurr();

        static CommEligChecker_UndCurr() { }
        private CommEligChecker_UndCurr() { }
        public static CommEligChecker_UndCurr Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election)
        {
            return notLibraryChecker(user);
        }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            return notLibraryChecker(user, reasons);
        }
    }

    // Faculty Welfare
    public class CommEligChecker_FacWelf : CommEligChecker
    {
        private static readonly CommEligChecker_FacWelf instance = new CommEligChecker_FacWelf();

        static CommEligChecker_FacWelf() { }
        private CommEligChecker_FacWelf() { }
        public static CommEligChecker_FacWelf Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election)
        {
            return tenureChecker(user) && notLibraryChecker(user);
        }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            return tenureChecker(user, reasons) & notLibraryChecker(user, reasons);
        }
    }

    // Promotions
    public class CommEligChecker_Promo : CommEligChecker
    {
        private static readonly CommEligChecker_Promo instance = new CommEligChecker_Promo();

        static CommEligChecker_Promo() { }
        private CommEligChecker_Promo() { }
        public static CommEligChecker_Promo Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election)
        {
            return tenureChecker(user) && notLibraryChecker(user);
        }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            return tenureChecker(user, reasons) & notLibraryChecker(user, reasons);
        }
    }

    // Faculty Retrenchment
    public class CommEligChecker_FacRetr : CommEligChecker
    {
        private static readonly CommEligChecker_FacRetr instance = new CommEligChecker_FacRetr();

        static CommEligChecker_FacRetr() { }
        private CommEligChecker_FacRetr() { }
        public static CommEligChecker_FacRetr Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return false; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            reasons.Add("Must be appointed");
            return false;
        }
    }

    // Faculty Hearing
    public class CommEligChecker_FacHear : CommEligChecker
    {
        private static readonly CommEligChecker_FacHear instance = new CommEligChecker_FacHear();

        static CommEligChecker_FacHear() { }
        private CommEligChecker_FacHear() { }
        public static CommEligChecker_FacHear Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election)
        {
            return tenureChecker(user) && notHMARChecker(user, election);
        }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            return tenureChecker(user, reasons) & notHMARChecker(user, election, reasons);
        }
    }

    // Faculty Mediation
    public class CommEligChecker_FacMed : CommEligChecker
    {
        private static readonly CommEligChecker_FacMed instance = new CommEligChecker_FacMed();

        static CommEligChecker_FacMed() { }
        private CommEligChecker_FacMed() { }
        public static CommEligChecker_FacMed Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election)
        {
            return tenureChecker(user) && notHMARChecker(user, election);
        }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            return tenureChecker(user, reasons) & notHMARChecker(user, election, reasons);
        }
    }

    // Retrenchment Appeals
    public class CommEligChecker_RetrApp : CommEligChecker
    {
        private static readonly CommEligChecker_RetrApp instance = new CommEligChecker_RetrApp();

        static CommEligChecker_RetrApp() { }
        private CommEligChecker_RetrApp() { }
        public static CommEligChecker_RetrApp Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election)
        {
            if (user.Tenure != true)
            {
                FacultySenateDBEntities db = new FacultySenateDBEntities();

                if ((from mem in db.CommitteeMembers
                     where mem.CommitteeSeat.Committee.Name == "Retrenchment Appeals Committee" &&
                           election.TermStartDate < mem.EndDate &&
                           mem.Faculty.Tenure == true
                     select mem).Count() < 3)
                {
                    return false;
                }
            }

            if (!notHMARChecker(user, election))
                return false;

            return true;
        }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            if (user.Tenure != true)
            {
                FacultySenateDBEntities db = new FacultySenateDBEntities();

                if ((from mem in db.CommitteeMembers
                     where mem.CommitteeSeat.Committee.Name == "Retrenchment Appeals Committee" &&
                           election.TermStartDate < mem.EndDate &&
                           mem.Faculty.Tenure == true
                     select mem).Count() < 3)
                {
                    reasons.Add("Three members must be tenured");
                    return notHMARChecker(user, election, reasons) && false;
                }
            }

            if (!notHMARChecker(user, election, reasons))
                return false;

            return true;
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    // Consortium Committees
    ///////////////////////////////////////////////////////////////////////////

    // Consortium Coordinating
    public class CommEligChecker_ConsCoord : CommEligChecker
    {
        private static readonly CommEligChecker_ConsCoord instance = new CommEligChecker_ConsCoord();

        static CommEligChecker_ConsCoord() { }
        private CommEligChecker_ConsCoord() { }
        public static CommEligChecker_ConsCoord Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Traffic, Safety, Buildings and Grounds
    public class CommEligChecker_TrafSaf : CommEligChecker
    {
        private static readonly CommEligChecker_TrafSaf instance = new CommEligChecker_TrafSaf();

        static CommEligChecker_TrafSaf() { }
        private CommEligChecker_TrafSaf() { }
        public static CommEligChecker_TrafSaf Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Cultural Affairs
    public class CommEligChecker_CultAff : CommEligChecker
    {
        private static readonly CommEligChecker_CultAff instance = new CommEligChecker_CultAff();

        static CommEligChecker_CultAff() { }
        private CommEligChecker_CultAff() { }
        public static CommEligChecker_CultAff Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Facilities Management Advisory
    public class CommEligChecker_FacMan : CommEligChecker
    {
        private static readonly CommEligChecker_FacMan instance = new CommEligChecker_FacMan();

        static CommEligChecker_FacMan() { }
        private CommEligChecker_FacMan() { }
        public static CommEligChecker_FacMan Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Cultural Diversity
    public class CommEligChecker_CultDiv : CommEligChecker
    {
        private static readonly CommEligChecker_CultDiv instance = new CommEligChecker_CultDiv();

        static CommEligChecker_CultDiv() { }
        private CommEligChecker_CultDiv() { }
        public static CommEligChecker_CultDiv Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Fiscal Advisory
    public class CommEligChecker_FiscAdv : CommEligChecker
    {
        private static readonly CommEligChecker_FiscAdv instance = new CommEligChecker_FiscAdv();

        static CommEligChecker_FiscAdv() { }
        private CommEligChecker_FiscAdv() { }
        public static CommEligChecker_FiscAdv Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Government Relations
    public class CommEligChecker_GovRel : CommEligChecker
    {
        private static readonly CommEligChecker_GovRel instance = new CommEligChecker_GovRel();

        static CommEligChecker_GovRel() { }
        private CommEligChecker_GovRel() { }
        public static CommEligChecker_GovRel Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Wellness
    public class CommEligChecker_Well : CommEligChecker
    {
        private static readonly CommEligChecker_Well instance = new CommEligChecker_Well();

        static CommEligChecker_Well() { }
        private CommEligChecker_Well() { }
        public static CommEligChecker_Well Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Information Technology
    public class CommEligChecker_IT : CommEligChecker
    {
        private static readonly CommEligChecker_IT instance = new CommEligChecker_IT();

        static CommEligChecker_IT() { }
        private CommEligChecker_IT() { }
        public static CommEligChecker_IT Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    ///////////////////////////////////////////////////////////////////////////
    // "Other" Committees
    ///////////////////////////////////////////////////////////////////////////

    // Maryland Higher Education Commission Faculty Advisory
    public class CommEligChecker_MHEC : CommEligChecker
    {
        private static readonly CommEligChecker_MHEC instance = new CommEligChecker_MHEC();

        static CommEligChecker_MHEC() { }
        private CommEligChecker_MHEC() { }
        public static CommEligChecker_MHEC Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Council of University System Faculty
    public class CommEligChecker_CouncUni : CommEligChecker
    {
        private static readonly CommEligChecker_CouncUni instance = new CommEligChecker_CouncUni();

        static CommEligChecker_CouncUni() { }
        private CommEligChecker_CouncUni() { }
        public static CommEligChecker_CouncUni Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Athletics
    public class CommEligChecker_Athl : CommEligChecker
    {
        private static readonly CommEligChecker_Athl instance = new CommEligChecker_Athl();

        static CommEligChecker_Athl() { }
        private CommEligChecker_Athl() { }
        public static CommEligChecker_Athl Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return true; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons) { return true; }
    }

    // Graduate Council
    public class CommEligChecker_Grad : CommEligChecker
    {
        private static readonly CommEligChecker_Grad instance = new CommEligChecker_Grad();

        static CommEligChecker_Grad() { }
        private CommEligChecker_Grad() { }
        public static CommEligChecker_Grad Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election)
        {
            return graduateChecker(user);
        }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            return graduateChecker(user, reasons);
        }
    }

    // USM Diversity Network's Faculty Initiatives
    public class CommEligChecker_USMDiv : CommEligChecker
    {
        private static readonly CommEligChecker_USMDiv instance = new CommEligChecker_USMDiv();

        static CommEligChecker_USMDiv() { }
        private CommEligChecker_USMDiv() { }
        public static CommEligChecker_USMDiv Instance
        {
            get { return instance; }
        }

        public override Boolean getEligibility(Faculty user, Election election) { return false; }
        public override Boolean getEligibility(Faculty user, Election election, List<String> reasons)
        {
            reasons.Add("Must be appointed");
            return false;
        }
    }

}