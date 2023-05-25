using Job_Fair_Sys_Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Job_Fair_Sys_API.Models
{
    public class CompanyViewModel
    {
        public int id { get; set; }
        public int? userId { get; set; }
        public string name { get; set; }
        public int? noOfInterviewers { get; set; }
        public string contact1 { get; set; }
        public string contact2 { get; set; }
        public int timeSlot { get; set; }
        public string profile { get; set; }
        public string status { get; set; }
        public string email { get; set; }
        public List<CompanySkillViewModel> skills { get; set; }

        public Company ToEntity()
        {
            var entity = new Company
            {
                Id = this.id,
                UserId = this.userId,
                Name = this.name,
                NoOfInterviewers = this.noOfInterviewers,
                Contact1 = this.contact1,
                Contact2 = this.contact2,
                TimeSlot = this.timeSlot,
                Profile = this.profile,
                Status = this.status,
                Email = this.email
            };

            return entity;
        }

        public static CompanyViewModel ToViewModel(Company company)
        {
            var model = new CompanyViewModel
            {
                id = company.Id,
                userId = company.UserId,
                name = company.Name,
                noOfInterviewers = company.NoOfInterviewers,
                contact1 = company.Contact1,
                contact2 = company.Contact2,
                timeSlot = company.TimeSlot ?? 0,
                profile = company.Profile,
                status = company.Status,
                
 
                skills = company.CompanyRequiredSkills.Select(x => new CompanySkillViewModel{
                    id = x.Skill_Id,
                    technology = x.Skill?.Technology
                }).ToList()
            };

            return model;
        }

    }

    public class CompanySkillViewModel
    {
        public int id { get; set; }
        public string technology { get; set; }
    }
}
