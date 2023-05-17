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
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Job_Fair_Sys_API.Controllers
{
    public class StudentController : ApiController
    {
        private readonly StudentRepository _studentRepository;
        public StudentController()
        {
            _studentRepository = new StudentRepository();
        }

        [HttpPost]
        public HttpResponseMessage Add(Student student)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Not a valid model");

                var newStudent = _studentRepository.AddStudent(student);
                return Request.CreateResponse(HttpStatusCode.OK, newStudent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/students")]
        public HttpResponseMessage All()
        {
            try
            {
                var students = _studentRepository.GetStudents();
                var studentModels = new List<StudentViewModel>();
                foreach (var item in students)
                {
                    
                    var viewModelTypeStudent = StudentViewModel.ToViewModel(item); //convert a company into CompanyViewModel type
                    studentModels.Add(viewModelTypeStudent);
                }
                return Request.CreateResponse(HttpStatusCode.OK, studentModels);
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public HttpResponseMessage Get(int id)
        {
            try
            {
                var student = _studentRepository.GetStudent(id);
                if(student is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                return Request.CreateResponse(HttpStatusCode.OK, student);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/student/uploadcv")]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<HttpResponseMessage>  UpLoadCV()
        {
            var httpRequest = HttpContext.Current.Request;
            //var body = await Request.Content.ReadAsStringAsync();
            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
            {
                string requestBody = reader.ReadToEnd();

                var reqData = JsonConvert.DeserializeObject<StudentViewModel>(requestBody);

                // Do something with the request body
            }

            HttpResponseMessage result = null;
            
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    docfiles.Add(filePath);
                }
                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);


            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }
    }
}
