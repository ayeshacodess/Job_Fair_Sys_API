using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Fair_Sys_Data.Repositories
{
    public class DirectorRepository : BaseRepository<Student>
    {
        public DirectorRepository() : base()
        {
        }

        public List<Student> GetCompanyStudents(int companyId)
        {
            var studentsIds = _dbContext.StudentSelectedCompanies.Where(x => x.Company_Id == companyId);
            var students = new List<Student>();
            foreach (var stdid in studentsIds)
            {
                var student = _dbContext.Students.Find(stdid);
                students.Add(student);
            }
           return students;
        }
    }
}
