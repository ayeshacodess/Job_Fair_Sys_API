//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Job_Fair_Sys_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class SocietyMember
    {
        public SocietyMember()
        {
            this.InterviewSchedules = new HashSet<InterviewSchedule>();
        }
    
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Name { get; set; }
        public bool Gender { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
    
        public virtual ICollection<InterviewSchedule> InterviewSchedules { get; set; }
        public virtual User User { get; set; }
    }
}
