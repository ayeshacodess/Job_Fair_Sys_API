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
        public List<SocietyMember> DeleteMember(SocietyMember member) //opps member ka account b del krna hoga, wo kr lungi but frontend se debugger q nai
            ////wi8 na yaar krny to den phly okkkk, iska bana k rakha tha pr mila nai mje #mess ab thk ha? g
        {
            try
            {
                var dbmember = _dbContext.SocietyMembers.FirstOrDefault(c => c.Id == member.Id);
                _dbContext.SocietyMembers.Remove(dbmember);
                var user = _dbContext.Users.Where(u => u.Username == member.Email).FirstOrDefault();
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
                return _dbContext.SocietyMembers.ToList();
            }
            catch (Exception)
            {
               
                throw;
            }
           
        }
    }
}
