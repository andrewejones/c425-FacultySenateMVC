//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FacultySenateMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ElectionVote
    {
        public int Id { get; set; }
        public int ElectionId { get; set; }
        public int CommitteeId { get; set; }
        public int FacultyId_Nominee { get; set; }
        public int FacultyId_Voter { get; set; }
    
        public virtual Committee Committee { get; set; }
        public virtual Election Election { get; set; }
        public virtual Faculty Faculty_Nominee { get; set; }
        public virtual Faculty Faculty_Voter { get; set; }
    }
}