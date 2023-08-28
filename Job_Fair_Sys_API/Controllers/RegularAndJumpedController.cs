using Job_Fair_Sys_API.Models;
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
    public class RegularAndJumpedController : ApiController
    {
        private readonly CompanyRepository _companyRespository;
        public RegularAndJumpedController()
        {
            _companyRespository = new CompanyRepository();
        }


        [HttpGet]
        [Route("api/companiesAndNoOfRegularAndJumpedStudents")]
        public HttpResponseMessage Get()
        {
            try
            {
                var companies = _companyRespository.DbContext.Companies.Where(x => x.Status == "Accept").ToList();
                var companiesAndRegularAndJumpedStudents = new List<RegularAndJumpedViewModel>();
                foreach (var company in companies)
                {
                    var jumpedStudentsInCompany = _companyRespository.DbContext.InterviewSchedules.Count(x => x.CompanyId == company.Id && x.IsJumped != null && x.IsJumped == true);
                    var regularStudentsInCompany = _companyRespository.DbContext.InterviewSchedules.Count(x => x.CompanyId == company.Id && (x.IsJumped == null || x.IsJumped == false));
                    var obj = new RegularAndJumpedViewModel
                    {
                        id = company.Id,
                        companyName = company.Name,
                        JumpedStudentsInterviews = jumpedStudentsInCompany,
                        regularInterviews = regularStudentsInCompany,
                    };
                    companiesAndRegularAndJumpedStudents.Add(obj);
                }
                return Request.CreateResponse(HttpStatusCode.OK, companiesAndRegularAndJumpedStudents);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/RegularStudents")]
        public HttpResponseMessage GetRegularStudents(int companyId)
        {
            try
            {

                var companySchedule = _companyRespository.DbContext.InterviewSchedules.Where(x => x.CompanyId == companyId && (x.IsJumped == null || x.IsJumped == false)).ToList();
                var regularStudents = new List<StudentViewModel>();
                foreach (var st in companySchedule)
                {
                    var student = st.Student;
                    var viewModelTypeStudent = StudentViewModel.ToViewModel(student);
                    regularStudents.Add(viewModelTypeStudent);
                }
              
                return Request.CreateResponse(HttpStatusCode.OK, regularStudents);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/JumpedStudents")]
        public HttpResponseMessage GetJumpedStudents(int companyId)
        {
            try
            {

                var companySchedule = _companyRespository.DbContext.InterviewSchedules.Where(x => x.CompanyId == companyId && x.IsJumped != null && x.IsJumped == true).ToList();
                var jumpedStudents = new List<StudentViewModel>();
                foreach (var st in companySchedule)
                {
                    var student = st.Student;
                    var viewModelTypeStudent = StudentViewModel.ToViewModel(student);
                    jumpedStudents.Add(viewModelTypeStudent);
                }

                return Request.CreateResponse(HttpStatusCode.OK, jumpedStudents);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
