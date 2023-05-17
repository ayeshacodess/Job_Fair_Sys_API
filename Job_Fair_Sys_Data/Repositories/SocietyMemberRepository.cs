using System;
using System.Collections.Generic;
using System.Linq;


namespace Job_Fair_Sys_Data.Repositories
{
    public class SocietyMemberRepository : BaseRepository<SocietyMember>
    {
        public SocietyMemberRepository() : base()
        {

        }

        public List<SocietyMember> GetSocietyMembers()
        {
            var members = _dbContext.SocietyMembers.ToList();
            return members;
        }

        public SocietyMember GetMember(int id)
        {

            return _dbContext.SocietyMembers.Find(id);
        }

        public void Add(SocietyMember member)
        {
            _dbContext.SocietyMembers.Add(member);
            _dbContext.SaveChanges();
        }

        public int Update(SocietyMember member)
        {
            var dbmember = _dbContext.SocietyMembers.FirstOrDefault(c => c.Id == member.Id);

            _dbContext.Entry(dbmember).CurrentValues.SetValues(member);

            return _dbContext.SaveChanges();
        }
    }
}
