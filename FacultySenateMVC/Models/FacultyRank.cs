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
    
    public partial class FacultyRank
    {
        public FacultyRank()
        {
            this.Faculty = new HashSet<Faculty>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Faculty> Faculty { get; set; }
    }
}