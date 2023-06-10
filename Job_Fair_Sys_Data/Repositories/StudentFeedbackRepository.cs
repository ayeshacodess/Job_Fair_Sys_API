using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Fair_Sys_Data.Repositories
{
   public class StudentFeedbackRepository : BaseRepository<StudentsFeedback>
    {
        public StudentFeedbackRepository() : base()
        {
        }

        public void AddFeedback(List<StudentsFeedback> feedback)
        {
            foreach (var item in feedback)
            {
                var dbFeedback = _dbContext.StudentsFeedbacks.FirstOrDefault(x => x.Skill_ld == item.Skill_ld && x.StudentId == item.StudentId && x.CompanyId == item.CompanyId);
                if(dbFeedback == null)
                    _dbContext.StudentsFeedbacks.Add(item);
                else
                {
                    dbFeedback.rate = item.rate;
                    _dbContext.Entry(dbFeedback).CurrentValues.SetValues(dbFeedback);
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
