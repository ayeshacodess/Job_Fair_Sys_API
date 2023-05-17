using Job_Fair_Sys_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Fair_Sys_API.Models
{
    public class SkillViewModel
    {
        public int id { get; set; }
        public string technology { get; set; }

        public Skill ToEntity()
        {
            var entity = new Skill
            {
                Id = this.id,
                Technology = this.technology
            };

            return entity;
        }

        public static SkillViewModel ToViewModel(Skill skill)
        {
            var model = new SkillViewModel
            {
                id = skill.Id,
                technology = skill.Technology
            };

            return model;
        }
    }
}