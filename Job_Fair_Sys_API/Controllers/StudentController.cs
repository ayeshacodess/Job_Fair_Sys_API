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

        //[HttpPost]
        //[Route("api/set/teerPasses")]
        //public async System.Threading.Tasks.Task<HttpResponseMessage> AddTeerPassesAsync()
        //{
        //    var httpRequest = HttpContext.Current.Request;

        //    using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
        //    {
        //        try
        //        {
        //            string requestBody = reader.ReadToEnd();

        //            var passes = JsonConvert.DeserializeObject<TeerPassesViewModel>(requestBody);
        //            var students = _studentRepository.GetStudents();
        //            var teer1Std = students.Where(x => x.CGPA >= 3.5);

        //            return Request.CreateResponse(HttpStatusCode.OK);
        //        }
        //        catch (Exception ex)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
        //        }
        //    }
        //}
        
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
                var students = _studentRepository.GetStudents().Take(50).ToList();
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

        [HttpGet]
        [Route("api/student/delete")]
        public HttpResponseMessage Delete(string status, int studentId)
        {
            try
            {
                var student = _studentRepository.GetStudent(studentId);
                if(student is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                var List=_studentRepository.DeleteStudent(student);
                return Request.CreateResponse(HttpStatusCode.OK, List);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

       // uploadCVUsingUpdateFunction
        [HttpPost]
        [Route("api/student/uploadcv")]
        public async Task<HttpResponseMessage>  UpLoadCV()
        {
            var httpRequest = HttpContext.Current.Request;

            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
            {
                try
                {
                    var content = httpRequest.Params.Get("body");
                    var reqData = JsonConvert.DeserializeObject<StudentViewModel>(content);

                    var dbStd = _studentRepository.DbContext.Students.FirstOrDefault(x => x.AridNumber == reqData.aridNumber);
                   
                    if (dbStd != null)
                    {
                        dbStd.StudyStatus = reqData.studyStatus;
                        dbStd.Contact1 = reqData.contact1;
                        dbStd.Contact2 = reqData.contact2;
                        dbStd.FypTech = reqData.FypTech;
                        dbStd.FypTitle = reqData.FypTitle;
                        dbStd.HasFYP = reqData.hasFYP;
                        dbStd.IsCVUploaded = true;
                        if(dbStd.CGPA > 3.5)
                        {
                            dbStd.noOfJumps = 6;
                            dbStd.isAllowedJumps = true;
                        }
                        if(dbStd.CGPA >= 3 && dbStd.CGPA <= 3.5)
                        {
                            dbStd.noOfJumps = 4;
                            dbStd.isAllowedJumps = true;
                        }

                        //variable to hold the CV path to save in DB
                        string cvpath = string.Empty;
                        if (httpRequest.Files.Count > 0)
                        {
                            //if any file attached then get the first one 
                            var studentCv = httpRequest.Files[0];

                            //directory name where we will save the CVs
                            var directoryName = "Student-CVs";

                            //relative directory path
                            var directoryPath = HttpContext.Current.Server.MapPath($"~/{directoryName}");

                            //check is directory exist on given path
                            var isDirExist = Directory.Exists(directoryPath);
                            if (!isDirExist) {
                                //directory does not exist, so create a new one
                                Directory.CreateDirectory(directoryPath);
                            }

                            //map the student file with directory
                            var fileNameWithPath = $"~/{directoryName}/{studentCv.FileName}";

                            //set the relative path for student file
                            var filePath = HttpContext.Current.Server.MapPath(fileNameWithPath);

                            //save the file on given path
                            studentCv.SaveAs(filePath);

                            //set the relative path for cv to store in DB
                            cvpath = fileNameWithPath;
                        }

                        dbStd.CVPath = cvpath;

                        _studentRepository.DbContext.Entry(dbStd).CurrentValues.SetValues(dbStd);
                        _studentRepository.DbContext.SaveChanges();

                        var studentSkills = _studentRepository.DbContext.StudentSkills.Where(x => x.StudentId == dbStd.Id);
                        foreach (var item in studentSkills)
                        {
                            _studentRepository.DbContext.StudentSkills.Remove(item);
                        }

                        var skills = _studentRepository.DbContext.Skills.ToList();

                        foreach (var skill in reqData.studentSkills)
                        {
                            var skl = skills.FirstOrDefault(x => x.Id == skill.skill_Id);

                            if (skl != null) {
                                var newRequiredSkill = new StudentSkill
                                {
                                    Level_Id = skill.level_Id,
                                    Skill_Id = skl.Id,
                                    StudentId = dbStd.Id
                                };

                                _studentRepository.DbContext.StudentSkills.Add(newRequiredSkill);
                            }
                        }

                        _studentRepository.DbContext.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, dbStd);
                    }

                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, "No record found.");
            }

            //HttpResponseMessage result = null;
            
            //if (httpRequest.Files.Count > 0)
            //{
            //    var docfiles = new List<string>();
            //    foreach (string file in httpRequest.Files)
            //    {
            //        var postedFile = httpRequest.Files[file];
            //        var filePath = HttpContext.Current.Server.MapPath("~/CV" + postedFile.FileName);
            //        postedFile.SaveAs(filePath);
            //        docfiles.Add(filePath);
            //    }
            //    result = Request.CreateResponse(HttpStatusCode.Created, docfiles);


            //}
            //else
            //{
            //    result = Request.CreateResponse(HttpStatusCode.BadRequest);
            //}
            //return result;
        }
    }
}
