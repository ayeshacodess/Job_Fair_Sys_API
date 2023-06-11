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
        [Route("api/schedule/shortlist")]
        public HttpResponseMessage ShortlistStudent(bool isShortList, int studentId, int companyId, int scheduleId)
        {
            try
            {
                _scheduleRepository.GetRecordAndAddShortList(isShortList, studentId, companyId, scheduleId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            
        }
        
        [HttpGet]
        [Route("api/schedule/interviewed")]
        public HttpResponseMessage StudentInterviewed(bool isInterviewed, int studentId, int companyId, int scheduleId)
        {
            try
            {
                _scheduleRepository.GetRecordAndAddInterviewed(isInterviewed, studentId, companyId, scheduleId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

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

                var models = new List<Models.ScheduleViewModel>();

                foreach (var item in schedule)
                {
                    var company = companies.FirstOrDefault(c => c.Id == item.CompanyId);
                    var student = students.FirstOrDefault(s => s.Id == item.StudentId && s.StudentSelectedCompanies.Any(c => c.Company_Id == company.Id));

                    if (company == null || student == null)
                        continue;
                    else
                    {
                        var model = new Models.ScheduleViewModel
                        {
                            id = item.Id,

                            studentId = student.Id,
                            studentName = student.Name,
                            aridNumber = student.AridNumber,
                            companyId = company.Id,
                            compnayName = company.Name,
                            date = item.Date,
                            startTime = item.StartTime,
                            endTime = item.EndTime,
                            interviewed = item.Interviewed,
                            description = item.Description,
                            allocatedRoom = item.AllocatedRoom,
                            isShortListed = item.IsShortListed ?? false
                        };

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

                    //get the selected Company record to take 
                    var company = _companyRespository.GetCompany(reqModel.selectedCompany);
                    if (company != null)
                    {
                        var companySchedule = _scheduleRepository.GetACompanySchedule(reqModel.selectedCompany);
                        var students = _scheduleRepository.GetStudents(company.Id);
                        
                        //companySchedule.
                        var notScheduledStudentsIds = new List<int>();

                        foreach (var std in students)
                        {
                            var isScheduled = companySchedule.FirstOrDefault(x => x.StudentId == std.Student_Id);
                            if(isScheduled == null)
                            {
                                notScheduledStudentsIds.Add(std.Student_Id);
                            }
                        }

                        var notScheduledStudentsScheduleList = _scheduleRepository
                            .DbContext.InterviewSchedules
                            .Where(x => notScheduledStudentsIds.Contains(x.StudentId)).ToList();

                        var companyStartTime = GetCompanyStartTimeByTimeSlot(company.TimeSlot ?? 0);

                        var newScheduleList = new List<InterviewSchedule>();

                        foreach (var studentId in notScheduledStudentsIds)
                        {
                            //getting current student schedule
                            var studentSchedule = notScheduledStudentsScheduleList.Where(x => x.StudentId == studentId);

                            //flag to control the d while loop
                            var loopContinue = false;
                            do
                            {
                                //checking is commpany busy on its start time or not
                                var companyTempSchedule = companySchedule.FirstOrDefault(schedule => IsCompanyBusy(companyStartTime, schedule.StartTime));
                                
                                //if company is not buys then will go inside
                                if (companyTempSchedule == null)
                                {
                                    //need to verify the availablity of student, is he available or not
                                    var studentTempSchedule = studentSchedule.FirstOrDefault(schedule => IsCompanyBusy(companyStartTime, schedule.StartTime));

                                    //is student is not available, then go inside
                                    if (studentTempSchedule == null)
                                    {
                                        //create schedule start and end time, based on company and student available start and end time
                                        var scheduleStartTime = new DateTime(companyStartTime.Value.Year, companyStartTime.Value.Month, companyStartTime.Value.Day, companyStartTime.Value.Hour, companyStartTime.Value.Minute, companyStartTime.Value.Second);
                                        var scheduleEndTime = new DateTime(companyStartTime.Value.Year, companyStartTime.Value.Month, companyStartTime.Value.Day, companyStartTime.Value.Hour, companyStartTime.Value.Minute, companyStartTime.Value.Second);
                                        scheduleEndTime = scheduleEndTime.AddMinutes(reqModel.timeDuration);

                                        //create a schedule object, as both parties are not busy
                                        var scd = new InterviewSchedule
                                        {
                                            StudentId = studentId,
                                            CompanyId = company.Id,
                                            AllocatedRoom = reqModel.allocatedRoom,
                                            StartTime = scheduleStartTime,
                                            EndTime = scheduleEndTime,
                                            Interviewed = false,
                                            Date = DateTime.Now,
                                            TimeDuration = reqModel.timeDuration
                                        };

                                        if (reqModel.role == "SocietyMember")
                                            scd.SocietyMemberId = reqModel.UserProfile;

                                        if (reqModel.role == "Admin")
                                            scd.AdminId = reqModel.UserProfile;

                                        //assign the company time with the current new schedule end time
                                        companyStartTime = scheduleEndTime;

                                        //discontinue the do while loop
                                        loopContinue = false;

                                        //add schedule with in new list
                                        newScheduleList.Add(scd);
                                    } 
                                    else
                                    {
                                        //this case will be excecuted, when student is busy on givn company start time of the schedule
                                        //TODO: We will do it later

                                    } 
                                } 
                                else
                                {
                                    //This case will be executed when the company is busy on start time

                                    //start time change to next slot by adding 10 minutes in the start time
                                    companyStartTime = companyStartTime.Value.AddMinutes(10);

                                    //do while loop will continue to find next avaialble start time
                                    loopContinue = true;
                                }

                            } while (loopContinue);
                        }
                        //if (startTime != null)
                        //{
                        //    List<Job_Fair_Sys_Data.InterviewSchedule> schedules = new List<Job_Fair_Sys_Data.InterviewSchedule>();

                        //    foreach (var std in students)
                        //    {
                        //        var scd = new Job_Fair_Sys_Data.InterviewSchedule();

                        //        var scheduleStartTime = new DateTime(startTime.Value.Year, startTime.Value.Month, startTime.Value.Day, startTime.Value.Hour, startTime.Value.Minute, startTime.Value.Second);
                        //        var scheduleEndTime = new DateTime(startTime.Value.Year, startTime.Value.Month, startTime.Value.Day, startTime.Value.Hour, startTime.Value.Minute, startTime.Value.Second);
                        //        scheduleEndTime = scheduleEndTime.AddMinutes(reqModel.timeDuration);

                        //        scd.StudentId = std.Student_Id;
                        //        scd.CompanyId = company.Id;

                        //        if (reqModel.role == "SocietyMember")
                        //            scd.SocietyMemberId = reqModel.UserProfile;

                        //        if (reqModel.role == "Admin")
                        //            scd.AdminId = reqModel.UserProfile;

                        //        scd.AllocatedRoom = reqModel.allocatedRoom;
                        //        scd.StartTime = scheduleStartTime;
                        //        scd.EndTime = scheduleEndTime;
                        //        scd.Interviewed = false;
                        //        scd.Date = DateTime.Now;
                        //        scd.TimeDuration = reqModel.timeDuration;
                        //        startTime = scheduleEndTime;

                        //        schedules.Add(scd);
                        //    }

                        //    _scheduleRepository.AddSchedules(schedules);
                        //}

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

        private bool IsCompanyBusy(DateTime? companyStartTime, DateTime? scheduleStartTime)
        {
            if (companyStartTime is null)
                return false;

            if (scheduleStartTime is null)
                return false;

            var firstTime = companyStartTime?.TimeOfDay;
            var secondTime = scheduleStartTime?.TimeOfDay;

            if (firstTime == secondTime)
                return true;
            else
                return false;
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
