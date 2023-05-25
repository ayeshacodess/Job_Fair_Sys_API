using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Job_Fair_Sys_Data.Repositories;
using System.Web.Http;
using Job_Fair_Sys_Data;
using Newtonsoft.Json;
using System;
using System.Web;
using Job_Fair_Sys_API.Models;
using System.IO;

namespace Job_Fair_Sys_API.Controllers
{
    public class ScheduleController : ApiController
    {
        protected readonly JobFairMgtEntities _dbContext = new JobFairMgtEntities();
        private readonly ScheduleRepository _scheduleRepository;
        private readonly CompanyRepository _companyRespository;
        public ScheduleController()
        {
            _scheduleRepository = new ScheduleRepository();
            _companyRespository = new CompanyRepository();
        }

        [HttpPost]
        [Route("api/schedule/generate")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> scheduleAsync()
        {
           var httpRequest = HttpContext.Current.Request;

            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
           {
                try
                {
                   string requestBody = reader.ReadToEnd();

                  var ob = JsonConvert.DeserializeObject<ScheduleViewModel>(requestBody);

        //            List<InterviewSchedule> schedule = new List<InterviewSchedule>();
        //            List<StudentSelectedCompany> students = _scheduleRepository.GetStudents(ob.);
        //            var company = _companyRespository.GetCompany(ob.companyId);
        //            var CompanyTime = company.TimeSlot;
        //            var comTime = company.TimeSlot;
        //            TimeSpan startTime9 = TimeSpan.FromHours(9);
        //            TimeSpan startTime12 = TimeSpan.FromHours(12);
        //            TimeSpan timeDuration = TimeSpan.FromMinutes(ob.timeDuration);
        //            TimeSpan endTime;
        //            foreach (var s in students)
        //            {
        //                schedule.Add(new InterviewSchedule
        //                {
        //                    StudentId = s.Student_Id,
        //                    CompanyId = ob.companyId,
        //                    SocietyMemberId = ob.societyMemberId,
        //                    Date = DateTime.Now,
        //                    TimeDuration = ob.timeDuration,
        //                    AllocatedRoom = ob.allocatedRoom,
        //                    StartTime = "9:00",
        //                    EndTime = "9:10",
    
        //            });
        //                _scheduleRepository.AddSchedule(schedule);
        //            }
                   return Request.CreateResponse(HttpStatusCode.OK, ob);
              }
               catch (Exception ex)
               {
                   return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
              }
           }
        }
    }
}
