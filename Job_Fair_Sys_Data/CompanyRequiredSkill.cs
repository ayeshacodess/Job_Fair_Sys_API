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
    
    public partial class CompanyRequiredSkill
    {
        public int CompanyId { get; set; }
        public int Skill_Id { get; set; }
        public Nullable<int> NoOfInterviewers { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
