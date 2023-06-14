using Job_Fair_Sys_API.Models;
using Job_Fair_Sys_Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Job_Fair_Sys_API.Controllers
{
    public class SummaryController : ApiController
    {
        private readonly ScheduleRepository _scheduleRepository;

        public SummaryController()
        {
            _scheduleRepository = new ScheduleRepository();
        }

        [HttpGet]
        [Route("api/summary/exective")]
        public HttpResponseMessage GetExectiveSummary()
        {
            try
            {
                var result = new ExectiveSummaryViewModel();

                var schedule = _scheduleRepository.GetSchedules();

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
                            percentile = SharedHelper.Utility.GetStudentPercentile(student.CGPA ?? 0),
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

                        models.Add(model);
                    }
                }

                result.numberOfCompanies = models.GroupBy(x => x.companyId).Count();
                result.numberOfInterviews = models.Count(x => x.interviewed);
                result.totalInterviewedStudents = models.Where(x => x.interviewed).GroupBy(x => x.studentId).Count();
                result.totalShortlistStudents = models.Where(x => x.isShortListed).GroupBy(x => x.studentId).Count();
                result.firstTearCount = models.Where(x => x.percentile == "First Teer" && x.interviewed).GroupBy(x => x.studentId).Count();
                result.secondTearCount = models.Where(x => x.percentile == "Second Teer" && x.interviewed).GroupBy(x => x.studentId).Count();
                result.thirdTearCount = models.Where(x => x.percentile == "Third Teer" && x.interviewed).GroupBy(x => x.studentId).Count();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/summary/companies")]
        public HttpResponseMessage GetCompaniesSummary()
        {
            try
            {
                var result = new List<CompanySummaryViewModel>();

                var schedule = _scheduleRepository.GetSchedules();

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
                            percentile = SharedHelper.Utility.GetStudentPercentile(student.CGPA ?? 0),
                            cgpa = student.CGPA ?? 0,
                            studentId = student.Id,
                            studentName = student.Name,
                            aridNumber = student.AridNumber,
                            companyId = company.Id,
                            compnayName = company.Name,
                            companyRate = company.rate,
                            date = item.Date,
                            startTime = item.StartTime,
                            endTime = item.EndTime,
                            interviewed = item.Interviewed,
                            description = item.Description,
                            allocatedRoom = item.AllocatedRoom,
                            isShortListed = item.IsShortListed ?? false
                        };

                        models.Add(model);
                    }
                }

                var companiesData = models.GroupBy(x => x.companyId).ToList();

                foreach (var item in companiesData)
                {
                    var com = item.Where(x => x.interviewed).FirstOrDefault();
                    var res = new CompanySummaryViewModel
                    {
                        companyId = com?.companyId ?? 0,
                        companyName = com?.compnayName,
                        companyRate = com?.companyRate ?? 0,
                        totalInterviews = item.Count(x => x.interviewed),
                        totalShortlisted = item.Count(x => x.isShortListed),
                        teerAvg = getAvgTeer(item)
                    };
                    result.Add(res);
                }

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private string getAvgTeer(IGrouping<int, ScheduleViewModel> cgpas)
        {
            var students = cgpas.GroupBy(x => x.studentId).ToList();
            
            var sum = 0.00;
            var totalstudents = students.Count();

            foreach (var item in students)
            {
                sum += item.FirstOrDefault()?.cgpa ?? 0.00;
            }

            var avg = sum / totalstudents;

            return SharedHelper.Utility.GetStudentPercentile(avg);
        }
    }
}
