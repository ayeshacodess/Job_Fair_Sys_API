﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Job_Fair_Sys_Data.Repositories;
using System.Web.Http;
using Job_Fair_Sys_Data;
using Newtonsoft.Json;
using System.Web;
using Job_Fair_Sys_API.Models;
using System.IO;

namespace Job_Fair_Sys_API.Controllers
{
    public class ScheduleController : ApiController
    {
        private readonly ScheduleRepository _scheduleRepository;
        private readonly CompanyRepository _companyRespository;
        private readonly StudentRepository _studentRepository;
        public ScheduleController()
        {
            _scheduleRepository = new ScheduleRepository();
            _companyRespository = new CompanyRepository();
            _studentRepository = new StudentRepository();
        }

        [HttpGet]
        [Route("api/schedule/jumpQueue")]
        public HttpResponseMessage jumpQueue(int companyId, int studentId)
        {
            try
            {
                var stdnt = _studentRepository.GetStudent(studentId);
                var stdCGPA = stdnt.CGPA ?? 0.00;
                int jumpsTaken = stdnt.noOfJumpsTaken ?? 0;

                var allowedJumps = _studentRepository.DbContext.Passes.FirstOrDefault();
                int allowedJump = (stdCGPA >= 3.5 && stdCGPA < 4.01) ? allowedJumps.level1 ?? 0: allowedJumps.level2 ?? 0; 
                
                var company = _companyRespository.GetCompany(companyId);
                var compSchedule = _companyRespository.DbContext.InterviewSchedules.Where(x => x.CompanyId == companyId).ToList();

                var ifNotJumpedStudent = compSchedule.All(x => x.IsJumped == null || x.IsJumped == false);
                
                var newScheduleList = new List<InterviewSchedule>();

                if (allowedJump >= jumpsTaken)
                {
                    if (ifNotJumpedStudent)
                    {
                        _scheduleRepository.DeleteCompanySchedule(companyId);

                        //get company schedule
                        var companySchedule = _scheduleRepository.GetACompanySchedule(companyId);

                        var allocatedroom = companySchedule.FirstOrDefault()?.AllocatedRoom ?? "lab 10";

                        //get students id(s) who applied/selected the company
                        var students = _scheduleRepository.GetStudents(companyId);
                        //get those students ids(and add in list) who selected the company but are not scheduled with the company
                        var studentsToCreateSchedule = new List<StudentsCGPAModel>();

                        StudentsCGPAModel firstStudent = new StudentsCGPAModel();

                        firstStudent.studentId = studentId;
                        firstStudent.CGPA = stdCGPA;

                        foreach (var std in students)
                        {
                            studentsToCreateSchedule.Add(new StudentsCGPAModel { studentId = std.Student_Id, CGPA = std.Student.CGPA ?? 0.00 });
                        }

                        studentsToCreateSchedule = studentsToCreateSchedule.OrderBy(x => x.CGPA).ToList();
                        studentsToCreateSchedule.Insert(0, firstStudent);

                        var companyStartTime = GetCompanyStartTimeByTimeSlot(company.TimeSlot ?? 0);
                        var companyLeavingTime = GetCompanyEndTimeByTimeSlot(company.TimeSlot ?? 0);

                        var isCompanyBusyForWHoleTime = false;

                        foreach (var studentItem in studentsToCreateSchedule)
                        {
                            //If company is busy 
                            if (companyStartTime == companyLeavingTime)
                                break;

                            //create schedule start and end time, based on company and student available start and end time
                            var scheduleStartTime = new DateTime(companyStartTime.Value.Year, companyStartTime.Value.Month, companyStartTime.Value.Day, companyStartTime.Value.Hour, companyStartTime.Value.Minute, companyStartTime.Value.Second);
                            var scheduleEndTime = new DateTime(companyStartTime.Value.Year, companyStartTime.Value.Month, companyStartTime.Value.Day, companyStartTime.Value.Hour, companyStartTime.Value.Minute, companyStartTime.Value.Second);
                            scheduleEndTime = scheduleEndTime.AddMinutes(10);

                            //create a schedule object, as both parties are not busy
                            var scd = new InterviewSchedule
                            {
                                StudentId = studentItem.studentId,
                                CompanyId = company.Id,
                                AllocatedRoom = allocatedroom,
                                StartTime = scheduleStartTime,
                                EndTime = scheduleEndTime,
                                Interviewed = false,
                                Date = DateTime.Now,
                                TimeDuration = 10,
                                IsJumped = studentItem.studentId == studentId ? true : false,
                            };

                            //assign the company time with the current new schedule end time
                            companyStartTime = scheduleEndTime;

                            //add schedule with in new list
                            newScheduleList.Add(scd);
                        }
                    }
                    else
                    {
                        //get company schedule
                        var companySchedule = _scheduleRepository.GetACompanySchedule(companyId);

                        var jumpedStudentIds = companySchedule
                            .Where(x => x.IsJumped != null && x.IsJumped == true)
                            .Select(s => new StudentsCGPAModel { studentId = s.StudentId, CGPA = s.Student.CGPA ?? 0.00, isJumped = true });

                        var jumpingStudent = new StudentsCGPAModel { studentId = studentId, CGPA = stdCGPA, isJumped = true };

                        //Main ingredient for student sorting
                        var firstTeerStudents = new List<StudentsCGPAModel>();
                        var secondTeerStudents = new List<StudentsCGPAModel>();
                        var thirdTeerStudents = new List<StudentsCGPAModel>();

                        foreach (var std in jumpedStudentIds)
                        {
                            if (std.CGPA >= 3.5 && std.CGPA <= 4.00)
                            {
                                firstTeerStudents.Add(std);
                            }
                            else if (std.CGPA >= 3 && std.CGPA <= 3.5)
                            {
                                secondTeerStudents.Add(std);
                            }
                            else if (std.CGPA < 3)
                            {
                                thirdTeerStudents.Add(std);
                            }
                        }

                        if (jumpingStudent.CGPA >= 3.5 && jumpingStudent.CGPA <= 4.00)
                        {
                            if(!firstTeerStudents.Any(x =>x.studentId == jumpingStudent.studentId))
                            firstTeerStudents.Add(jumpingStudent);
                        }
                        else if (jumpingStudent.CGPA >= 3 && jumpingStudent.CGPA <= 3.5)
                        {
                            if (!secondTeerStudents.Any(x => x.studentId == jumpingStudent.studentId))
                                secondTeerStudents.Add(jumpingStudent);
                        }
                        else if (jumpingStudent.CGPA < 3)
                        {
                            if (!thirdTeerStudents.Any(x => x.studentId == jumpingStudent.studentId))
                                thirdTeerStudents.Add(jumpingStudent);
                        }

                        var finalJumpingStudents = new List<StudentsCGPAModel>();
                        finalJumpingStudents.AddRange(firstTeerStudents);
                        finalJumpingStudents.AddRange(secondTeerStudents);
                        finalJumpingStudents.AddRange(thirdTeerStudents);

                        var allocatedroom = companySchedule.FirstOrDefault()?.AllocatedRoom ?? "lab 10";

                        _scheduleRepository.DeleteCompanySchedule(companyId);

                        //get students id(s) who applied/selected the company
                        var students = _scheduleRepository.GetStudents(companyId);

                        //get those students ids(and add in list) who selected the company but are not scheduled with the company
                        var studentsToCreateSchedule = new List<StudentsCGPAModel>();

                        foreach (var std in students)
                        {
                            studentsToCreateSchedule.Add(new StudentsCGPAModel { studentId = std.Student_Id, CGPA = std.Student.CGPA ?? 0.00 });
                        }

                        finalJumpingStudents.AddRange(studentsToCreateSchedule);

                        var companyStartTime = GetCompanyStartTimeByTimeSlot(company.TimeSlot ?? 0);
                        var companyLeavingTime = GetCompanyEndTimeByTimeSlot(company.TimeSlot ?? 0);

                        foreach (var studentItem in finalJumpingStudents)
                        {
                            //If company is busy 
                            if (companyStartTime == companyLeavingTime)
                                break;

                            //create schedule start and end time, based on company and student available start and end time
                            var scheduleStartTime = new DateTime(companyStartTime.Value.Year, companyStartTime.Value.Month, companyStartTime.Value.Day, companyStartTime.Value.Hour, companyStartTime.Value.Minute, companyStartTime.Value.Second);
                            var scheduleEndTime = new DateTime(companyStartTime.Value.Year, companyStartTime.Value.Month, companyStartTime.Value.Day, companyStartTime.Value.Hour, companyStartTime.Value.Minute, companyStartTime.Value.Second);
                            scheduleEndTime = scheduleEndTime.AddMinutes(10);

                            //create a schedule object, as both parties are not busy
                            var scd = new InterviewSchedule
                            {
                                StudentId = studentItem.studentId,
                                CompanyId = company.Id,
                                AllocatedRoom = allocatedroom,
                                StartTime = scheduleStartTime,
                                EndTime = scheduleEndTime,
                                Interviewed = false,
                                Date = DateTime.Now,
                                TimeDuration = 10,
                                IsJumped = studentItem.studentId == studentId ? true : false,
                            };

                            //assign the company time with the current new schedule end time
                            companyStartTime = scheduleEndTime;

                            //add schedule with in new list
                            newScheduleList.Add(scd);
                        }
                    }
                }            

                if (newScheduleList.Count > 0)
                {
                    _scheduleRepository.AddSchedules(newScheduleList);
                }

                stdnt.noOfJumpsTaken = ++jumpsTaken;
                _studentRepository.DbContext.Entry(stdnt).CurrentValues.SetValues(stdnt);
                _studentRepository.DbContext.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
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
        [Route("api/schedule/interviewedAndShortListed")]
        public HttpResponseMessage InterviewedAndShortListedStudents()
        {
            try
            {
                var schedule = _scheduleRepository.GetInterviewedAndShortListedStudents();
                var model = new List<InterviewedAndShortListedViewModel>();
                foreach (var item in schedule)
                {
                    var std = new InterviewedAndShortListedViewModel
                    {
                        companyName = item.Company.Name,
                        studentName = item.Student.Name,
                        aridNumber = item.Student.AridNumber,
                        interviewed = isInterviewed(item.Interviewed),
                        shortListed = isShortListed(item.IsShortListed ?? false),

                    };
                    model.Add(std);
                }
               
                return Request.CreateResponse(HttpStatusCode.OK, model);
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
                    var student = students.FirstOrDefault(s => s.Id == item.StudentId);

                    if (company == null || student == null)
                        continue;
                    else
                    {

                        var model = new Models.ScheduleViewModel
                        {
                            id = item.Id,
                            percentile = SharedHelper.Utility.GetStudentPercentile(student.CGPA ?? 0),
                            studentId = student.Id,
                            myNumberInQueue = getNumberInQueue(item.CompanyId, item.StudentId),
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
                
                models.Sort((x, y) => x.startTime.Value.TimeOfDay.CompareTo(y.startTime.Value.TimeOfDay));

                return Request.CreateResponse(HttpStatusCode.OK, models);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/schedule/jumpTheQueue")] 
        public HttpResponseMessage Jump(int companyId, int studentId)
        {
            try
            {
                var company = _companyRespository.GetCompany(companyId);

                var s = _studentRepository.GetStudent(studentId);
                int numberOfJumps = s.noOfJumps ?? 0; //TODO: GET THE NUMBER OF JUMP FROM ALLOWED TABLE
                int jumpsTaken = s.noOfJumpsTaken ?? 0;

                if (jumpsTaken < numberOfJumps)
                {
                   if (company != null)
                   {
                        _scheduleRepository.DeleteCompanySchedule(companyId);

                        //get company schedule
                        var companySchedule = _scheduleRepository.GetACompanySchedule(companyId);

                        var allocatedroom = companySchedule.FirstOrDefault()?.AllocatedRoom ?? "lab 10";

                        //get students id(s) who applied/selected the company
                        var students = _scheduleRepository.GetStudents(company.Id);

                        //get those students ids(and add in list) who selected the company but are not scheduled with the company
                        var notScheduledStudentsIds = new List<StudentsCGPAModel>();

                        StudentsCGPAModel firstStudent = new StudentsCGPAModel();

                        foreach (var std in students)
                        {
                            if (std.Student_Id == studentId)
                            {
                                firstStudent.studentId = studentId;
                                firstStudent.CGPA = std.Student.CGPA ?? 0.00;
                            }
                            else
                            {
                                notScheduledStudentsIds.Add(new StudentsCGPAModel { studentId = std.Student_Id, CGPA = std.Student.CGPA ?? 0.00 });
                            }
                        }

                        notScheduledStudentsIds = notScheduledStudentsIds.OrderBy(x => x.CGPA).ToList();
                        notScheduledStudentsIds.Insert(0, firstStudent);

                        //get the schedule of those students who are not scheduled with the company, to see their available time
                        var notScheduledStudentsScheduleList = _scheduleRepository.DbContext.InterviewSchedules.ToList();
                        notScheduledStudentsScheduleList = notScheduledStudentsScheduleList.Where(x => notScheduledStudentsIds.Any(i => i.studentId == x.StudentId)).ToList();

                        var companyStartTime = GetCompanyStartTimeByTimeSlot(company.TimeSlot ?? 0);

                        var remainingStudentsIds = new List<StudentsCGPAModel>();
                        var newScheduleList = new List<InterviewSchedule>();
                        var isCompanyBusyForWHoleTime = false;

                        foreach (var studentItem in notScheduledStudentsIds)
                        {
                            //If company is busy 
                            if (isCompanyBusyForWHoleTime)
                                continue;

                            //getting current student schedule
                            var studentSchedule = notScheduledStudentsScheduleList.Where(x => x.StudentId == studentItem.studentId);

                            //flag to control the do while loop
                            var loopContinue = false;
                            do
                            {
                                //checking is commpany busy on its start time or not (on its strt time or in schedule time ??)
                                var companyTempSchedule = companySchedule.FirstOrDefault(schedule => IsCompanyBusy(companyStartTime, schedule.StartTime));

                                //if company is not buys then will go inside
                                if (companyTempSchedule == null)
                                {
                                    //need to verify the availablity of student, is he available or not
                                    var studentTempSchedule = studentSchedule.FirstOrDefault(schedule => IsCompanyBusy(companyStartTime, schedule.StartTime));

                                    if (studentItem.studentId == studentId)
                                    {
                                        //no need to check availablity 
                                        studentTempSchedule = null;
                                    }

                                    //is student is not available, then go inside
                                    if (studentTempSchedule == null)
                                    {
                                        //create schedule start and end time, based on company and student available start and end time
                                        var scheduleStartTime = new DateTime(companyStartTime.Value.Year, companyStartTime.Value.Month, companyStartTime.Value.Day, companyStartTime.Value.Hour, companyStartTime.Value.Minute, companyStartTime.Value.Second);
                                        var scheduleEndTime = new DateTime(companyStartTime.Value.Year, companyStartTime.Value.Month, companyStartTime.Value.Day, companyStartTime.Value.Hour, companyStartTime.Value.Minute, companyStartTime.Value.Second);
                                        scheduleEndTime = scheduleEndTime.AddMinutes(10);

                                        //create a schedule object, as both parties are not busy
                                        var scd = new InterviewSchedule
                                        {
                                            StudentId = studentItem.studentId,
                                            CompanyId = company.Id,
                                            AllocatedRoom = allocatedroom,
                                            StartTime = scheduleStartTime,
                                            EndTime = scheduleEndTime,
                                            Interviewed = false,
                                            Date = DateTime.Now,
                                            TimeDuration = 10
                                        };


                                        //assign the company time with the current new schedule end time
                                        companyStartTime = scheduleEndTime;

                                        //discontinue the do while loop
                                        loopContinue = false;

                                        //add schedule with in new list
                                        newScheduleList.Add(scd);

                                        remainingStudentsIds.Remove(studentItem);
                                    }
                                    else
                                    {
                                        //this case will be excecuted, when student is busy on givn company start time of the schedule
                                        remainingStudentsIds.Add(studentItem);
                                        var tempList = remainingStudentsIds.Concat(notScheduledStudentsIds).Distinct().OrderBy(x => x.CGPA).ToList();
                                        notScheduledStudentsIds = tempList;

                                        if (notScheduledStudentsIds.Count == 1)
                                        {
                                            //start time change to next slot by adding 10 minutes in the start time
                                            companyStartTime = companyStartTime.Value.AddMinutes(10);

                                            //we have to check, is company end time is here or not.
                                            var companyEndTime = GetCompanyEndTimeByTimeSlot(company.TimeSlot ?? 0);

                                            if (companyStartTime.Value.TimeOfDay == companyEndTime.Value.TimeOfDay)
                                            {
                                                isCompanyBusyForWHoleTime = true;
                                                loopContinue = false;
                                            }
                                            else
                                            {
                                                //do while loop will continue to find next avaialble start time
                                                loopContinue = true;
                                            }
                                        }
                                        else
                                        {
                                            loopContinue = false;
                                        }
                                    }
                                }
                                else
                                {
                                    //This case will be executed when the company is busy on start time
                                    //start time change to next slot by adding 10 minutes in the start time
                                    companyStartTime = companyStartTime.Value.AddMinutes(10);

                                    //we have to check, is company end time is here or not.
                                    var companyEndTime = GetCompanyEndTimeByTimeSlot(company.TimeSlot ?? 0);

                                    if (companyStartTime.Value.TimeOfDay == companyEndTime.Value.TimeOfDay)
                                    {
                                        isCompanyBusyForWHoleTime = true;
                                        loopContinue = false;
                                    }
                                    else
                                    {
                                        //do while loop will continue to find next avaialble start time
                                        loopContinue = true;
                                    }
                                }

                            } while (loopContinue);
                        }

                        if (newScheduleList.Count > 0)
                        {
                            _scheduleRepository.AddSchedules(newScheduleList);
                        }
                        var st = _companyRespository.DbContext.StudentSelectedCompanies.Where(x => x.Student_Id == studentId).ToList();
                        foreach (var item in st)
                        {
                            item.IsJumped = true ;
                        }
                    }

                    s.noOfJumpsTaken = ++jumpsTaken;

                    _studentRepository.DbContext.Entry(s).CurrentValues.SetValues(s);
                    _studentRepository.DbContext.SaveChanges();

                }

                return Request.CreateResponse(HttpStatusCode.OK);
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

                    //get the selected Company record to take timeSlot
                    var company = _companyRespository.GetCompany(reqModel.selectedCompany);

                    if (company != null)
                    {
                        //get the company schedule
                        var companySchedule = _scheduleRepository.GetACompanySchedule(reqModel.selectedCompany);

                        //get students id(s) who applied/selected the company
                        var students = _scheduleRepository.GetStudents(company.Id);

                        //get those students ids(and add in list) who selected the company but are not scheduled with the company
                        var notScheduledStudentsIds = new List<StudentsCGPAModel>();
                        foreach (var std in students)
                        {
                            var isScheduled = companySchedule.FirstOrDefault(x => x.StudentId == std.Student_Id);
                            if(isScheduled == null)
                            {
                                notScheduledStudentsIds.Add(new StudentsCGPAModel { studentId= std.Student_Id , CGPA = std.Student.CGPA ?? 0.00});
                            }
                        }

                        //get the schedule of those students who are not scheduled with the company, to see their available time
                        var notScheduledStudentsScheduleList = _scheduleRepository.DbContext.InterviewSchedules.ToList();

                        notScheduledStudentsScheduleList = notScheduledStudentsScheduleList.Where(x => notScheduledStudentsIds.Any(i => i.studentId == x.StudentId)).ToList();

                        var companyStartTime = GetCompanyStartTimeByTimeSlot(company.TimeSlot ?? 0);

                        var remainingStudentsIds = new List<StudentsCGPAModel>();
                        var newScheduleList = new List<InterviewSchedule>();
                        var isCompanyBusyForWHoleTime = false;

                        notScheduledStudentsIds = notScheduledStudentsIds.OrderBy(x => x.CGPA).ToList();

                        foreach (var studentItem in notScheduledStudentsIds)
                        {
                            //If company is busy 
                            if (isCompanyBusyForWHoleTime) 
                                continue;

                            //getting current student schedule
                            var studentSchedule = notScheduledStudentsScheduleList.Where(x => x.StudentId == studentItem.studentId);

                            //flag to control the do while loop
                            var loopContinue = false;
                            do
                            {
                                //checking is commpany busy on its start time or not (on its strt time or in schedule time ??)
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
                                        scheduleEndTime = scheduleEndTime.AddMinutes(10);

                                        //create a schedule object, as both parties are not busy
                                        var scd = new InterviewSchedule
                                        {
                                            StudentId = studentItem.studentId,
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

                                        remainingStudentsIds.Remove(studentItem);
                                    } 
                                    else
                                    {
                                        //this case will be excecuted, when student is busy on givn company start time of the schedule
                                        remainingStudentsIds.Add(studentItem);
                                        var tempList = remainingStudentsIds.Concat(notScheduledStudentsIds).Distinct().OrderBy(x => x.CGPA).ToList();
                                        notScheduledStudentsIds = tempList;

                                        if (notScheduledStudentsIds.Count == 1)
                                        {
                                            //start time change to next slot by adding 10 minutes in the start time
                                            companyStartTime = companyStartTime.Value.AddMinutes(10);

                                            //we have to check, is company end time is here or not.
                                            var companyEndTime = GetCompanyEndTimeByTimeSlot(company.TimeSlot ?? 0);

                                            if (companyStartTime.Value.TimeOfDay == companyEndTime.Value.TimeOfDay)
                                            {
                                                isCompanyBusyForWHoleTime = true;
                                                loopContinue = false;
                                            }
                                            else
                                            {
                                                //do while loop will continue to find next avaialble start time
                                                loopContinue = true;
                                            }
                                        }
                                        else
                                        {
                                            loopContinue = false;
                                        }
                                    } 
                                } 
                                else
                                {
                                    //This case will be executed when the company is busy on start time
                                    //start time change to next slot by adding 10 minutes in the start time
                                    companyStartTime = companyStartTime.Value.AddMinutes(10);

                                    //we have to check, is company end time is here or not.
                                    var companyEndTime = GetCompanyEndTimeByTimeSlot(company.TimeSlot ?? 0);

                                    if (companyStartTime.Value.TimeOfDay == companyEndTime.Value.TimeOfDay)
                                    {
                                        isCompanyBusyForWHoleTime = true;
                                        loopContinue = false;
                                    }
                                    else
                                    {
                                        //do while loop will continue to find next avaialble start time
                                        loopContinue = true;
                                    }
                                }

                            } while (loopContinue);
                        }

                        if (newScheduleList.Count > 0)
                        {
                            _scheduleRepository.AddSchedules(newScheduleList);
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

        private DateTime? GetCompanyEndTimeByTimeSlot(int timeSlot)
        {
            var today = DateTime.Now;
            var fivePm = new DateTime(today.Year, today.Month, today.Day, 17, 0, 0);
            switch (timeSlot)
            {
                case 1:
                    //9-12
                    var twelvePm = new DateTime(today.Year, today.Month, today.Day, 12, 0, 0);
                    return twelvePm;
                case 2:
                    //12-5
                    return fivePm;
                case 3:
                    //9-5
                    return fivePm;

                default: return null;
            }
        }
        private string isInterviewed(Boolean interviewed)
        {
            if (interviewed)
            {
                return "Interviewed";
            }
            else
            {
                return "not interviewed";
            }
        }
        private string isShortListed(Boolean shortListed)
        {
            if (shortListed)
            {
                return "ShortListed";
            }
            else
            {
                return "---";
            }
        }
        private int getNumberInQueue(int companyId, int studentId)
        {
            var companySchedule = _scheduleRepository.GetACompanySchedule(companyId);
            companySchedule.Sort((x, y) => x.StartTime.Value.TimeOfDay.CompareTo(y.StartTime.Value.TimeOfDay));
            var studentIndex = companySchedule.FindIndex(x => x.StudentId == studentId);
            ++studentIndex;
            return studentIndex;
        }
    }
}
