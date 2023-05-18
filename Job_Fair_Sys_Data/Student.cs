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
    
    public partial class Student
    {
        public Student()
        {
            this.InterviewSchedules = new HashSet<InterviewSchedule>();
            this.StudentsFeedbacks = new HashSet<StudentsFeedback>();
            this.StudentSelectedCompanies = new HashSet<StudentSelectedCompany>();
            this.StudentSkills = new HashSet<StudentSkill>();
        }
    
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Name { get; set; }
        public string AridNumber { get; set; }
        public string StudyStatus { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public Nullable<bool> Gender { get; set; }
        public Nullable<double> CGPA { get; set; }
        public string CVPath { get; set; }
        public Nullable<bool> HasFYP { get; set; }
        public string FypTitle { get; set; }
        public string FypTech { get; set; }
        public string FypGrad { get; set; }
        public Nullable<bool> IsCVUploaded { get; set; }
    
        public virtual ICollection<InterviewSchedule> InterviewSchedules { get; set; }
        public virtual ICollection<StudentsFeedback> StudentsFeedbacks { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<StudentSelectedCompany> StudentSelectedCompanies { get; set; }
        public virtual ICollection<StudentSkill> StudentSkills { get; set; }
    }
}
