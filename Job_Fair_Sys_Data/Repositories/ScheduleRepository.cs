using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Fair_Sys_Data.Repositories
{
    public class ScheduleRepository : BaseRepository<StudentSelectedCompany>
    {
        public ScheduleRepository() : base()
        {
        }

        public List<StudentSelectedCompany> GetStudents(int companyId)
        {
            var students = _dbContext.StudentSelectedCompanies.Where(x => x.Company_Id == companyId).ToList();
            return students;
        }
        public void AddSchedule(List<InterviewSchedule> schedule)
        {
            foreach (var item in schedule)
            {
                _dbContext.InterviewSchedules.Add(item);
            }
            _dbContext.SaveChanges();
        }
    }
}