using Job_Fair_Sys_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Fair_Sys_API.Models
{
    public class RegularAndJumpedViewModel
    {
        public int id { get; set; }
        public string companyName { get; set; }
        public int JumpedStudentsInterviews { get; set; }
        public int regularInterviews { get; set; }

    //    public static RegularAndJumpedViewModel ToViewModel(InterviewSchedule s)
    //    {
    //        var model = new RegularAndJumpedViewModel
    //        {
    //            companyName = s.Company.Name;
    //        totalJumpedStudents = s.
    //        regularInterviews = 
    //};

    //        return model;
    //    }

}
}