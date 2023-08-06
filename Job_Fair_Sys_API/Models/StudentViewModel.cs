using Job_Fair_Sys_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Fair_Sys_API.Models
{
    public class StudentViewModel
    {
        public int id { get; set; }
        public Nullable<int> userId { get; set; }
        public int noOfJumps { get; set; }
        public string name { get; set; }
        public string aridNumber { get; set; }
        public string studyStatus { get; set; }
        public string contact1 { get; set; }
        public string contact2 { get; set; }
        public Nullable<bool> gender { get; set; }
        public string cvpath { get; set; }
        public Nullable<bool> hasFYP { get; set; }
        public string FypTitle { get; set; }
        public string FypTech { get; set; }
        public string FypGrad { get; set; }
        public Nullable<bool> IsCVUploaded { get; set; }

        public List<StudentSkillViewModel> studentSkills { get; set; }

        public Student ToEntity()
        {
            var student = new Student
            {
                Id = this.id,
                UserId = this.userId,
                Name = this.name,
                AridNumber = this.aridNumber,
                StudyStatus = this.studyStatus,
                Contact1 = this.contact1,
                Contact2 = this.contact2,
                CVPath = this.cvpath,
                Gender = this.gender,
                HasFYP = this.hasFYP,
                FypGrad = this.FypGrad,
                FypTitle = this.FypTitle,
                FypTech = this.FypTech,
                IsCVUploaded = this.IsCVUploaded
            };

            return student;
        }

        public static StudentViewModel ToViewModel(Student student)
        {
            var model = new StudentViewModel
            {
                id = student.Id,
                userId = student.UserId,
                name = student.Name,
                aridNumber = student.AridNumber,
                studyStatus = student.StudyStatus,
                contact1 = student.Contact1,
                contact2 = student.Contact2,
                cvpath = student.CVPath,
                gender = student.Gender,
                FypGrad = student.FypGrad,
                FypTech = student.FypTech,
                FypTitle = student.FypTitle,
                studentSkills = student.StudentSkills.Select(x => new StudentSkillViewModel
                { 
                    skill_Id = x.Skill_Id, 
                    level_Id = 1
                }).ToList()
            };

            return model;
        }
    }

    public class StudentSkillViewModel
    {
        public int skill_Id { get; set; }
        public int level_Id { get; set; }
        

    }

    public class StudentsCGPAModel {
        public int studentId { get; set; }
        public double CGPA { get; set; } = 0.00;
    }

}