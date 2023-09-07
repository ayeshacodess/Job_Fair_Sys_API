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
    public class SkillAvgFeedbackViewModel
    {
        public int Skill_Id { get; set; }
        public string TechnologyName { get; set; }
        public decimal Average { get; set; }
    }

    public class StudentPercentageForCompanyViewModel
    {
        public int Skill_Id { get; set; }
        public string TechnologyName { get; set; }
        public decimal Average { get; set; }
        public int TotalNumberOfStudent { get; set; }
        public int NumberOfStudentExcellent { get; set; }
        public int NumberOfStudentGood { get; set; }
        public int NumberOfStudentFair{ get; set; }
        public int NumberOfStudentAverage { get; set; }
        public int NumberOfStudentBad { get; set; }
    }

    public class TaskController : ApiController
    {
        private readonly CompanyRepository _companyRespository;
        public TaskController()
        {
            _companyRespository = new CompanyRepository();
        }

        [HttpGet]
        [Route("api/technoAvg")]
        public HttpResponseMessage technologyAvg(int? companyId)
        {
            try
            {
                var queriedFeedback = new List<StudentsFeedback>();

                if (companyId != null)
                {
                    queriedFeedback = _companyRespository.DbContext.StudentsFeedbacks.Where(x => x.CompanyId == companyId).ToList();
                }
                else 
                {
                    queriedFeedback = _companyRespository.DbContext.StudentsFeedbacks.ToList();
                }

                var feedbacks = queriedFeedback.GroupBy(x => x.Skill_ld).ToList();
                
                var avgOfSkills = new List<SkillAvgFeedbackViewModel>();

                //foreach
                foreach (var item in feedbacks)
                {
                    var skillRateSum = item.Sum(x => x.rate) ?? 0;
                    var skillFeedbackCount = item.Count();

                    decimal avgS = skillRateSum / skillFeedbackCount;
                    var techName = item.FirstOrDefault().Skill.Technology;
                    var skilId = item.Key ?? 0;

                    avgOfSkills.Add(new SkillAvgFeedbackViewModel { Average = avgS, TechnologyName = techName, Skill_Id = skilId });
                }

                avgOfSkills = avgOfSkills.OrderBy(x => x.Skill_Id).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, avgOfSkills);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/StudentPercentageForCompany")]
        public HttpResponseMessage StudentPercentageInASkill(int? companyId)
        {
            try
            {
                var queriedFeedback = new List<StudentsFeedback>();

                if (companyId != null)
                {
                    queriedFeedback = _companyRespository.DbContext.StudentsFeedbacks.Where(x => x.CompanyId == companyId).ToList();
                }
                else
                {
                    queriedFeedback = _companyRespository.DbContext.StudentsFeedbacks.ToList();
                }

                var feedbacks = queriedFeedback.GroupBy(x => x.Skill_ld).ToList();

                var response = new List<StudentPercentageForCompanyViewModel>();

                //foreach
                foreach (var item in feedbacks)
                {
                    var skillRateSum = item.Sum(x => x.rate) ?? 0;
                    var skillFeedbackCount = item.Count();

                    decimal avgS = skillRateSum / skillFeedbackCount;

                    var totalStudentOfSkill = item.Count();
                    
                    var excellentSkillStudentCount = item.Count(x => x.rate == 5);
                    var goodSkillStudentCount = item.Count(x => x.rate == 4);
                    var fairSkillStudentCount = item.Count(x => x.rate == 3);
                    var averageSkillStudentCount = item.Count(x => x.rate == 2);
                    var worsttSkillStudentCount = item.Count(x => x.rate == 1);

                    var techName = item.FirstOrDefault().Skill.Technology;
                    var skilId = item.Key ?? 0;

                    var skilModel = new StudentPercentageForCompanyViewModel
                    {
                        Skill_Id = skilId,
                        TechnologyName = techName,
                        TotalNumberOfStudent = totalStudentOfSkill,
                        Average = avgS,
                        NumberOfStudentExcellent = excellentSkillStudentCount,
                        NumberOfStudentGood = goodSkillStudentCount,
                        NumberOfStudentAverage = averageSkillStudentCount,
                        NumberOfStudentFair = fairSkillStudentCount,
                        NumberOfStudentBad = worsttSkillStudentCount
                    };
                    response.Add(skilModel);
                }

                response = response.OrderBy(x => x.Skill_Id).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
