using Job_Fair_Sys_Data;
using Job_Fair_Sys_Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Job_Fair_Sys_API.Controllers
{
    public class AvgFeedbackViewModel
    {
        public string studentName { get; set; }
        public decimal AverageFeedback { get; set; }
    }
    public class CompanyAvgViewModel
    {
        public string CompanyName { get; set; }
        public decimal CompanyAverageOfFeedbacks { get; set; }
    }


    public class Task2Controller : ApiController
    {

        private readonly CompanyRepository _companyRespository;
        public Task2Controller()
        {
            _companyRespository = new CompanyRepository();
        }

        [HttpGet]
        [Route("api/studentTechnlogiesFeedbackAvgInACompany")]
        public HttpResponseMessage technologiesAvg(int? companyId)
        {
            try
            {
                var queriedFeedbackOfStudentsInACompany = new List<StudentsFeedback>();

                if (companyId != null)
                {
                    queriedFeedbackOfStudentsInACompany = _companyRespository.DbContext.StudentsFeedbacks.Where(x => x.CompanyId == companyId).ToList();
                }
               

                var feedbacks = queriedFeedbackOfStudentsInACompany.GroupBy(x => x.StudentId).ToList();

                var avgOfStudentFeedBack = new List<AvgFeedbackViewModel>();

                //foreach
                foreach (var studentFeedback in feedbacks)
                {
                    var studentFeedbackSum = studentFeedback.Sum(x => x.rate) ?? 0;
                    var studentFeedbackCount = studentFeedback.Count();

                    decimal avgS = studentFeedbackSum / studentFeedbackCount;
                    var studentName = studentFeedback.FirstOrDefault().Student.Name;
                    var Obj = new AvgFeedbackViewModel
                    {
                        studentName = studentName,
                        AverageFeedback = avgS,
                    };

                    avgOfStudentFeedBack.Add(Obj);
                }

                //avgOfSkills = avgOfSkills.OrderBy(x => x.Skill_Id).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, avgOfStudentFeedBack);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/CompaniesAverage")]
        public HttpResponseMessage CompanyAvg()
        {
            try
            {
                var queriedFeedback = new List<StudentsFeedback>();
                    queriedFeedback = _companyRespository.DbContext.StudentsFeedbacks.ToList();
               

                var feedbacks = queriedFeedback.GroupBy(x => x.CompanyId).ToList();

                var response = new List<CompanyAvgViewModel>();

                //foreach
                foreach (var item in feedbacks)
                {
                    var companyFeedbacksSum = item.Sum(x => x.rate) ?? 0;
                    var companyFeedbacksCount = item.Count();

                    decimal avgS = companyFeedbacksSum / companyFeedbacksCount;
                   
                 var Model = new CompanyAvgViewModel
                 {
                     CompanyName = item.FirstOrDefault().Company.Name,
                     CompanyAverageOfFeedbacks = avgS,
                    
                 };
                    response.Add(Model);
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
