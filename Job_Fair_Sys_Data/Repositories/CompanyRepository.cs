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
        public void AddUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public int Update(Company company)
        {
            var dbcompany = _dbContext.Companies.FirstOrDefault(c => c.Id == company.Id);

            _dbContext.Entry(dbcompany).CurrentValues.SetValues(company);

            return _dbContext.SaveChanges();
        }
        public void RemoveUser(User user)
        {
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }
        public string AcceptToRemoveCompany(User u)
        {
            string res = "";
            try
            {
                var com = _dbContext.Companies.Where(x => x.Email == u.Username).FirstOrDefault();
                if (com != null)
                {
                    _dbContext.Companies.Remove(com);
                    var removedSkills = _dbContext.CompanyRequiredSkills.Where(e=>e.CompanyId==com.Id).ToList();

                    foreach (var s in removedSkills)
                    {
                        _dbContext.CompanyRequiredSkills.Remove(s);
                    }
                    res += _dbContext.SaveChanges();
                }
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message+"\n"+ex.StackTrace;
               // throw;
            }
           
            
        }
        public string PendingToRemoveCompany(int u)
        {
            string res = "";
            try
            {
                var com = _dbContext.Companies.Where(x => x.Id == u).FirstOrDefault();
                if (com != null)
                {
                    _dbContext.Companies.Remove(com);
                    var removedSkills = _dbContext.CompanyRequiredSkills.Where(e => e.CompanyId == com.Id).ToList();

                    foreach (var s in removedSkills)
                    {
                        _dbContext.CompanyRequiredSkills.Remove(s);
                    }
                    res += _dbContext.SaveChanges();
                }
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message + "\n" + ex.StackTrace;
                // throw;
            }


        }


    }
}
