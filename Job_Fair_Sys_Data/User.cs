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
    
    public partial class User
    {
        public User()
        {
            this.Companies = new HashSet<Company>();
            this.SocietyMembers = new HashSet<SocietyMember>();
            this.Students = new HashSet<Student>();
        }
    
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
    
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<SocietyMember> SocietyMembers { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
