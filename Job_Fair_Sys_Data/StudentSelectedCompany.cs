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
    
    public partial class StudentSelectedCompany
    {
        public int id { get; set; }
        public int Company_Id { get; set; }
        public int Student_Id { get; set; }
        public Nullable<bool> DummyColumn { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual Student Student { get; set; }
    }
}
