using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Fair_Sys_API.Models
{
    public class InterviewedAndShortListedViewModel
    {
        public string companyName { get; set; }
        public string aridNumber { get; set; }
        public string studentName { get; set; }
        public string interviewed { get; set; }
        public string shortListed { get; set; }
    }
}