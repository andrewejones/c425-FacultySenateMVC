using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FacultySenateMVC.Models;

namespace FacultySenateMVC.Helpers
{

    // Used in M&E Area

    public class ElecInfo
    {
        public List<CommInfo> electionCommittees { get; set; }
        public List<int> electedSeats { get; set; }
        public Election election { get; set; }

        public ElecInfo(Election electionIn, List<CommInfo> electionCommitteesIn, List<int> electedSeatsIn)
        {
            election = electionIn;
            electionCommittees = electionCommitteesIn;
            electedSeats = electedSeatsIn;
        }
    }

    public class CommInfo
    {
        public Committee committee { get; set; }
        public List<NomInfo> nominees { get; set; }

        public CommInfo(Committee committeeIn, List<NomInfo> nomineesIn)
        {
            committee = committeeIn;
            nominees = nomineesIn;
        }
    }

    public class NomInfo
    {
        public Faculty nominee { get; set; }
        public int voteNum { get; set; }
        public int electedSeat { get; set; }
        public List<int> nominatedSeats { get; set; }
        public List<int> ineligibleSeats { get; set; }

        public NomInfo(Faculty nomineeIn, int voteNumIn, int electedSeatIn, List<int> nominatedSeatsIn, List<int> ineligibleSeatsIn)
        {
            nominee = nomineeIn;
            voteNum = voteNumIn;
            electedSeat = electedSeatIn;
            nominatedSeats = nominatedSeatsIn;
            ineligibleSeats = ineligibleSeatsIn;
        }
    }

    // Used in User Area

    public class CommitteeNomineesVM
    {
        public Committee committee { get; set; }
        public List<NomineeVoteVM> nominees { get; set; }
        public int totalVotesAllowed { get; set; }
        public int voteCount { get; set; }

        public CommitteeNomineesVM(Committee committeeIn)
        {
            committee = committeeIn;
            nominees = new List<NomineeVoteVM>();
            totalVotesAllowed = 0;
            voteCount = 0;
        }
    }

    public class NomineeVoteVM
    {
        public Faculty nominee { get; set; }
        public Boolean votedfor { get; set; }

        public NomineeVoteVM(Faculty nomineeIn, Boolean votedforIn)
        {
            nominee = nomineeIn;
            votedfor = votedforIn;
        }
    }

}