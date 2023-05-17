using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Job_Fair_Sys_Data.Repositories
{
    public class CompanyRepository : BaseRepository<Company>
    {
        public CompanyRepository() : base()
        {
        }

        public List<Company> GetCompanies()
        {
            var companies = _dbContext.Companies.ToList();
            return companies;
        }

        public Company GetCompany(int id)
        {
            return _dbContext.Companies.Find(id);
        }

        public void Add(Company company)
        {
            _dbContext.Companies.Add(company);
            _dbContext.SaveChanges(); 
        }

        public int Update(Company company)
        {
            var dbcompany = _dbContext.Companies.FirstOrDefault(c => c.Id == company.Id);

            _dbContext.Entry(dbcompany).CurrentValues.SetValues(company);

            return _dbContext.SaveChanges();
        }

    }
}
