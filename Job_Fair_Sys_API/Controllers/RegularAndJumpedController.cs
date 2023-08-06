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
    public class RegularAndJumpedController : ApiController
    {
        private readonly CompanyRepository _companyRespository;
        public RegularAndJumpedController()
        {
            _companyRespository = new CompanyRepository();
        }


        [HttpGet]
        [Route("api/companiesRegAndJump")]
        public HttpResponseMessage Get(string role, int userId)
        {
            try
            {
                var st = _companyRespository.DbContext.StudentSelectedCompanies.Where(x => x.Student_Id == userId).ToList();
                var itm = new List<RegularAndJumpedViewModel>();
                foreach (var item in st)
                {
                    //RegularAndJumpedViewModel.ToViewModel(item);
                }
                return Request.CreateResponse(HttpStatusCode.OK, itm);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        }
}
