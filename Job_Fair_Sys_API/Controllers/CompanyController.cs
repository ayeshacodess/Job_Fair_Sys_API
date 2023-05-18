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

        public CompanyController()
        {
            _companyRespository = new CompanyRepository();
        }

        [HttpGet]
        [Route("api/companies")]
        public HttpResponseMessage Get()
        {
            try
            {
                var companies = _companyRespository.GetCompanies();
                List<CompanyViewModel> models = new List<CompanyViewModel>();

                foreach (var item in companies)
                {
                    var ViewModelTypeCompany = CompanyViewModel.ToViewModel(item); //convert a company into CompanyViewModel type
                    models.Add(ViewModelTypeCompany);
                }

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
            var body = await Request.Content.ReadAsStringAsync();
            using (var reader = new StreamReader(body) )
            {
                string requestBody = reader.ReadToEnd();
                var reqData = JsonConvert.DeserializeObject<CompanyViewModel>(requestBody);
                // Do something with the request body
            }
            return Request.CreateResponse(HttpStatusCode.OK);
            //try     
            //{
            //    if (company == null) return Request.CreateResponse(HttpStatusCode.NotAcceptable);

            //    var entity = company.ToEntity();
            //    //var entity = new Company();
            //    //entity.id = company.id;
            //    //entity.userId = company.userId;
            //    //entity.name = company.name;
            //    //entity.allotedRoom = company.allotedRoom;
            //    //entity.startTime = company.startTime;
            //    //entity.endTime = company.endTime;
            //    //_companyRespository.Add(entity);

            //    var skills = _companyRespository.DbContext.Skills.ToList();
            //    foreach (var skilliD in company.skill_Ids)
            //    {
            //        var skill = skills.FirstOrDefault(x => x.id == skilliD); // will retun skill id nd tech

            //        var newRequiredSkill = new CompanyRequiredSkill
            //        {
            //            noOfInterviewers = 0,
            //            Skill = skill
            //        };

            //        entity.CompanyRequiredSkills.Add(newRequiredSkill);
            //    }

            //     _companyRespository.Add(entity);

            //    return Request.CreateResponse(HttpStatusCode.OK, company);
            //}
            //catch (Exception ex)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            //}
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
                
                foreach (var skilliD in company.skill_Ids)
                {
                    var skill = skills.FirstOrDefault(x => x.Id == skilliD);

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
    }
}