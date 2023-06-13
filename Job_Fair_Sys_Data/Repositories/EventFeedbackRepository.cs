using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Fair_Sys_Data.Repositories
{
    public class EventFeedbackRepository : BaseRepository<EventFeedback>
    {
        public EventFeedbackRepository() : base()
        {
            
        }

        public List<EventFeedback> GetEventFeedbacks()
        {
            var eventFeedback = _dbContext.EventFeedbacks.ToList();
            return eventFeedback;
        }

        public EventFeedback Add(EventFeedback feedback)
        {
            var eventFeedback = _dbContext.EventFeedbacks.Add(feedback);
            _dbContext.SaveChanges();
            return eventFeedback;
        }

        public EventFeedback GetCompanyFeedback(int id)
        {
            var feedback = _dbContext.EventFeedbacks.FirstOrDefault(x => x.CompanyId == id);
            return feedback;
        }
    }
}
