using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FacultySenateMVC.Models;

///////////////////////////////////////////////////////////////////////////////
// NOTE: Most of the code can, and probably should, be moved up from the
//       specific election types to new helper methods in the base class
///////////////////////////////////////////////////////////////////////////////

namespace FacultySenateMVC.Helpers
{
    // Election Type Factory
    public sealed class ElectionTypeHelperFactory
    {
        private static readonly ElectionTypeHelperFactory instance = new ElectionTypeHelperFactory();

        static ElectionTypeHelperFactory() { }
        private ElectionTypeHelperFactory() { }

        public static ElectionTypeHelperFactory Instance
        {
            get { return instance; }
        }

        public ElectionTypeHelper getElecTypeEligChecker(Election election)
        {
            if (election.ElectionType.Name == "School")
            {
                return ElectionTypeHelper_School.Instance;
            }
            else if (election.ElectionType.Name == "At-Large")
            {
                return ElectionTypeHelper_AtLarge.Instance;
            }
            else if (election.ElectionType.Name == "Special (School)")
            {
                return ElectionTypeHelper_SpecialSchool.Instance;
            }
            else if (election.ElectionType.Name == "Special (At-Large)")
            {
                return ElectionTypeHelper_SpecialAtLarge.Instance;
            }
            else
            {
                throw new ArgumentException("Invalid election type; rules not yet defined", "election");
            }
        }
    }

    // Election Type Base Class
    public abstract class ElectionTypeHelper
    {
        // DB connection
        protected FacultySenateDBEntities db = new FacultySenateDBEntities();

        ///////////////////////////////////////////////////////////////////////
        // Nomination Methods
        ///////////////////////////////////////////////////////////////////////

        // get nominated, eligible and ineligible lists
        public abstract void nom_getEligibilityLists(Faculty user, Election election, List<Committee> nominated, List<Committee> eligible, List<Committee> ineligible);
        
        // returns list of nominees for committee
        public List<ElectionNomination> nom_getCommitteeNominees(Election election, Committee committee)
        {
            // get nominees
            var nominees = from nom in db.ElectionNominations
                           where election.Id == nom.ElectionId &&
                               committee.Id == nom.CommitteeSeat.CommitteeId
                           orderby nom.Faculty.FacultyDiscipline.FacultySchoolId, nom.Faculty.Last, nom.Faculty.First
                           group nom by nom.FacultyId into facnoms
                           select facnoms.FirstOrDefault();

            return nominees.ToList();
        }

        // returns reasons for ineligibility
        public abstract List<String> nom_getIneligibilityReasons(Faculty user, Election election, Committee committee);

        // nominate user for committee
        public abstract void nom_nominate(Faculty user, Election election, Committee committee);

        // unnominate user for committee
        public void nom_unnominate(Faculty user, Election election, Committee committee)
        {
            // get user nominations
            var nominations = from nom in db.ElectionNominations
                              where election.Id == nom.ElectionId &&
                                    committee.Id == nom.CommitteeSeat.CommitteeId &&
                                    user.Id == nom.FacultyId
                              select nom;

            // delete each nomination
            foreach (var nomination in nominations)
            {
                db.ElectionNominations.Remove(nomination);
            }

            // save changes to database
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

        }

        ///////////////////////////////////////////////////////////////////////
        // Voting Methods
        ///////////////////////////////////////////////////////////////////////

        // add user's vote for nominee
        public abstract void vote_vote(Faculty user, Faculty nominee, Committee committee, Election election);

        // remove user's vote for nominee
        public void vote_unvote(Faculty user, Faculty nominee, Committee committee, Election election)
        {
            var vote = (from v in db.ElectionVotes
                        where v.ElectionId == election.Id &&
                              v.CommitteeId == committee.Id &&
                              v.FacultyId_Nominee == nominee.Id &&
                              v.FacultyId_Voter == user.Id
                        select v).FirstOrDefault();

            db.ElectionVotes.Remove(vote);

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

        }

        // return list of nominees user may vote for
        public abstract List<CommitteeNomineesVM> vote_getVotableNominees(Faculty user, Election election);

        // return full list of nominees for election
        public List<CommitteeNomineesVM> vote_getAllNominees(Election election)
        {
            List<CommitteeNomineesVM> committeeNomineesList = new List<CommitteeNomineesVM>();

            // get committees with nominees
            var committees = (from nom in db.ElectionNominations
                              where nom.ElectionId == election.Id
                              select nom.CommitteeSeat.Committee).Distinct().
                              OrderBy(c => c.CommitteeTypeId).
                              ThenBy(c => c.Name);

            // for each committee with nominees
            foreach (var committee in committees)
            {

                CommitteeNomineesVM committeeNominees = new CommitteeNomineesVM(committee);

                // get nominees for this committee
                var nominees = (from nom in db.ElectionNominations
                                where nom.ElectionId == election.Id &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id
                                select nom.Faculty).Distinct().
                                OrderBy(f => f.FacultyDiscipline.FacultySchool.Name).
                                ThenBy(f => f.Last).
                                ThenBy(f => f.First);

                // for each nominee
                foreach (var nominee in nominees)
                {
                    // add to our list of nominees
                    committeeNominees.nominees.Add(new NomineeVoteVM(nominee, false));
                }

                // add this item to our list
                committeeNomineesList.Add(committeeNominees);
            }

            return committeeNomineesList;
        }

        ///////////////////////////////////////////////////////////////////////
        // Election Methods
        ///////////////////////////////////////////////////////////////////////

        // return list of committees and their nominees with eligible seats and vote counts
        public ElecInfo elect_getCommitteeNominees(Election election)
        {
            // committee members already elected
            var elecMems = from mem in db.CommitteeMembers
                           where mem.ElectionId == election.Id
                           select mem;

            // nominations
            var elecNoms = from nom in db.ElectionNominations
                           where nom.ElectionId == election.Id
                           select nom;

            // votes cast
            var elecVotes = from vote in db.ElectionVotes
                            where vote.ElectionId == election.Id
                            select vote;

            // committees with nominees
            var committees = (from nom in elecNoms
                              orderby nom.CommitteeSeat.Committee.CommitteeTypeId, nom.CommitteeSeat.Committee.Name
                              select nom.CommitteeSeat.Committee).Distinct();

            List<CommInfo> committeeInfoList = new List<CommInfo>();
            List<NomInfo> nomineeInfoList;
            List<int> elecSeats = new List<int>();
            List<int> nomSeats;
            List<int> ineligSeats;

            int voteNum, electedSeat;

            // for each committe with nominees
            foreach (var committee in committees)
            {
                // get nominees for this committee
                var nominees = (from nom in elecNoms
                                where nom.CommitteeSeat.CommitteeId == committee.Id
                                select nom.Faculty).Distinct().
                OrderByDescending(f => elecVotes.Where(v => v.CommitteeId == committee.Id && v.FacultyId_Nominee == f.Id).Count());

                nomineeInfoList = new List<NomInfo>();

                // for each of the nominees
                foreach (var nominee in nominees)
                {
                    // get number of votes
                    voteNum = elecVotes.Where(v => v.CommitteeId == committee.Id && v.FacultyId_Nominee == nominee.Id).Count();

                    // check if already elected for this committee in this election
                    var member = elecMems.Where(m => m.FacultyId == nominee.Id && m.CommitteeSeat.CommitteeId == committee.Id).FirstOrDefault();

                    // set elected seat and add to list if needed
                    electedSeat = 0;
                    if (member != null)
                    {
                        electedSeat = member.CommitteeSeatId;
                        elecSeats.Add(electedSeat);
                    }

                    // seats nominated for
                    var seats = from nom in elecNoms
                                where nom.FacultyId == nominee.Id &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id
                                orderby nom.CommitteeSeatId
                                select nom.CommitteeSeat;

                    nomSeats = seats.Select(n => n.Id).ToList();
                    ineligSeats = new List<int>();

                    // double check they're still eligible for each seat
                    foreach (var seat in seats)
                    {
                        // if not, then add to ineligible list
                        // NOTE: the only way they'd become ineligible during the electing period is if:
                        // - Someone with same discipline was elected
                        // - They were elected to a second committee
                        // - They're now ineligible for one of the "you cannot serve on these committees concurrently" committees
                        if (!(discNotOnCommChecker(nominee, election, seat.Committee) &&
                              notOnTwoCommChecker(nominee, election) &&
                              CommEligCheckerFactory.Instance.getCommEligChecker(seat.Committee).getEligibility(nominee, election)))
                        {
                            ineligSeats.Add(seat.Id);
                        }
                    }

                    // add to nominee list
                    nomineeInfoList.Add(new NomInfo(nominee, voteNum, electedSeat, nomSeats, ineligSeats));
                }

                // add to committee list
                committeeInfoList.Add(new CommInfo(committee, nomineeInfoList));
            }

            return new ElecInfo(election, committeeInfoList, elecSeats);
        }

        // elect member to seat
        public abstract void elect_elect(Faculty nominee, CommitteeSeat seat, Election election, String notes);

        // unelect member from seat
        public void elect_unelect(Faculty nominee, CommitteeSeat seat, Election election)
        {
            var member = (from mem in db.CommitteeMembers
                          where mem.ElectionId == election.Id &&
                                mem.CommitteeSeatId == seat.Id &&
                                mem.FacultyId == nominee.Id
                          select mem).FirstOrDefault();

            db.CommitteeMembers.Remove(member);

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        // Helper Methods for subclasses
        ///////////////////////////////////////////////////////////////////////

        // return true if Schools match
        protected Boolean sameSchoolChecker(Faculty user, CommitteeSeat seat)
        {
            return user.FacultyDiscipline.FacultySchoolId == seat.FacultySchoolId;
        }
        protected Boolean sameSchoolChecker(Faculty user, CommitteeSeat seat, List<String> reasons)
        {
            if (user.FacultyDiscipline.FacultySchoolId != seat.FacultySchoolId)
            {
                reasons.Add("No "+user.FacultyDiscipline.FacultySchool.Abbreviation+" seat available");
                return false;
            }
            return true;
        }

        // return true if NOT already on Committee
        protected Boolean notOnCommChecker(Faculty user, Election election, Committee committee)
        {
            return (from mem in db.CommitteeMembers
                    where mem.FacultyId == user.Id &&
                          mem.CommitteeSeat.CommitteeId == committee.Id &&
                          mem.EndDate > election.TermStartDate
                    select mem).FirstOrDefault() == null;
        }
        protected Boolean notOnCommChecker(Faculty user, Election election, Committee committee, List<String> reasons)
        {
            if ((from mem in db.CommitteeMembers
                 where mem.FacultyId == user.Id &&
                       mem.CommitteeSeat.CommitteeId == committee.Id &&
                       mem.EndDate > election.TermStartDate
                 select mem).FirstOrDefault() != null)
            {
                reasons.Add("Already on committee");
                return false;
            }
            return true;
        }

        // return true if discipline NOT on Committee
        protected Boolean discNotOnCommChecker(Faculty user, Election election, Committee committee)
        {
            return (from mem in db.CommitteeMembers
                    where mem.FacultyId != user.Id &&
                          mem.Faculty.FacultyDisciplineId == user.FacultyDisciplineId &&
                          mem.CommitteeSeat.CommitteeId == committee.Id &&
                          mem.EndDate > election.TermStartDate
                    select mem).FirstOrDefault() == null;
        }
        protected Boolean discNotOnCommChecker(Faculty user, Election election, Committee committee, List<String> reasons)
        {
            if ((from mem in db.CommitteeMembers
                 where mem.FacultyId != user.Id &&
                       mem.Faculty.FacultyDisciplineId == user.FacultyDisciplineId &&
                       mem.CommitteeSeat.CommitteeId == committee.Id &&
                       mem.EndDate > election.TermStartDate
                 select mem).FirstOrDefault() != null)
            {
                reasons.Add("Same discipline already on committee");
                return false;
            }
            return true;
        }

        // return true if NOT on two Committees
        protected Boolean notOnTwoCommChecker(Faculty user, Election election)
        {
            return ((from mem in db.CommitteeMembers
                     where mem.FacultyId == user.Id &&
                           mem.EndDate > election.TermStartDate
                     select mem).Count() < 2);
        }
        protected Boolean notOnTwoCommChecker(Faculty user, Election election, List<String> reasons)
        {
            if ((from mem in db.CommitteeMembers
                 where mem.FacultyId == user.Id &&
                       mem.EndDate > election.TermStartDate
                 select mem).Count() >= 2)
            {
                reasons.Add("Already on two committees");
                return false;
            }
            return true;
        }

        // return true if NOT on Committee for more than 6 of past 7 years during term
        protected Boolean notPast7YearsChecker(Faculty user, Election election, CommitteeSeat seat, int termLength)
        {
            // check (7 - termLength) years of history
            int numYearsToCheck = 7 - termLength;

            // get user's record on committee
            var records = from mem in db.CommitteeMembers
                          where mem.FacultyId == user.Id &&
                                mem.CommitteeSeat.CommitteeId == seat.CommitteeId &&
                                mem.Appointed == false
                          select mem;

            for (int i = 0; i < numYearsToCheck; i++)
            {
                // year to check
                int startYear = election.TermStartDate.Year - 1 - i;

                // check if they served
                Boolean served = records.Where(m => m.StartDate.Year <= startYear && startYear < m.EndDate.Year).Count() > 0;

                // if not they're eligible
                if (!served)
                {
                    return true;
                }
            }

            return false;
        }
        protected Boolean notPast7YearsChecker(Faculty user, Election election, CommitteeSeat seat, int termLength, List<String> reasons)
        {
            // check (7 - termLength) years of history
            int numYearsToCheck = 7 - termLength;

            // get user's record on committee
            var records = from mem in db.CommitteeMembers
                          where mem.FacultyId == user.Id &&
                                mem.CommitteeSeat.CommitteeId == seat.CommitteeId &&
                                mem.Appointed == false
                          select mem;

            for (int i = 0; i < numYearsToCheck; i++)
            {
                // year to check
                int startYear = election.TermStartDate.Year - 1 - i;

                // check if they served
                Boolean served = records.Where(m => m.StartDate.Year <= startYear && startYear < m.EndDate.Year).Count() > 0;

                // if not they're eligible
                if (!served)
                {
                    return true;
                }
            }

            reasons.Add("Cannot serve seven consecutive years");
            return false;
        }
        
    }

    ///////////////////////////////////////////////////////////////////////////
    // Election Types
    ///////////////////////////////////////////////////////////////////////////

    // School Election
    public class ElectionTypeHelper_School : ElectionTypeHelper
    {
        private static readonly ElectionTypeHelper_School instance = new ElectionTypeHelper_School();

        static ElectionTypeHelper_School() { }
        private ElectionTypeHelper_School() { }

        public static ElectionTypeHelper_School Instance
        {
            get { return instance; }
        }

        ///////////////////////////////////////////////////////////////////////
        // Nomination Methods
        ///////////////////////////////////////////////////////////////////////

        public override void nom_getEligibilityLists(Faculty user, Election election, List<Committee> nominated, List<Committee> eligible, List<Committee> ineligible)
        {
            // get committees nominated for
            var nominatedComms = (from nom in db.ElectionNominations
                                  where nom.ElectionId == election.Id &&
                                        nom.FacultyId == user.Id
                                  select nom.CommitteeSeat.Committee).Distinct().
                                  OrderBy(c => c.CommitteeTypeId).
                                  ThenBy(c => c.Name);

            // add each to 'nominated'
            foreach (var committee in nominatedComms)
            {
                nominated.Add(committee);
            }

            // get occupied seats
            var occupiedSeats = from member in db.CommitteeMembers
                                where election.TermStartDate < member.EndDate
                                select member.CommitteeSeatId;

            // find most compatible Seat from each Committee
            var openSeats = (from seat in db.CommitteeSeats // look in Seat table
                             where seat.FacultySchoolId != null && // school seat
                                   !occupiedSeats.Contains(seat.Id) && // not occupied
                                   !nominatedComms.Contains(seat.Committee) && // not nominated for it
                                   seat.Committee.TermLength != null && // has a term length
                                   seat.StaggerYear != null && // has a staggered term
                                   seat.Active == true && // active
                                   seat.Committee.Active == true // committee active
                             group seat by seat.CommitteeId into committeeseats // group Seats by Committee
                             select committeeseats.
                             OrderBy(s => Math.Abs(user.FacultyDiscipline.FacultySchoolId - (s.FacultySchoolId ?? int.MaxValue))). // best School match
                             ThenBy(s => ((int)((election.TermStartDate.Year - s.StaggerYear) % s.Committee.TermLength) == 0) ? s.Committee.TermLength : (int)((election.TermStartDate.Year - s.StaggerYear) % s.Committee.TermLength)). // shortest remaining term
                             FirstOrDefault()). // select best seat
                             OrderBy(s => s.Committee.CommitteeTypeId). // sort by committee type
                             ThenBy(s => s.Committee.Name); // then by name

            int termLength;

            // for each seat
            foreach (var seat in openSeats)
            {
                // calculate term length
                termLength = (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength);

                // 0 means full term length
                if (termLength == 0)
                {
                    termLength = (int)seat.Committee.TermLength;
                }

                // check eligibility for each Committee
                if (sameSchoolChecker(user, seat) &&
                    notOnCommChecker(user, election, seat.Committee) &&
                    discNotOnCommChecker(user, election, seat.Committee) &&
                    notOnTwoCommChecker(user, election) &&
                    notPast7YearsChecker(user, election, seat, termLength) &&
                    CommEligCheckerFactory.Instance.getCommEligChecker(seat.Committee).getEligibility(user, election))
                {
                    eligible.Add(seat.Committee);
                }
                else
                {
                    ineligible.Add(seat.Committee);
                }
            }
        }

        public override List<String> nom_getIneligibilityReasons(Faculty user, Election election, Committee committee)
        {
            // get occupied seats
            var occupiedSeats = from member in db.CommitteeMembers
                                where election.TermStartDate < member.EndDate
                                select member.CommitteeSeatId;

            // get open seat
            var openSeat = (from seat in db.CommitteeSeats // look in Seat table
                            where seat.CommitteeId == committee.Id && // match committee
                                  !occupiedSeats.Contains(seat.Id) && // not occupied
                                  seat.Committee.TermLength != null && // has a term length
                                  seat.StaggerYear != null && // has a staggered term
                                  seat.Active == true && // active
                                  seat.Committee.Active == true // committee active
                            select seat).
                            OrderBy(s => Math.Abs(user.FacultyDiscipline.FacultySchoolId - (s.FacultySchoolId ?? int.MaxValue))). // best School match
                            ThenBy(s => ((int)((election.TermStartDate.Year - s.StaggerYear) % s.Committee.TermLength) == 0) ? s.Committee.TermLength : (int)((election.TermStartDate.Year - s.StaggerYear) % s.Committee.TermLength)). // shortest remaining term
                            FirstOrDefault(); // best matching seat

            List<String> reasons = new List<String>();

            // calculate term length
            int termLength = (int)((election.TermStartDate.Year - openSeat.StaggerYear) % openSeat.Committee.TermLength);

            // 0 mean full term
            if (termLength == 0)
            {
                termLength = (int)openSeat.Committee.TermLength;
            }

            // get all reasons for ineligibility
            Boolean elig =
                sameSchoolChecker(user, openSeat, reasons) &
                notOnCommChecker(user, election, openSeat.Committee, reasons) &
                discNotOnCommChecker(user, election, openSeat.Committee, reasons) &
                notOnTwoCommChecker(user, election, reasons) &
                notPast7YearsChecker(user, election, openSeat, termLength, reasons) &
                CommEligCheckerFactory.Instance.getCommEligChecker(openSeat.Committee).getEligibility(user, election, reasons);

            return reasons;
        }

        public override void nom_nominate(Faculty user, Election election, Committee committee)
        {
            // occupied seats
            var occupiedSeats = from member in db.CommitteeMembers
                                where election.TermStartDate < member.EndDate
                                select member.CommitteeSeatId;

            var nomSeats = (from seat in db.ElectionNominations
                            where seat.ElectionId == election.Id &&
                                  seat.FacultyId == user.Id &&
                                  seat.CommitteeSeat.CommitteeId == committee.Id
                            select seat.CommitteeSeatId);

            // open seats
            var openSeats = (from seat in db.CommitteeSeats
                             where committee.Id == seat.CommitteeId && // match committee
                                   seat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId && // match school
                                   seat.Committee.TermLength != null && // has term length
                                   seat.StaggerYear != null && // has staggered term
                                   !occupiedSeats.Contains(seat.Id) && // not occupied
                                   seat.Active == true && // active
                                   seat.Committee.Active == true // committee active
                             select seat).Distinct();

            ElectionNomination nomination;
            int termLength;

            // for each open seat
            foreach (var seat in openSeats)
            {
                // calculate term length
                termLength = (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength);

                // 0 mean full term
                if (termLength == 0)
                {
                    termLength = (int)seat.Committee.TermLength;
                }

                // nominate for seat if eligible
                if (!nomSeats.Contains(seat.Id) &&
                    sameSchoolChecker(user, seat) &&
                    notOnCommChecker(user, election, seat.Committee) &&
                    discNotOnCommChecker(user, election, seat.Committee) &&
                    notOnTwoCommChecker(user, election) &&
                    notPast7YearsChecker(user, election, seat, termLength) &&
                    CommEligCheckerFactory.Instance.getCommEligChecker(seat.Committee).getEligibility(user, election))
                {
                    nomination = new ElectionNomination();

                    nomination.ElectionId = election.Id;
                    nomination.CommitteeSeatId = seat.Id;
                    nomination.FacultyId = user.Id;

                    db.ElectionNominations.Add(nomination);
                }
            }

            // save changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

        }

        ///////////////////////////////////////////////////////////////////////
        // Voting Methods
        ///////////////////////////////////////////////////////////////////////

        public override void vote_vote(Faculty user, Faculty nominee, Committee committee, Election election)
        {
            // check user and nominee of same school
            Boolean sameSchool = user.FacultyDiscipline.FacultySchoolId == nominee.FacultyDiscipline.FacultySchoolId;

            // check nominee was nominated
            Boolean nominated = (from nom in db.ElectionNominations
                                 where nom.ElectionId == election.Id &&
                                       nom.CommitteeSeat.CommitteeId == committee.Id &&
                                       nom.FacultyId == nominee.Id
                                 select nom).Count() > 0;

            // check user hasn't already voted for them
            Boolean notVoted = (from vote in db.ElectionVotes
                                where vote.ElectionId == election.Id &&
                                      vote.CommitteeId == committee.Id &&
                                      vote.FacultyId_Nominee == nominee.Id &&
                                      vote.FacultyId_Voter == user.Id
                                select vote).Count() == 0;

            // check that user has votes left
            int votesAllowed = (from nom in db.ElectionNominations
                                where nom.ElectionId == election.Id &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id &&
                                      nom.CommitteeSeat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId
                                select nom.CommitteeSeat).Distinct().Count();
            int votesMade = (from vote in db.ElectionVotes
                             where vote.ElectionId == election.Id &&
                                   vote.CommitteeId == committee.Id &&
                                   vote.FacultyId_Voter == user.Id
                             select vote).Count();
            Boolean canVote = votesAllowed > votesMade;

            // check users from same school, nominee was nominated, user didn't vote for nominee already, and user can vote
            if (sameSchool && nominated && notVoted && canVote)
            {
                ElectionVote vote = new ElectionVote();

                vote.ElectionId = election.Id;
                vote.CommitteeId = committee.Id;
                vote.FacultyId_Nominee = nominee.Id;
                vote.FacultyId_Voter = user.Id;

                db.ElectionVotes.Add(vote);
            }

            // save changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public override List<CommitteeNomineesVM> vote_getVotableNominees(Faculty user, Election election)
        {
            // committees that have nominees
            var committees = (from nom in db.ElectionNominations
                              where nom.ElectionId == election.Id &&
                                    nom.CommitteeSeat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId
                              select nom.CommitteeSeat.Committee).Distinct();

            // users votes
            var votes = from vote in db.ElectionVotes
                        where vote.ElectionId == election.Id &&
                              vote.FacultyId_Voter == user.Id
                        select vote;

            // create our list
            List<CommitteeNomineesVM> committeeNomineesList = new List<CommitteeNomineesVM>();

            // for each committee with nominees
            foreach (var committee in committees)
            {
                // add the committee to our list
                CommitteeNomineesVM committeeNominees = new CommitteeNomineesVM(committee);

                // total votes allowed equals the number of distinct seats with nominees
                committeeNominees.totalVotesAllowed = (from nom in db.ElectionNominations
                                                       where nom.ElectionId == election.Id &&
                                                             nom.CommitteeSeat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId &&
                                                             nom.CommitteeSeat.CommitteeId == committee.Id
                                                       select nom.CommitteeSeat).Distinct().Count();

                // get nominees for this committee
                var nominees = (from nom in db.ElectionNominations
                                where nom.ElectionId == election.Id &&
                                      nom.CommitteeSeat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id
                                select nom.Faculty).Distinct();

                // restrict vote count
                if (nominees.Count() < committeeNominees.totalVotesAllowed)
                {
                    committeeNominees.totalVotesAllowed = nominees.Count();
                }

                // for each nominee for this committee
                foreach (var nominee in nominees)
                {
                    // check if user voted for the nominee already
                    Boolean userVoted = votes.Where(v => v.FacultyId_Nominee == nominee.Id && v.CommitteeId == committee.Id).Count() > 0;
                    
                    if (userVoted)
                    {
                        // add nominee to our list, specify user voted for them already
                        committeeNominees.nominees.Add(new NomineeVoteVM(nominee, true));
                        // add to number of votes made
                        committeeNominees.voteCount++;
                    }
                    else
                    {
                        // add nominee to our list, specify user hasn't voted for them already
                        committeeNominees.nominees.Add(new NomineeVoteVM(nominee, false));
                    }
                }

                // add this one to our list
                committeeNomineesList.Add(committeeNominees);
            }

            return committeeNomineesList;
        }

        ///////////////////////////////////////////////////////////////////////
        // Election Methods
        ///////////////////////////////////////////////////////////////////////

        // elect nominee to seat (adds them to CommitteeMember table)
        public override void elect_elect(Faculty nominee, CommitteeSeat seat, Election election, String notes)
        {
            // calculate term length
            int termLength = ((int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength) == 0) ? (int)seat.Committee.TermLength : (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength);

            var elecMem = (from mem in db.CommitteeMembers
                           where mem.ElectionId == election.Id &&
                                 mem.CommitteeSeatId == seat.Id
                           select mem);

            // check eligibility
            if (elecMem == null &&
                sameSchoolChecker(nominee, seat) &&
                notOnCommChecker(nominee, election, seat.Committee) &&
                discNotOnCommChecker(nominee, election, seat.Committee) &&
                notOnTwoCommChecker(nominee, election) &&
                notPast7YearsChecker(nominee, election, seat, termLength) &&
                CommEligCheckerFactory.Instance.getCommEligChecker(seat.Committee).getEligibility(nominee, election))
            {
                CommitteeMember member = new CommitteeMember();

                member.FacultyId = nominee.Id;
                member.CommitteeSeatId = seat.Id;
                member.StartDate = election.TermStartDate;
                member.EndDate = member.StartDate.AddYears(termLength);
                member.ElectionId = election.Id;
                member.Appointed = false;
                member.Notes = notes;

                db.CommitteeMembers.Add(member);

                // save changes to DB
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Ineligible; not elected...");
            }

        }

    }

    // At-Large Election
    public class ElectionTypeHelper_AtLarge : ElectionTypeHelper
    {
        private static readonly ElectionTypeHelper_AtLarge instance = new ElectionTypeHelper_AtLarge();

        static ElectionTypeHelper_AtLarge() { }
        private ElectionTypeHelper_AtLarge() { }

        public static ElectionTypeHelper_AtLarge Instance
        {
            get { return instance; }
        }

        ///////////////////////////////////////////////////////////////////////
        // Nomination Methods
        ///////////////////////////////////////////////////////////////////////

        public override void nom_getEligibilityLists(Faculty user, Election election, List<Committee> nominated, List<Committee> eligible, List<Committee> ineligible)
        {
            // get committees nominated for
            var nominatedComms = (from nom in db.ElectionNominations
                                  where nom.ElectionId == election.Id &&
                                        nom.FacultyId == user.Id
                                  select nom.CommitteeSeat.Committee).Distinct().
                                  OrderBy(c => c.CommitteeTypeId).
                                  ThenBy(c => c.Name);

            // add to 'nominated' list
            foreach (var nom in nominatedComms)
            {
                nominated.Add(nom);
            }

            // occupied seats
            var occupiedSeats = from member in db.CommitteeMembers // look in Member table
                                  where election.TermStartDate < member.EndDate // match current Committee Members
                                  select member.CommitteeSeatId; // return list of Seat IDs
            
            // get list of open seats (except for Senate)
            var openSeats = (from seat in db.CommitteeSeats // look in Seat table
                             where seat.Committee.Name != "Senate" && // exclude Senate
                                   !occupiedSeats.Contains(seat.Id) && // not occupied
                                   !nominatedComms.Contains(seat.Committee) && // not nominated
                                   seat.Committee.TermLength != null && // has term length
                                   seat.StaggerYear != null && // has staggered term
                                   seat.Active == true && // active
                                   seat.Committee.Active == true // committee active
                             group seat by seat.CommitteeId into committeeseats // group seats by committee
                             select committeeseats.
                             OrderBy(s => Math.Abs((int)(user.FacultyDiscipline.FacultySchoolId - s.FacultySchoolId ?? int.MaxValue))). // school seats first
                             ThenBy(s => ((int)((election.TermStartDate.Year - s.StaggerYear) % s.Committee.TermLength) == 0) ? s.Committee.TermLength : (int)((election.TermStartDate.Year - s.StaggerYear) % s.Committee.TermLength)). // shortest remaining term
                             FirstOrDefault()). // select best seat
                             OrderBy(s => s.Committee.CommitteeTypeId).
                             ThenBy(s => s.Committee.Name);

            // get open seat for Senate (separate due to term being treated differently)
            var openSenateSeat = (from seat in db.CommitteeSeats // look in Seat table
                                  where seat.Committee.Name == "Senate" && // only Senate seat
                                        !occupiedSeats.Contains(seat.Id) && // not occupied
                                        !nominatedComms.Contains(seat.Committee) && // not already nominated
                                        seat.Active == true && // active
                                        seat.Committee.Active == true // committee active
                                  orderby ((int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength) == 0) ? seat.Committee.TermLength : (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength) // shortest term
                                  select seat).FirstOrDefault(); // best matching seat

            int termLength;

            // run checks on Senate seat, if there is one
            if (openSenateSeat != null)
            {
                termLength = (int)((election.TermStartDate.Year - openSenateSeat.StaggerYear) % openSenateSeat.Committee.TermLength);

                if (termLength == 0)
                {
                    termLength = (int)openSenateSeat.Committee.TermLength;
                }

                if (notOnCommChecker(user, election, openSenateSeat.Committee) &&
                    discNotOnCommChecker(user, election, openSenateSeat.Committee) &&
                    notOnTwoCommChecker(user, election) &&
                    notPast7YearsChecker(user, election, openSenateSeat, termLength) &&
                    CommEligCheckerFactory.Instance.getCommEligChecker(openSenateSeat.Committee).getEligibility(user, election))
                {
                    eligible.Add(openSenateSeat.Committee);
                }
                else
                {
                    ineligible.Add(openSenateSeat.Committee);
                }
            }

            // check eligibility for each Committee
            foreach (var seat in openSeats)
            {
                if (seat.FacultySchoolId != null && seat.FacultySchoolId != user.FacultyDiscipline.FacultySchoolId)
                {
                    termLength = 1;
                }
                else
                {
                    termLength = (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength);
                }

                if (termLength == 0)
                {
                    termLength = (int)seat.Committee.TermLength;
                }

                if (notOnCommChecker(user, election, seat.Committee) &&
                    discNotOnCommChecker(user, election, seat.Committee) &&
                    notOnTwoCommChecker(user, election) &&
                    notPast7YearsChecker(user, election, seat, termLength) &&
                    CommEligCheckerFactory.Instance.getCommEligChecker(seat.Committee).getEligibility(user, election))
                {
                    eligible.Add(seat.Committee);
                }
                else
                {
                    ineligible.Add(seat.Committee);
                }
            }

        }

        public override List<String> nom_getIneligibilityReasons(Faculty user, Election election, Committee committee)
        {
            // occupied seats
            var occupiedSeats = from member in db.CommitteeMembers
                                where committee.Id == member.CommitteeSeat.CommitteeId &&
                                      election.TermStartDate < member.EndDate
                                select member.CommitteeSeatId;

            // get open seat (except for Senate)
            var openSeat = (from seat in db.CommitteeSeats // look in Seat table
                            where seat.CommitteeId == committee.Id && // match Committee
                                  !occupiedSeats.Contains(seat.Id) && // not occupied
                                  seat.Committee.TermLength != null && // has term length
                                  seat.StaggerYear != null && // has staggered term
                                  seat.Active == true && // active
                                  seat.Committee.Active == true // committee active
                            orderby seat.FacultySchoolId ?? int.MaxValue, // school seats first
                                    ((int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength) == 0) ? seat.Committee.TermLength : (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength) // shortest term
                            select seat).FirstOrDefault();

            int termLength = (int)((election.TermStartDate.Year - openSeat.StaggerYear) % openSeat.Committee.TermLength);

            if (openSeat.FacultySchoolId != null && openSeat.FacultySchoolId != user.FacultyDiscipline.FacultySchoolId)
            {
                termLength = 1;
            }

            // get open seat for Senate (separate due to term being treated differently)
            if (committee.Name == "Senate")
            {
                openSeat = (from seat in db.CommitteeSeats // look in Seat table
                            where seat.CommitteeId == committee.Id && // match Committee
                                  !occupiedSeats.Contains(seat.Id) && // not occupied
                                  seat.Committee.TermLength != null && // has term length
                                  seat.StaggerYear != null && // has staggered term
                                  seat.Active == true && // active
                                  seat.Committee.Active == true // committee active
                            orderby ((int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength) == 0) ? seat.Committee.TermLength : (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength) // shortest term
                            select seat).FirstOrDefault();

                termLength = (int)((election.TermStartDate.Year - openSeat.StaggerYear) % openSeat.Committee.TermLength);

            }

            if (termLength == 0)
            {
                termLength = (int)openSeat.Committee.TermLength;
            }

            List<String> reasons = new List<String>();

            // get reasons for ineligibility
            Boolean elig =
                notOnCommChecker(user, election, openSeat.Committee, reasons) &
                discNotOnCommChecker(user, election, openSeat.Committee, reasons) &
                notOnTwoCommChecker(user, election, reasons) &
                notPast7YearsChecker(user, election, openSeat, termLength, reasons) &
                CommEligCheckerFactory.Instance.getCommEligChecker(openSeat.Committee).getEligibility(user, election, reasons);

            return reasons;
        }

        public override void nom_nominate(Faculty user, Election election, Committee committee)
        {
            // occupied seats
            var occupiedSeats = from member in db.CommitteeMembers
                                where election.TermStartDate < member.EndDate
                                select member.CommitteeSeatId;

            var nomSeats = (from seat in db.ElectionNominations
                            where seat.ElectionId == election.Id &&
                                  seat.FacultyId == user.Id &&
                                  seat.CommitteeSeat.CommitteeId == committee.Id
                            select seat.CommitteeSeatId);

            // get open seats
            var openSeats = (from seat in db.CommitteeSeats // look in Seat table
                             where seat.CommitteeId == committee.Id && // match Committee
                                   !occupiedSeats.Contains(seat.Id) && // not occupied
                                   seat.Committee.TermLength != null && // has term length
                                   seat.StaggerYear != null && // has staggered term
                                   seat.Active == true && // active
                                   seat.Committee.Active == true // committee active
                             select seat).Distinct();

            ElectionNomination nomination;
            int termLength;

            // nominate user for each Seat they're eligible for
            foreach (var seat in openSeats)
            {
                termLength = (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength);

                if (termLength == 0)
                {
                    termLength = (int)seat.Committee.TermLength;
                }

                if (seat.Committee.Name != "Senate" && seat.FacultySchoolId != null && seat.FacultySchoolId != user.FacultyDiscipline.FacultySchoolId)
                {
                    termLength = 1;
                }

                if (!nomSeats.Contains(seat.Id) &&
                    notOnCommChecker(user, election, seat.Committee) &&
                    discNotOnCommChecker(user, election, seat.Committee) &&
                    notOnTwoCommChecker(user, election) &&
                    notPast7YearsChecker(user, election, seat, termLength) &&
                    CommEligCheckerFactory.Instance.getCommEligChecker(seat.Committee).getEligibility(user, election))
                {
                    nomination = new ElectionNomination();

                    nomination.ElectionId = election.Id;
                    nomination.CommitteeSeatId = seat.Id;
                    nomination.FacultyId = user.Id;

                    db.ElectionNominations.Add(nomination);
                }
            }

            // save changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

        }

        ///////////////////////////////////////////////////////////////////////
        // Voting Methods
        ///////////////////////////////////////////////////////////////////////

        public override void vote_vote(Faculty user, Faculty nominee, Committee committee, Election election)
        {
            // check nominee was nominated
            Boolean nominated = (from nom in db.ElectionNominations
                                 where nom.ElectionId == election.Id &&
                                       nom.CommitteeSeat.CommitteeId == committee.Id &&
                                       nom.FacultyId == nominee.Id
                                 select nom).Count() > 0;

            // check user hasn't already voted for them
            Boolean notVoted = (from vote in db.ElectionVotes
                                where vote.ElectionId == election.Id &&
                                      vote.CommitteeId == committee.Id &&
                                      vote.FacultyId_Nominee == nominee.Id &&
                                      vote.FacultyId_Voter == user.Id
                                select vote).Count() == 0;

            // check that user has votes left
            int votesAllowed = (from nom in db.ElectionNominations
                                where nom.ElectionId == election.Id &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id
                                select nom.CommitteeSeat).Distinct().Count();
            int votesMade = (from vote in db.ElectionVotes
                             where vote.ElectionId == election.Id &&
                                   vote.CommitteeId == committee.Id &&
                                   vote.FacultyId_Voter == user.Id
                             select vote).Count();
            Boolean canVote = votesAllowed > votesMade;

            // check nominee was nominated, user didn't vote for nominee already, and user can vote
            if (nominated && notVoted && canVote)
            {
                ElectionVote vote = new ElectionVote();

                vote.ElectionId = election.Id;
                vote.CommitteeId = committee.Id;
                vote.FacultyId_Nominee = nominee.Id;
                vote.FacultyId_Voter = user.Id;

                db.ElectionVotes.Add(vote);
            }

            // save changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public override List<CommitteeNomineesVM> vote_getVotableNominees(Faculty user, Election election)
        {
            // committees that have nominees
            var committees = (from nom in db.ElectionNominations
                              where nom.ElectionId == election.Id
                              select nom.CommitteeSeat.Committee).Distinct();

            // users votes
            var votes = from vote in db.ElectionVotes
                        where vote.ElectionId == election.Id &&
                              vote.FacultyId_Voter == user.Id
                        select vote;

            // create our list
            List<CommitteeNomineesVM> committeeNomineesList = new List<CommitteeNomineesVM>();

            // for each committee with nominees
            foreach (var committee in committees)
            {
                // add the committee to our list
                CommitteeNomineesVM committeeNominees = new CommitteeNomineesVM(committee);

                // total votes allowed equals the number of distinct seats with nominees
                committeeNominees.totalVotesAllowed = (from nom in db.ElectionNominations
                                                       where nom.ElectionId == election.Id &&
                                                             nom.CommitteeSeat.CommitteeId == committee.Id
                                                       select nom.CommitteeSeat).Distinct().Count();

                // get nominees for this committee
                var nominees = (from nom in db.ElectionNominations
                                where nom.ElectionId == election.Id &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id
                                select nom.Faculty).Distinct();

                // restrict vote count
                if (nominees.Count() < committeeNominees.totalVotesAllowed)
                {
                    committeeNominees.totalVotesAllowed = nominees.Count();
                }

                // for each nominee for this committee
                foreach (var nominee in nominees)
                {
                    // check if user voted for the nominee already
                    Boolean userVoted = votes.Where(v => v.FacultyId_Nominee == nominee.Id && v.CommitteeId == committee.Id).Count() > 0;

                    if (userVoted)
                    {
                        // add nominee to our list, specify user voted for them already
                        committeeNominees.nominees.Add(new NomineeVoteVM(nominee, true));
                        // add to number of votes made
                        committeeNominees.voteCount++;
                    }
                    else
                    {
                        // add nominee to our list, specify user hasn't voted for them already
                        committeeNominees.nominees.Add(new NomineeVoteVM(nominee, false));
                    }
                }

                // add this one to our list
                committeeNomineesList.Add(committeeNominees);
            }

            return committeeNomineesList;
        }

        ///////////////////////////////////////////////////////////////////////
        // Election Methods
        ///////////////////////////////////////////////////////////////////////

        // elect nominee to seat
        public override void elect_elect(Faculty nominee, CommitteeSeat seat, Election election, String notes)
        {
            // calculate term length
            int termLength = (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength);

            if (seat.FacultySchoolId != null && seat.FacultySchoolId != nominee.FacultyDiscipline.FacultySchoolId)
            {
                termLength = 1;
            }

            if (seat.Committee.Name == "Senate")
            {
                termLength = (int)((election.TermStartDate.Year - seat.StaggerYear) % seat.Committee.TermLength);
            }

            if (termLength == 0)
            {
                termLength = (int)seat.Committee.TermLength;
            }
            
            var elecMem = (from mem in db.CommitteeMembers
                           where mem.ElectionId == election.Id &&
                                 mem.CommitteeSeatId == seat.Id
                           select mem);

            // check eligibility
            if (elecMem.Count() == 0 &&
                notOnCommChecker(nominee, election, seat.Committee) &&
                discNotOnCommChecker(nominee, election, seat.Committee) &&
                notOnTwoCommChecker(nominee, election) &&
                CommEligCheckerFactory.Instance.getCommEligChecker(seat.Committee).getEligibility(nominee, election))
            {
                CommitteeMember member = new CommitteeMember();

                member.FacultyId = nominee.Id;
                member.CommitteeSeatId = seat.Id;
                member.StartDate = election.TermStartDate;
                member.EndDate = member.StartDate.AddYears(termLength);
                member.ElectionId = election.Id;
                member.Appointed = false;
                member.Notes = notes;

                db.CommitteeMembers.Add(member);

                // save changes to DB
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
        }

    }

    // Special (School) Election
    public class ElectionTypeHelper_SpecialSchool : ElectionTypeHelper
    {
        private static readonly ElectionTypeHelper_SpecialSchool instance = new ElectionTypeHelper_SpecialSchool();

        static ElectionTypeHelper_SpecialSchool() { }
        private ElectionTypeHelper_SpecialSchool() { }

        public static ElectionTypeHelper_SpecialSchool Instance
        {
            get { return instance; }
        }

        ///////////////////////////////////////////////////////////////////////
        // Nomination Methods
        ///////////////////////////////////////////////////////////////////////

        public override void nom_getEligibilityLists(Faculty user, Election election, List<Committee> nominated, List<Committee> eligible, List<Committee> ineligible)
        {
            // get committees nominated for
            var nominatedComms = (from nom in db.ElectionNominations
                                  where nom.ElectionId == election.Id &&
                                        nom.FacultyId == user.Id
                                  select nom.CommitteeSeat.Committee).Distinct().
                                  OrderBy(c => c.CommitteeTypeId).
                                  ThenBy(c => c.Name);

            // add each to 'nominated'
            foreach (var committee in nominatedComms)
            {
                nominated.Add(committee);
            }

            // find most compatible Seat from each Committee
            var openSeats = (from seat in db.ElectionSpecialSeats // look in Seat table
                             where seat.CommitteeSeat.FacultySchoolId != null && // school seat
                                   !nominatedComms.Contains(seat.CommitteeSeat.Committee) && // not nominated for it
                                   seat.CommitteeSeat.Active == true && // active
                                   seat.CommitteeSeat.Committee.Active == true // committee active
                             group seat by seat.CommitteeSeat.CommitteeId into committeeseats // group Seats by Committee
                             select committeeseats.
                             OrderBy(s => Math.Abs(user.FacultyDiscipline.FacultySchoolId - (s.CommitteeSeat.FacultySchoolId ?? int.MaxValue))). // best School match
                             ThenBy(s => ((int)((election.TermStartDate.Year - s.CommitteeSeat.StaggerYear) % s.CommitteeSeat.Committee.TermLength) == 0) ? s.CommitteeSeat.Committee.TermLength : (int)((election.TermStartDate.Year - s.CommitteeSeat.StaggerYear) % s.CommitteeSeat.Committee.TermLength)). // shortest remaining term
                             FirstOrDefault()). // select best seat
                             OrderBy(s => s.CommitteeSeat.Committee.CommitteeTypeId). // sort by committee type
                             ThenBy(s => s.CommitteeSeat.Committee.Name); // then by name

            // for each seat
            foreach (var seat in openSeats)
            {
                // check eligibility for each Committee
                if (sameSchoolChecker(user, seat.CommitteeSeat) &&
                    notOnCommChecker(user, election, seat.CommitteeSeat.Committee) &&
                    discNotOnCommChecker(user, election, seat.CommitteeSeat.Committee) &&
                    notOnTwoCommChecker(user, election) &&
                    (seat.TermLength == 0 || notPast7YearsChecker(user, election, seat.CommitteeSeat, seat.TermLength)) &&
                    CommEligCheckerFactory.Instance.getCommEligChecker(seat.CommitteeSeat.Committee).getEligibility(user, election))
                {
                    eligible.Add(seat.CommitteeSeat.Committee);
                }
                else
                {
                    ineligible.Add(seat.CommitteeSeat.Committee);
                }
            }
        }

        public override List<String> nom_getIneligibilityReasons(Faculty user, Election election, Committee committee)
        {
            // open seats
            var openSeat = (from seat in db.ElectionSpecialSeats
                            where seat.CommitteeSeat.CommitteeId == committee.Id // match committee
                            orderby Math.Abs(user.FacultyDiscipline.FacultySchoolId - (seat.CommitteeSeat.FacultySchoolId ?? int.MaxValue)), // best School match
                                    seat.TermLength // shortest term
                            select seat).FirstOrDefault(); // select best seat

            List<String> reasons = new List<String>();

            // get reasons for ineligibility
            Boolean elig =
                sameSchoolChecker(user, openSeat.CommitteeSeat, reasons) &
                notOnCommChecker(user, election, openSeat.CommitteeSeat.Committee, reasons) &
                discNotOnCommChecker(user, election, openSeat.CommitteeSeat.Committee, reasons) &
                notOnTwoCommChecker(user, election, reasons) &
                (openSeat.TermLength == 0 || notPast7YearsChecker(user, election, openSeat.CommitteeSeat, openSeat.TermLength, reasons)) &
                CommEligCheckerFactory.Instance.getCommEligChecker(openSeat.CommitteeSeat.Committee).getEligibility(user, election, reasons);

            return reasons;
        }

        public override void nom_nominate(Faculty user, Election election, Committee committee)
        {
            // open seats
            var openSeats = (from specseat in db.ElectionSpecialSeats
                             where committee.Id == specseat.CommitteeSeat.CommitteeId && // match committee
                                   specseat.CommitteeSeat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId // match school
                             select specseat).Distinct(); // return Seats from Committee

            var nomSeats = (from seat in db.ElectionNominations
                            where seat.ElectionId == election.Id &&
                                  seat.FacultyId == user.Id &&
                                  seat.CommitteeSeat.CommitteeId == committee.Id
                            select seat.CommitteeSeatId);

            ElectionNomination nomination;

            // for each open seat
            foreach (var seat in openSeats)
            {
                // nominate for seat if eligible
                if (!nomSeats.Contains(seat.CommitteeSeatId) &&
                    sameSchoolChecker(user, seat.CommitteeSeat) &&
                    notOnCommChecker(user, election, seat.CommitteeSeat.Committee) &&
                    discNotOnCommChecker(user, election, seat.CommitteeSeat.Committee) &&
                    notOnTwoCommChecker(user, election) &&
                    (seat.TermLength == 0 || notPast7YearsChecker(user, election, seat.CommitteeSeat, seat.TermLength)) &&
                    CommEligCheckerFactory.Instance.getCommEligChecker(seat.CommitteeSeat.Committee).getEligibility(user, election))
                {
                    nomination = new ElectionNomination();

                    nomination.ElectionId = election.Id;
                    nomination.CommitteeSeatId = seat.Id;
                    nomination.FacultyId = user.Id;

                    db.ElectionNominations.Add(nomination);
                }
            }

            // save changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

        }

        ///////////////////////////////////////////////////////////////////////
        // Voting Methods
        ///////////////////////////////////////////////////////////////////////

        public override void vote_vote(Faculty user, Faculty nominee, Committee committee, Election election)
        {
            // check user and nominee of same school
            Boolean sameSchool = user.FacultyDiscipline.FacultySchoolId == nominee.FacultyDiscipline.FacultySchoolId;

            // check nominee was nominated
            Boolean nominated = (from nom in db.ElectionNominations
                                 where nom.ElectionId == election.Id &&
                                       nom.CommitteeSeat.CommitteeId == committee.Id &&
                                       nom.FacultyId == nominee.Id
                                 select nom).Count() > 0;

            // check user hasn't already voted for them
            Boolean notVoted = (from vote in db.ElectionVotes
                                where vote.ElectionId == election.Id &&
                                      vote.CommitteeId == committee.Id &&
                                      vote.FacultyId_Nominee == nominee.Id &&
                                      vote.FacultyId_Voter == user.Id
                                select vote).Count() == 0;

            // check that user has votes left
            int votesAllowed = (from nom in db.ElectionNominations
                                where nom.ElectionId == election.Id &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id &&
                                      nom.CommitteeSeat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId
                                select nom.CommitteeSeat).Distinct().Count();
            int votesMade = (from vote in db.ElectionVotes
                             where vote.ElectionId == election.Id &&
                                   vote.CommitteeId == committee.Id &&
                                   vote.FacultyId_Voter == user.Id
                             select vote).Count();
            Boolean canVote = votesAllowed > votesMade;

            // check users from same school, nominee was nominated, user didn't vote for nominee already, and user can vote
            if (sameSchool && nominated && notVoted && canVote)
            {
                ElectionVote vote = new ElectionVote();

                vote.ElectionId = election.Id;
                vote.CommitteeId = committee.Id;
                vote.FacultyId_Nominee = nominee.Id;
                vote.FacultyId_Voter = user.Id;

                db.ElectionVotes.Add(vote);
            }

            // save changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public override List<CommitteeNomineesVM> vote_getVotableNominees(Faculty user, Election election)
        {
            // committees that have nominees
            var committees = (from nom in db.ElectionNominations
                              where nom.ElectionId == election.Id &&
                                    nom.CommitteeSeat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId
                              select nom.CommitteeSeat.Committee).Distinct();

            // users votes
            var votes = from vote in db.ElectionVotes
                        where vote.ElectionId == election.Id &&
                              vote.FacultyId_Voter == user.Id
                        select vote;

            // create our list
            List<CommitteeNomineesVM> committeeNomineesList = new List<CommitteeNomineesVM>();

            // for each committee with nominees
            foreach (var committee in committees)
            {
                // add the committee to our list
                CommitteeNomineesVM committeeNominees = new CommitteeNomineesVM(committee);

                // total votes allowed equals the number of distinct seats with nominees
                committeeNominees.totalVotesAllowed = (from nom in db.ElectionNominations
                                                       where nom.ElectionId == election.Id &&
                                                             nom.CommitteeSeat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId &&
                                                             nom.CommitteeSeat.CommitteeId == committee.Id
                                                       select nom.CommitteeSeat).Distinct().Count();

                // get nominees for this committee
                var nominees = (from nom in db.ElectionNominations
                                where nom.ElectionId == election.Id &&
                                      nom.CommitteeSeat.FacultySchoolId == user.FacultyDiscipline.FacultySchoolId &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id
                                select nom.Faculty).Distinct();

                // restrict vote count
                if (nominees.Count() < committeeNominees.totalVotesAllowed)
                {
                    committeeNominees.totalVotesAllowed = nominees.Count();
                }

                // for each nominee for this committee
                foreach (var nominee in nominees)
                {
                    // check if user voted for the nominee already
                    Boolean userVoted = votes.Where(v => v.FacultyId_Nominee == nominee.Id && v.CommitteeId == committee.Id).Count() > 0;

                    if (userVoted)
                    {
                        // add nominee to our list, specify user voted for them already
                        committeeNominees.nominees.Add(new NomineeVoteVM(nominee, true));
                        // add to number of votes made
                        committeeNominees.voteCount++;
                    }
                    else
                    {
                        // add nominee to our list, specify user hasn't voted for them already
                        committeeNominees.nominees.Add(new NomineeVoteVM(nominee, false));
                    }
                }

                // add this one to our list
                committeeNomineesList.Add(committeeNominees);
            }

            return committeeNomineesList;
        }

        ///////////////////////////////////////////////////////////////////////
        // Election Methods
        ///////////////////////////////////////////////////////////////////////

        // elect nominee to seat (adds them to CommitteeMember table)
        public override void elect_elect(Faculty nominee, CommitteeSeat seat, Election election, String notes)
        {
            // get seat from special election table for term length and end date
            ElectionSpecialSeat specSeat = (from specseat in db.ElectionSpecialSeats
                                            where specseat.ElectionId == election.Id &&
                                                  specseat.CommitteeSeatId == seat.Id
                                            select specseat).FirstOrDefault();

            var elecMem = (from mem in db.CommitteeMembers
                           where mem.ElectionId == election.Id &&
                                 mem.CommitteeSeatId == seat.Id
                           select mem);

            // check eligibility
            if (elecMem == null &&
                specSeat != null &&
                sameSchoolChecker(nominee, seat) &&
                notOnCommChecker(nominee, election, seat.Committee) &&
                discNotOnCommChecker(nominee, election, seat.Committee) &&
                notOnTwoCommChecker(nominee, election) &&
                (specSeat.TermLength == 0 || notPast7YearsChecker(nominee, election, seat, specSeat.TermLength)) &&
                CommEligCheckerFactory.Instance.getCommEligChecker(seat.Committee).getEligibility(nominee, election))
            {
                CommitteeMember member = new CommitteeMember();

                member.FacultyId = nominee.Id;
                member.CommitteeSeatId = seat.Id;
                member.StartDate = election.TermStartDate;
                member.EndDate = specSeat.TermEndDate;
                member.ElectionId = election.Id;
                member.Appointed = (specSeat.TermLength == 0);
                member.Notes = notes;

                db.CommitteeMembers.Add(member);

                // save changes to DB
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

        }

    }

    // Special (At-Large) Election
    public class ElectionTypeHelper_SpecialAtLarge : ElectionTypeHelper
    {
        private static readonly ElectionTypeHelper_SpecialAtLarge instance = new ElectionTypeHelper_SpecialAtLarge();

        static ElectionTypeHelper_SpecialAtLarge() { }
        private ElectionTypeHelper_SpecialAtLarge() { }

        public static ElectionTypeHelper_SpecialAtLarge Instance
        {
            get { return instance; }
        }

        ///////////////////////////////////////////////////////////////////////
        // Nomination Methods
        ///////////////////////////////////////////////////////////////////////

        public override void nom_getEligibilityLists(Faculty user, Election election, List<Committee> nominated, List<Committee> eligible, List<Committee> ineligible)
        {
            // get committees nominated for
            var nominatedComms = (from nom in db.ElectionNominations
                                  where nom.ElectionId == election.Id &&
                                        nom.FacultyId == user.Id
                                  select nom.CommitteeSeat.Committee).Distinct().
                                  OrderBy(c => c.CommitteeTypeId).
                                  ThenBy(c => c.Name);

            // add each to 'nominated'
            foreach (var committee in nominatedComms)
            {
                nominated.Add(committee);
            }

            // find most compatible seat from each committee
            var openSeats = (from seat in db.ElectionSpecialSeats
                             where !nominatedComms.Contains(seat.CommitteeSeat.Committee) && // not nominated for it
                                   seat.CommitteeSeat.Active == true && // active
                                   seat.CommitteeSeat.Committee.Active == true // committee active
                             group seat by seat.CommitteeSeat.CommitteeId into committeeseats // group Seats by Committee
                             select committeeseats.
                             OrderBy(s => s.TermLength). // shortest term
                             FirstOrDefault()). // select best seat
                             OrderBy(s => s.CommitteeSeat.Committee.CommitteeTypeId). // sort by committee type
                             ThenBy(s => s.CommitteeSeat.Committee.Name); // then by name

            // for each seat
            foreach (var seat in openSeats)
            {
                // check eligibility for each Committee
                if (notOnCommChecker(user, election, seat.CommitteeSeat.Committee) &&
                    discNotOnCommChecker(user, election, seat.CommitteeSeat.Committee) &&
                    notOnTwoCommChecker(user, election) &&
                    (seat.TermLength == 0 || notPast7YearsChecker(user, election, seat.CommitteeSeat, seat.TermLength)) &&
                    CommEligCheckerFactory.Instance.getCommEligChecker(seat.CommitteeSeat.Committee).getEligibility(user, election))
                {
                    eligible.Add(seat.CommitteeSeat.Committee);
                }
                else
                {
                    ineligible.Add(seat.CommitteeSeat.Committee);
                }
            }
        }

        public override List<String> nom_getIneligibilityReasons(Faculty user, Election election, Committee committee)
        {
            // open seat
            var openSeat = (from specseat in db.ElectionSpecialSeats
                            where specseat.CommitteeSeat.CommitteeId == committee.Id // match committee
                            orderby specseat.TermLength // shortest term
                            select specseat).FirstOrDefault(); // best seat

            List<String> reasons = new List<String>();

            // get reasons for ineligibility
            Boolean elig =
                notOnCommChecker(user, election, openSeat.CommitteeSeat.Committee, reasons) &
                discNotOnCommChecker(user, election, openSeat.CommitteeSeat.Committee, reasons) &
                notOnTwoCommChecker(user, election, reasons) &
                (openSeat.TermLength == 0 || notPast7YearsChecker(user, election, openSeat.CommitteeSeat, openSeat.TermLength, reasons)) &
                CommEligCheckerFactory.Instance.getCommEligChecker(openSeat.CommitteeSeat.Committee).getEligibility(user, election, reasons);

            return reasons;
        }

        public override void nom_nominate(Faculty user, Election election, Committee committee)
        {
            // open seats
            var openSeats = (from seat in db.ElectionSpecialSeats
                             where committee.Id == seat.CommitteeSeat.CommitteeId // match committee
                             select seat).Distinct();

            var nomSeats = (from seat in db.ElectionNominations
                            where seat.ElectionId == election.Id &&
                                  seat.FacultyId == user.Id &&
                                  seat.CommitteeSeat.CommitteeId == committee.Id
                            select seat.CommitteeSeatId);

            ElectionNomination nomination;

            // nominate user for each seat they're eligible for
            foreach (var seat in openSeats)
            {
                if (!nomSeats.Contains(seat.CommitteeSeatId) &&
                    notOnCommChecker(user, election, seat.CommitteeSeat.Committee) &&
                    discNotOnCommChecker(user, election, seat.CommitteeSeat.Committee) &&
                    notOnTwoCommChecker(user, election) &&
                    (seat.TermLength == 0 || notPast7YearsChecker(user, election, seat.CommitteeSeat, seat.TermLength)) &&
                    CommEligCheckerFactory.Instance.getCommEligChecker(seat.CommitteeSeat.Committee).getEligibility(user, election))
                {
                    nomination = new ElectionNomination();

                    nomination.ElectionId = election.Id;
                    nomination.CommitteeSeatId = seat.CommitteeSeat.Id;
                    nomination.FacultyId = user.Id;

                    db.ElectionNominations.Add(nomination);
                }
            }

            // save changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

        }

        ///////////////////////////////////////////////////////////////////////
        // Voting Methods
        ///////////////////////////////////////////////////////////////////////

        public override void vote_vote(Faculty user, Faculty nominee, Committee committee, Election election)
        {
            // check nominee was nominated
            Boolean nominated = (from nom in db.ElectionNominations
                                 where nom.ElectionId == election.Id &&
                                       nom.CommitteeSeat.CommitteeId == committee.Id &&
                                       nom.FacultyId == nominee.Id
                                 select nom).Count() > 0;

            // check user hasn't already voted for them
            Boolean notVoted = (from vote in db.ElectionVotes
                                where vote.ElectionId == election.Id &&
                                      vote.CommitteeId == committee.Id &&
                                      vote.FacultyId_Nominee == nominee.Id &&
                                      vote.FacultyId_Voter == user.Id
                                select vote).Count() == 0;

            // check that user has votes left
            int votesAllowed = (from nom in db.ElectionNominations
                                where nom.ElectionId == election.Id &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id
                                select nom.CommitteeSeat).Distinct().Count();
            int votesMade = (from vote in db.ElectionVotes
                             where vote.ElectionId == election.Id &&
                                   vote.CommitteeId == committee.Id &&
                                   vote.FacultyId_Voter == user.Id
                             select vote).Count();
            Boolean canVote = votesAllowed > votesMade;

            // check nominee was nominated, user didn't vote for nominee already, and user can vote
            if (nominated && notVoted && canVote)
            {
                ElectionVote vote = new ElectionVote();

                vote.ElectionId = election.Id;
                vote.CommitteeId = committee.Id;
                vote.FacultyId_Nominee = nominee.Id;
                vote.FacultyId_Voter = user.Id;

                db.ElectionVotes.Add(vote);
            }

            // save changes to DB
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public override List<CommitteeNomineesVM> vote_getVotableNominees(Faculty user, Election election)
        {
            // committees that have nominees
            var committees = (from nom in db.ElectionNominations
                              where nom.ElectionId == election.Id
                              select nom.CommitteeSeat.Committee).Distinct();

            // users votes
            var votes = from vote in db.ElectionVotes
                        where vote.ElectionId == election.Id &&
                              vote.FacultyId_Voter == user.Id
                        select vote;

            // create our list
            List<CommitteeNomineesVM> committeeNomineesList = new List<CommitteeNomineesVM>();

            // for each committee with nominees
            foreach (var committee in committees)
            {
                // add the committee to our list
                CommitteeNomineesVM committeeNominees = new CommitteeNomineesVM(committee);

                // total votes allowed equals the number of distinct seats with nominees
                committeeNominees.totalVotesAllowed = (from nom in db.ElectionNominations
                                                       where nom.ElectionId == election.Id &&
                                                             nom.CommitteeSeat.CommitteeId == committee.Id
                                                       select nom.CommitteeSeat).Distinct().Count();

                // get nominees for this committee
                var nominees = (from nom in db.ElectionNominations
                                where nom.ElectionId == election.Id &&
                                      nom.CommitteeSeat.CommitteeId == committee.Id
                                select nom.Faculty).Distinct();

                // restrict vote count
                if (nominees.Count() < committeeNominees.totalVotesAllowed)
                {
                    committeeNominees.totalVotesAllowed = nominees.Count();
                }

                // for each nominee for this committee
                foreach (var nominee in nominees)
                {
                    // check if user voted for the nominee already
                    Boolean userVoted = votes.Where(v => v.FacultyId_Nominee == nominee.Id && v.CommitteeId == committee.Id).Count() > 0;

                    if (userVoted)
                    {
                        // add nominee to our list, specify user voted for them already
                        committeeNominees.nominees.Add(new NomineeVoteVM(nominee, true));
                        // add to number of votes made
                        committeeNominees.voteCount++;
                    }
                    else
                    {
                        // add nominee to our list, specify user hasn't voted for them already
                        committeeNominees.nominees.Add(new NomineeVoteVM(nominee, false));
                    }
                }

                // add this one to our list
                committeeNomineesList.Add(committeeNominees);
            }

            return committeeNomineesList;
        }

        ///////////////////////////////////////////////////////////////////////
        // Election Methods
        ///////////////////////////////////////////////////////////////////////

        // elect nominee to seat
        public override void elect_elect(Faculty nominee, CommitteeSeat seat, Election election, String notes)
        {
            // get seat from special election table for term length and end date
            ElectionSpecialSeat specSeat = (from specseat in db.ElectionSpecialSeats
                                            where specseat.ElectionId == election.Id &&
                                                  specseat.CommitteeSeatId == seat.Id
                                            select specseat).FirstOrDefault();
            
            var elecMem = (from mem in db.CommitteeMembers
                           where mem.ElectionId == election.Id &&
                                 mem.CommitteeSeatId == seat.Id
                           select mem);

            // check eligibility
            if (elecMem == null &&
                specSeat != null &&
                notOnCommChecker(nominee, election, seat.Committee) &&
                discNotOnCommChecker(nominee, election, seat.Committee) &&
                notOnTwoCommChecker(nominee, election) &&
                (specSeat.TermLength == 0 || notPast7YearsChecker(nominee, election, seat, specSeat.TermLength)) &&
                CommEligCheckerFactory.Instance.getCommEligChecker(seat.Committee).getEligibility(nominee, election))
            {
                CommitteeMember member = new CommitteeMember();

                member.FacultyId = nominee.Id;
                member.CommitteeSeatId = seat.Id;
                member.StartDate = election.TermStartDate;
                member.EndDate = specSeat.TermEndDate;
                member.ElectionId = election.Id;
                member.Appointed = (specSeat.TermLength == 0);
                member.Notes = notes;

                db.CommitteeMembers.Add(member);

                // save changes to DB
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

        }

    }

}