using Job_Fair_Sys_API.Models;
using Job_Fair_Sys_Data;
using Job_Fair_Sys_Data.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Job_Fair_Sys_API.Controllers
{
    public class CompanyController : ApiController
    {
        private readonly CompanyRepository _companyRespository;
        private readonly StudentSelectedCompanyRepository _studentSelectedCompanyRepository;
        private readonly AccountRepository _accountRepository;
        private readonly StudentRepository _studentRepository;

        public CompanyController()
        {
            _studentSelectedCompanyRepository = new StudentSelectedCompanyRepository();
            _companyRespository = new CompanyRepository();
            _accountRepository = new AccountRepository();
            _studentRepository = new StudentRepository();
        }


        [HttpGet]
        [Route("api/companies")]
        public HttpResponseMessage Get(string role, int userId)
        {
            try
            {
                var models = role == "Student" ? GetCompaniesForStudent(userId) : GetCompaniesDataByRole(role,userId);
                //var models = GetCompaniesDataByRole(role, userId);
                return Request.CreateResponse(HttpStatusCode.OK, models);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/company/add")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> AddCompanyAsync()
        {
            var httpRequest = HttpContext.Current.Request;

            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
            {
                try
                {
                    string requestBody = reader.ReadToEnd();

                    var company = JsonConvert.DeserializeObject<CompanyViewModel>(requestBody);

                    var entity = new Company
                    {
                        Id = company.id,
                        Name = company.name,
                        Contact1 = company.contact1,
                        Contact2 = company.contact2,
                        TimeSlot = company.timeSlot,
                        NoOfInterviewers = company.noOfInterviewers,
                        Status = "Pending",
                        Email = company.email
                    };

                    //_companyRespository.Add(entity);

                    var skills = _companyRespository.DbContext.Skills.ToList();
                    foreach (var skl in company.skills)
                    {
                        var skill = skills.FirstOrDefault(x => x.Id == skl.id); // will retun skill id nd tech

                        var newRequiredSkill = new CompanyRequiredSkill
                        {
                            NoOfInterviewers = 0, 
                            Skill = skill
                        };

                        entity.CompanyRequiredSkills.Add(newRequiredSkill);
                    }

                    _companyRespository.Add(entity);
                    return Request.CreateResponse(HttpStatusCode.OK, company);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("api/company/update")]
        public HttpResponseMessage UpdateCompany(CompanyViewModel company)
        {
            try
            {
                if (company == null || company.id == 0) return Request.CreateResponse(HttpStatusCode.NotAcceptable);

                var entity = _companyRespository.GetCompany(company.id);

                if (entity == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                //entity.Id = company.id;
                //entity.UserId = company.userId;
                //entity.Name = company.name;
                //entity.Contact1 = company.contact1;
                //entity.Contact2 = company.contact2;
                //entity.TimeSlot = company.timeSlot;
                //entity.Profile = company.profile;
                //entity.Status = company.status;
                CompanyViewModel.ToViewModel(entity);

                var removedSkills = entity.CompanyRequiredSkills.ToList();

                foreach (var s in removedSkills) {
                    entity.CompanyRequiredSkills.Remove(s);
                }
                var skills = _companyRespository.DbContext.Skills.ToList();
                
                foreach (var skl in company.skills)
                {
                    var skill = skills.FirstOrDefault(x => x.Id == skl.id);

                    var newRequiredSkill = new CompanyRequiredSkill
                    {
                        NoOfInterviewers = 0,
                        Skill = skill
                    };

                    entity.CompanyRequiredSkills.Add(newRequiredSkill);
                }

                _companyRespository.DbContext.Entry(entity).CurrentValues.SetValues(entity);
                _companyRespository.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, company);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/company/status")]
        public HttpResponseMessage SetAcceptRejectStatus(string status, int companyId)
        {
            //if()
            try
            {
                var company = _companyRespository.GetCompany(companyId);
                var u = _accountRepository.GetUser(company.Email);
                if (company == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                

                company.Status = status;
                _companyRespository.DbContext.Entry(company).CurrentValues.SetValues(company);
                _companyRespository.SaveChanges();

                var updatedCompanies = GetCompaniesDataByRole("Admin",companyId);
                var dbUser = _accountRepository.IsAuthorizedUser(company.Email, company.Contact1);


                 if (status == "Accept" && dbUser != null)
                {
                    #region Accept to accept
                    return Request.CreateResponse(HttpStatusCode.OK, updatedCompanies);
                    #endregion End!
                }

                //pending to accept
                else if(status == "Accept" && dbUser is null)
                {
                    #region pending to accept
                    var user = new User
                    {
                        Username = company.Email,
                        Password = company.Contact1,
                        Role = "Company"
                    };
                    // li.Add(user);
                    _companyRespository.AddUser(user);
                    #endregion End!
                }
                else if (status == "Reject" && dbUser == null)
                {
                    #region Pending to reject
                    return Request.CreateResponse(HttpStatusCode.OK, updatedCompanies);
                    // string v = _companyRespository.PendingToRemoveCompany(companyId);
                    #endregion End!
                }
                else if (status == "Reject" && dbUser != null)      //apko dikhana chah rae the k accept reject k button py nai
                    //hota blank nd student company select krta h wahan b select unselect k button py nai hota blank
                {
                    #region accept to reject
                    _accountRepository.RemoveUser(u);
                    return Request.CreateResponse(HttpStatusCode.OK, updatedCompanies);
                    // string v = _companyRespository.AcceptToRemoveCompany(u);
                    #endregion End!
                }
             
                
                else { 
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Please Click again!");
                } 

               
                
                return Request.CreateResponse(HttpStatusCode.OK, updatedCompanies);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            
        }

        [HttpGet]
        [Route("api/company/studentapply")]
        public HttpResponseMessage StudentSelectUnselectCompany(int companyId,  int studentId)
        {
           
            var company = _studentSelectedCompanyRepository.DbContext.StudentSelectedCompanies.FirstOrDefault(x => x.Student_Id == studentId && x.Company_Id == companyId);

            if (company == null)
            {
                var studentSelectedCompany = new StudentSelectedCompany
                {
                    Student_Id = studentId,
                    Company_Id = companyId
                };

                _studentSelectedCompanyRepository.Add(studentSelectedCompany);
            }
            else 
            {
                _studentSelectedCompanyRepository.DbContext.StudentSelectedCompanies.Remove(company);
                _studentSelectedCompanyRepository.SaveChanges();
            }

            var acceptedCompanies = GetCompaniesForStudent(studentId);

            return Request.CreateResponse(HttpStatusCode.OK, acceptedCompanies);
        }

        [HttpGet]
        [Route("api/company/rating")]
        public HttpResponseMessage CompanyRating(int companyId, int rating)
        {
            var company = _companyRespository.DbContext.Companies.FirstOrDefault(x => x.Id == companyId);
            
            if (company != null)
            {
                company.rate = rating;

                _companyRespository.DbContext.Entry(company).CurrentValues.SetValues(company);
                _companyRespository.SaveChanges();

            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private List<CompanyViewModel> GetCompaniesDataByRole(string role, int id)
        {
            //related companiries..
            var companies = _companyRespository.GetCompanies();
            List<CompanyViewModel> Result = new List<CompanyViewModel>();
            var companiesSkills = _companyRespository.GetCompanyRequiredSkills();

            if (role == "Student")
            {
                companies = companies.Where(c => c.Status == "Accept").ToList();
                var studentskills = _companyRespository.GetStudentSkills(id).Select(x => x.Skill_Id).ToList();

                companies.ForEach(company => {    
                    var companySkills = company.CompanyRequiredSkills.Select(x => x.Skill_Id).ToList();
                    var isAnySkillMatch = companySkills.Intersect(studentskills).ToList();
                    if(isAnySkillMatch.Count > 0)
                    {
                        Result.Add(CompanyViewModel.ToViewModel(company));
                    }
                });
            }
            else
            {
                foreach (var item in companies)
                {
                    var ViewModelTypeCompany = CompanyViewModel.ToViewModel(item); //convert a company into CompanyViewModel type
                    Result.Add(ViewModelTypeCompany);
                }
            } 


            //var models = new List<CompanyViewModel>();

            //foreach (var item in companies)
            //{
            //    var ViewModelTypeCompany = CompanyViewModel.ToViewModel(item); //convert a company into CompanyViewModel type
            //    models.Add(ViewModelTypeCompany);
            //}

            return Result;
        }

        //method to show on frontend companies listview to students 

        private List<CompanyViewModel> GetCompaniesForStudent(int studentId) 
        {
            var companies = GetCompaniesDataByRole("Student",studentId);

            var studentSlectedCompanies = _studentSelectedCompanyRepository.GetStudentSelectedCompanies(studentId);

            foreach (var item in companies)
            {
                if (studentSlectedCompanies.Any(c => c.Company_Id == item.id))
                {
                    item.status = "Selected";
                } else 
                {
                    item.status = "Un-Selected";
                }
            }
            return companies;
        }
    }
}