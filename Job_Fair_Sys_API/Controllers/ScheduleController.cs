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
        private readonly ScheduleRepository _scheduleRepository;
        private readonly CompanyRepository _companyRespository;

        public ScheduleController()
        {
            _scheduleRepository = new ScheduleRepository();
            _companyRespository = new CompanyRepository();
        }

        
        [HttpGet]
        [Route("api/schedule/get")]
        public HttpResponseMessage Get(string role, int userId)
        {
            try
            {
                //schedule
                var schedule = _scheduleRepository.GetSchedules();

                if (role == "Student")
                    schedule = schedule.Where(x => x.StudentId == userId).ToList();
                else if (role == "Company")
                    schedule = schedule.Where(x => x.CompanyId == userId).ToList();

                var students = _scheduleRepository.DbContext.Students.ToList();
                var companies = _scheduleRepository.DbContext.Companies.Where(c => c.Status == "Accept").ToList();

                var models = new List<ScheduleViewModel>();

                foreach (var item in schedule)
                {
                    var student = students.FirstOrDefault(s => s.Id == item.StudentId);
                    var company = companies.FirstOrDefault(c => c.Id == item.CompanyId);

                    var model = new ScheduleViewModel();

                    model.id = item.Id;
                    
                    model.studentId = student.Id;   
                    model.studentName = student.Name;
                    model.aridNumber = student.AridNumber;
                    model.companyId = company.Id;
                    model.compnayName = company.Name;
                    model.date = item.Date;
                    model.startTime = item.StartTime;
                    model.endTime = item.EndTime;
                    model.interviewed = item.Interviewed;
                    model.description = item.Description;
                    model.allocatedRoom = item.AllocatedRoom;

                    if (role == "Admin")
                    {
                        model.createorId = item.AdminId ?? 0;
                        model.creatorRole = "Admin";
                    } 
                    else if (role == "SocietyMember")
                    {
                        model.createorId = item.SocietyMemberId ?? 0;
                        model.creatorRole = "SocietMember";
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
                    var reqModel = JsonConvert.DeserializeObject<GenerateScheduleViewModel>(requestBody);

                    //-------------------------
                    var company = _companyRespository.GetCompany(reqModel.selectedCompany);
                    if (company != null)
                    {
                        var startTime = GetCompanyStartTimeByTimeSlot(company.TimeSlot ?? 0);
                        var students = _scheduleRepository.GetStudents(company.Id);

                        if (startTime != null)
                        {
                            List<InterviewSchedule> schedules = new List<InterviewSchedule>();

                            foreach (var std in students)
                            {
                                var scd = new InterviewSchedule();

                                var scheduleStartTime = new DateTime(startTime.Value.Year, startTime.Value.Month, startTime.Value.Day, startTime.Value.Hour, startTime.Value.Minute, startTime.Value.Second);
                                var scheduleEndTime = new DateTime(startTime.Value.Year, startTime.Value.Month, startTime.Value.Day, startTime.Value.Hour, startTime.Value.Minute, startTime.Value.Second);
                                scheduleEndTime = scheduleEndTime.AddMinutes(reqModel.timeDuration);

                                scd.StudentId = std.Student_Id;
                                scd.CompanyId = company.Id;

                                if (reqModel.role == "SocietyMember")
                                    scd.SocietyMemberId = reqModel.UserProfile;

                                if (reqModel.role == "Admin")
                                    scd.AdminId = reqModel.UserProfile;

                                scd.AllocatedRoom = reqModel.allocatedRoom;
                                scd.StartTime = scheduleStartTime;
                                scd.EndTime = scheduleEndTime;
                                scd.Interviewed = false;
                                scd.Date = DateTime.Now;

                                startTime = scheduleEndTime;

                                schedules.Add(scd);
                            }

                            _scheduleRepository.AddSchedules(schedules);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, reqModel);
                    }
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Company Not found.");
                }
                  catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        private DateTime? GetCompanyStartTimeByTimeSlot(int timeSlot)
        {
            var today = DateTime.Now;
            var nineAm = new DateTime(today.Year, today.Month, today.Day, 9, 0, 0);
            switch (timeSlot)
            {
                case 1:
                    //9-12
                    return nineAm;
                case 2:
                    var twelvePm = new DateTime(today.Year, today.Month, today.Day, 12, 0, 0);
                    return twelvePm;
                case 3:
                    //9-5
                    return nineAm;

                default: return null;
                   
            }
        }
    }
}
