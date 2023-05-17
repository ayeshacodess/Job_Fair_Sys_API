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
    
    public partial class InterviewSchedule
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CompanyId { get; set; }
        public int SocietyMemberId { get; set; }
        public System.DateTime Date { get; set; }
        public int TimeSlot { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public bool Interviewed { get; set; }
        public string Description { get; set; }
        public string AllocatedRoom { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual SocietyMember SocietyMember { get; set; }
        public virtual Student Student { get; set; }
    }
}
