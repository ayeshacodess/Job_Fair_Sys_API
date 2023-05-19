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
    public class AccountController : ApiController
    {
        private readonly AccountRepository _accountRepository;

        public AccountController()
        {
            _accountRepository = new AccountRepository();
        }

        [HttpGet]
        [Route("api/account/login")]
        public HttpResponseMessage Login(string username, string password)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Email or Password both are required");
                }

                var dbUser = _accountRepository.IsAuthorizedUser(username, password);
                

                if(dbUser is null)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "User not found");
                }

                 var user = UserViewModel.ToViewModel(dbUser);

                if (!string.IsNullOrWhiteSpace(user.role) && user.role.Equals("Student")) 
                {
                    var dbStudent = _accountRepository.DbContext.Students.FirstOrDefault(x => x.AridNumber == user.username);
                    if (dbStudent != null)
                    {
                        user.userProfileId = dbStudent.Id;
                        user.name = dbStudent.Name;
                        user.cgpa = dbStudent.CGPA != null ?  dbStudent.CGPA.ToString() : "0.00"; //have to take from db
                        user.aridNumber = dbStudent.AridNumber;
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
