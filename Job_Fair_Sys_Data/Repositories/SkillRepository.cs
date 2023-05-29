using System.Collections.Generic;
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

        public Skill Get(int id)
        {
            return _dbContext.Skills.Find(id);
        }

        public bool Update(Skill skill)
        {
            var dbSkill = _dbContext.Skills.Find(skill.Id);
            _dbContext.Entry(dbSkill).CurrentValues.SetValues(skill);
            return _dbContext.SaveChanges() > 0;
        }

        public bool DeleteSkill(int id)
        {
            var dbSkill = _dbContext.Skills.Find(id);
            _dbContext.Skills.Remove(dbSkill);
            return _dbContext.SaveChanges() > 0;
        }
    }
}
