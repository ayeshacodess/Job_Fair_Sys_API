using Job_Fair_Sys_API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Job_Fair_Sys_Data.Repositories;
using Job_Fair_Sys_Data;

namespace Job_Fair_Sys_API.Controllers
{
    public class PassController : ApiController
    {
        private readonly PassRepository _passRepository;
        public PassController()
        {

            _passRepository = new PassRepository();
            
        }
        [HttpPost]
        [Route("api/set/teerPasses")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> AddTeerPassesAsync()
        {
            var httpRequest = HttpContext.Current.Request;

            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
            {
                try
                {
                    string requestBody = reader.ReadToEnd();

                    var passes = JsonConvert.DeserializeObject<TeerPassesViewModel>(requestBody);

                    var dbPasses = _passRepository.DbContext.Passes.FirstOrDefault();

                    if (dbPasses == null)
                    {

                        var p = new Pass
                        {
                            level1 = passes.level1,
                            level2 = passes.level2,
                        };
                        _passRepository.DbContext.Passes.Add(p);
                        _passRepository.DbContext.SaveChanges();
                    }
                    else
                    {
                        dbPasses.level1 = passes.level1;
                        dbPasses.level2 = passes.level2; 

                        _passRepository.DbContext.Entry(dbPasses).CurrentValues.SetValues(dbPasses);
                        _passRepository.DbContext.SaveChanges();
                    }
                   
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }
    }
}
