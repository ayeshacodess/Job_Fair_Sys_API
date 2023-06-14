using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Fair_Sys_API.Models
{
    public class ExectiveSummaryViewModel
    {
        public int numberOfCompanies { get; set; }
        public int numberOfInterviews { get; set; }
        public int totalInterviewedStudents { get; set; }
        public int totalShortlistStudents { get; set; }
        public int firstTearCount { get; set; }
        public int secondTearCount { get; set; }
        public int thirdTearCount { get; set; }
    }
}