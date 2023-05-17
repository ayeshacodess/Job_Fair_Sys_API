using Job_Fair_Sys_Data;
using System;

namespace Job_Fair_Sys_API.Models
{
    public class MemberViewModel
    { 
        public int id { get; set; }
        public Nullable<int> userId { get; set; }
        public string name { get; set; }
        public bool gender { get; set; }
        public string contact { get; set; }

        public SocietyMember ToEntity()
        {
            var entity = new SocietyMember
            {
                Id = this.id,
                UserId = this.userId,
                Name = this.name,
                Gender = this.gender,
                Contact = this.contact,
            };

            return entity;
        }

        public static MemberViewModel ToViewModel(SocietyMember member)
        {
            var model = new MemberViewModel
            {
                id = member.Id,
                userId = member.UserId,
                name = member.Name,
                gender = member.Gender,
                contact = member.Contact,
            };

            return model;
        }

    }
}