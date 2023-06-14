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
using System.Threading.Tasks;
using System.Web.Http;

namespace Job_Fair_Sys_API.Controllers
{
    public class StudentFeedbackController : ApiController
    {
        private readonly StudentFeedbackRepository _studentFeedbackRespository;
        public StudentFeedbackController()
        {
            _studentFeedbackRespository = new StudentFeedbackRepository();
        }

        [Route("api/student/studentFeedback")]
        [HttpPost]
        public async Task<HttpResponseMessage> StudentFeedback()
        {
            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
            {
                try
                {
                    string requestBody = reader.ReadToEnd();
                    var reqData = JsonConvert.DeserializeObject<StudentFeedbackViewModel>(requestBody);
                    var list = new List<StudentsFeedback>();
                    foreach (var item in reqData.stdFeedback)
                    {
                        var stdFeedBackObj = new StudentsFeedback
                        {
                            StudentId = reqData.studentId,
                            CompanyId = reqData.companyId,
                            Skill_ld = item.skill_ld,
                            rate = item.rate,
                        };
                        list.Add(stdFeedBackObj);
                    }

                    _studentFeedbackRespository.AddFeedback(list);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }

            }
        }

        [Route("api/student/getStudentFeedback")]
        [HttpGet]
        public HttpResponseMessage getStudentFeedback(int id)
        {
            try
            {
                var studentFeedbacks = _studentFeedbackRespository.DbContext.StudentsFeedbacks.Where(x => x.StudentId == id).ToList();

                var groupedData = studentFeedbacks.GroupBy(x => x.CompanyId).ToList();

                var models = new List<StudentFeedbackViewModel>();
                foreach (var item in groupedData)
                {
                    var feedbacks = item.ToList();

                    var model = new StudentFeedbackViewModel();

                    foreach (var fb in feedbacks)
                    {
                        model.studentId = fb.StudentId;
                        model.companyId = fb.Company.Id;
                        model.companyName = fb.Company.Name;

                        var data = new FeedbackViewMOdel
                        {
                            rate = fb.rate ?? 0,
                            skillName = fb.Skill.Technology,
                            skill_ld = fb.Skill.Id
                        };

                        model.stdFeedback.Add(data);
                    }

                    models.Add(model);
                }
                
                return Request.CreateResponse(HttpStatusCode.OK, models);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
