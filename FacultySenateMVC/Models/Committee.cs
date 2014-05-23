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
    
    public partial class Committee
    {
        public Committee()
        {
            this.CommitteeSeat = new HashSet<CommitteeSeat>();
            this.ElectionVote = new HashSet<ElectionVote>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> TermLength { get; set; }
        public int CommitteeTypeId { get; set; }
        public bool Active { get; set; }
    
        public virtual CommitteeType CommitteeType { get; set; }
        public virtual ICollection<CommitteeSeat> CommitteeSeat { get; set; }
        public virtual ICollection<ElectionVote> ElectionVote { get; set; }
    }
}