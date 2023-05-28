using System;
using System.Collections.Generic;
using System.Linq;

namespace Job_Fair_Sys_Data.Repositories
{
    public class ScheduleRepository : BaseRepository<InterviewSchedule>
    {
        public ScheduleRepository() : base()
        {
        }

        public List<StudentSelectedCompany> GetStudents(int companyId)
        {
            var students = _dbContext.StudentSelectedCompanies.Where(x =>  x.co);
            return students;
        }

        public void AddSchedules(List<InterviewSchedule> schedule)
        {
            foreach (var item in schedule)
            {
                _dbContext.InterviewSchedules.Add(item);
            }
            _dbContext.SaveChanges();
        }

        public List<InterviewSchedule> GetStudentsByInterviewSchedule(int userId)
        {
            var students = _dbContext.InterviewSchedules.Where(x => x.SocietyMemberId == userId).ToList();
            return students;
        }
    }
}