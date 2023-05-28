using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Fair_Sys_Data.Repositories
{
    public class StudentSelectedCompanyRepository: BaseRepository<StudentSelectedCompany>
    {
        public StudentSelectedCompanyRepository() : base()
        {

        }

        public List<StudentSelectedCompany> GetCompanySelectedStudents(int companyId) 
        {
            var data = _dbContext.StudentSelectedCompanies.Where(c => c.Company_Id == companyId).ToList();
            return data; 
        }

        public List<StudentSelectedCompany> GetStudentSelectedCompanies(int studentId)
        {
            var companies = _dbContext.StudentSelectedCompanies.Where(c => c.Student_Id == studentId).ToList();
            return companies;
        }

        public StudentSelectedCompany GetStudentSelectedCompany(int studentId, int companyId)
        {
            var company = _dbContext.StudentSelectedCompanies.FirstOrDefault(x => x.Company_Id == companyId && x.Student_Id == studentId);
            return company ;
        }

        public void Add(StudentSelectedCompany company)
        {
            _dbContext.StudentSelectedCompanies.Add(company);
            _dbContext.SaveChanges();
        }

        public void Remove(StudentSelectedCompany company) 
        {
            _dbContext.StudentSelectedCompanies.Remove(company);
            _dbContext.SaveChanges();
        }
    }
}
