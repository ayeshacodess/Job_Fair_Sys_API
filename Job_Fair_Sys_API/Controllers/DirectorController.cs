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
    public class DirectorController : ApiController
    {
        private readonly CompanyRepository _companyRespository;
        private readonly StudentSelectedCompanyRepository _studentSelectedCompanyRepository;
        private readonly AccountRepository _accountRepository;
        private readonly StudentRepository _studentRepository;
        private readonly DirectorRepository _directorRepository;
        public DirectorController()
        {
            _studentSelectedCompanyRepository = new StudentSelectedCompanyRepository();
            _companyRespository = new CompanyRepository();
            _accountRepository = new AccountRepository();
            _studentRepository = new StudentRepository();
            _directorRepository = new DirectorRepository();
        }

        [Route("api/AcceptedCompanies")]
        [HttpGet]
        public HttpResponseMessage GetCompaniesForDirector()
        {
            try
            {
                var companies = _companyRespository.GetCompanies();
                var acceptedComp = companies.Where(x => x.Status == "Accept").ToList();
                var acceptedCompanies = new List<CompanyViewModel>();
                foreach (var item in acceptedComp)
                {
                    acceptedCompanies.Add(CompanyViewModel.ToViewModel(item));
                }
                return Request.CreateResponse(HttpStatusCode.OK, acceptedCompanies);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("api/StudentsInterviewedInCompany")]
        [HttpGet]
        public HttpResponseMessage StudentsInterviewed(int companyId)
        {
            try
            {
                var schedule = _companyRespository.DbContext.InterviewSchedules.Where(x => x.CompanyId == companyId && x.Interviewed).ToList();
                var stds = schedule.Select(x => x.Student).ToList();
                var students = new List<StudentViewModel>();
                foreach (var item in stds)
                {
                    students.Add(StudentViewModel.ToViewModel(item));
                }
                return Request.CreateResponse(HttpStatusCode.OK, students);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //[Route("api/StudentsInterviewedInCompany")]
        //[HttpGet]
        //public HttpResponseMessage CompanyStudents(int companyId)
        //{
        //    try
        //    {
        //        var students = _directorRepository.GetCompanyStudents(companyId);
        //        var stdList = new List<StudentViewModel>();
        //        foreach (var student in students)
        //        {
        //            stdList.Add(StudentViewModel.ToViewModel(student));
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, stdList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}

        [HttpGet]
        [Route("api/GetStudent")]
        public HttpResponseMessage StdGet(int studentId)
        {
            try
            {
                var std = _studentRepository.GetStudent(studentId);
                if (std is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                var student = StudentViewModel.ToViewModel(std);
                return Request.CreateResponse(HttpStatusCode.OK, student);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    } 
}
