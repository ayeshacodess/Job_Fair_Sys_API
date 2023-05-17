﻿using System.Collections.Generic;
using System.Linq;

namespace Job_Fair_Sys_Data.Repositories
{
    public class SkillRepository : BaseRepository<Skill>
    {
        public SkillRepository() : base()
        {            
        }

        public List<Skill> GetSkills()
        {
            var skills = _dbContext.Skills.ToList();
            return skills;
        }

        public int Add(Skill skill)
        {
            _dbContext.Skills.Add(skill);
            var result = _dbContext.SaveChanges(); //saveChanges() return number of rows
            return result;
        }

        public bool Update(Skill skill)
        {
            var dbSkill = _dbContext.Skills.Find(skill.Id);
            _dbContext.Entry(dbSkill).CurrentValues.SetValues(skill);
            return _dbContext.SaveChanges() > 0;
        }
    }
}
