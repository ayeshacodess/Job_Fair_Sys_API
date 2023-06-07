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
using System.IO;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Job_Fair_Sys_API.Controllers
{
    public class SkillController : ApiController
    {
        private readonly SkillRepository _skillRepository;
        public SkillController()
        {
            _skillRepository = new SkillRepository();
        }

        [Route("api/skill/get")]
        public HttpResponseMessage Get()
        {
            try
            {
                var skills = GetSkillsFromDb();
                return Request.CreateResponse(HttpStatusCode.OK, skills);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("api/skill/addupdate")]
        [HttpPost]
        public async Task<HttpResponseMessage> AddSkill()
        {
            using (var reader = new StreamReader(await Request.Content.ReadAsStreamAsync()))
            {
                try
                {
                    string requestBody = reader.ReadToEnd();
                    var skill = JsonConvert.DeserializeObject<Skill>(requestBody);
                    
                    if (skill.Id > 0)
                    {
                        var skillToUpdate = _skillRepository.Get(skill.Id);
                        if (skillToUpdate != null)
                        {
                            skillToUpdate.Technology = skill.Technology;
                            _skillRepository.Update(skillToUpdate);
                        }
                    }
                    else 
                    {
                        _skillRepository.Add(skill);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        [HttpGet]
        [Route("api/skill/delete")]
        public HttpResponseMessage Delete(int skill_Id)
        {
            try
            {
                var skills = _skillRepository.DeleteSkill(skill_Id);
                var updatedSKill = GetSkillsFromDb();
                return Request.CreateResponse(HttpStatusCode.OK, updatedSKill);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<SkillViewModel> GetSkillsFromDb() 
        {
            var dbSkills = _skillRepository.GetSkills();
            var skills = new List<SkillViewModel>();

            foreach (var item in dbSkills)
            {
                skills.Add(SkillViewModel.ToViewModel(item));
            }

            return skills;
        }

        [Route("api/studentSkills/get")]
        public HttpResponseMessage GetStudentSkill(string aridno)
        {
            try
            {
                var skills = _skillRepository.GetStudentSkillsFromDb(aridno);

                var response = new List<SkillViewModel>();

                foreach (var item in skills)
                {
                    response.Add(SkillViewModel.ToViewModel(item.Skill));
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
