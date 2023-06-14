using Job_Fair_Sys_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Fair_Sys_API.Models
{
    public class StudentFeedbackViewModel
    {
        
        public Nullable<int> studentId { get; set; }
        public Nullable<int> companyId { get; set; }
        public string companyName { get; set; }
        public List<FeedbackViewMOdel> stdFeedback { get; set; }

        public StudentFeedbackViewModel()
        {
            stdFeedback = new List<FeedbackViewMOdel>();
        }

    }

    public class FeedbackViewMOdel
    {
        public int rate { get; set; }
        public int skill_ld { get; set; }
        public string skillName { get; set; }
    }
}