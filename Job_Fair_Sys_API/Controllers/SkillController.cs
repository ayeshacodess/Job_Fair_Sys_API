using Job_Fair_Sys_Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Job_Fair_Sys_Data;
using Job_Fair_Sys_API.Models;
using Microsoft.Ajax.Utilities;

namespace Job_Fair_Sys_API.Controllers
{
    public class SkillController : ApiController
    {
        private readonly SkillRepository _skillRepository;
        public SkillController()
        {
            _skillRepository = new SkillRepository();
        }

        public HttpResponseMessage Get()
        {
            try
            {
                var dbSkills = _skillRepository.GetSkills();
                var skills = new List<SkillViewModel>();

                foreach (var item in dbSkills)
                {
                    skills.Add(SkillViewModel.ToViewModel(item));
                }

                return Request.CreateResponse(HttpStatusCode.OK, skills);
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage AddSkill(Skill skill)
        {
            try
            {
                var add_skill = _skillRepository.Add(skill);
                return Request.CreateResponse(HttpStatusCode.OK, add_skill);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
