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
    
    public partial class Skill
    {
        public Skill()
        {
            this.StudentSkills = new HashSet<StudentSkill>();
            this.CompanyRequiredSkills = new HashSet<CompanyRequiredSkill>();
        }
    
        public int Id { get; set; }
        public string Technology { get; set; }
    
        public virtual ICollection<StudentSkill> StudentSkills { get; set; }
        public virtual ICollection<CompanyRequiredSkill> CompanyRequiredSkills { get; set; }
    }
}
