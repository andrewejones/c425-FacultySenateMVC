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
    
    public partial class ElectionSpecialSeat
    {
        public int Id { get; set; }
        public int ElectionId { get; set; }
        public int CommitteeSeatId { get; set; }
        public int TermLength { get; set; }
        public System.DateTime TermEndDate { get; set; }
    
        public virtual CommitteeSeat CommitteeSeat { get; set; }
        public virtual Election Election { get; set; }
    }
}