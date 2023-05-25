using Job_Fair_Sys_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Fair_Sys_API.Models
{
    public class ScheduleViewModel
    {
        public int studentId { get; set; }
        public int companyId { get; set; }
        public int societyMemberId { get; set; }
        public System.DateTime date { get; set; }
        public int timeDuration { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public bool interviewed { get; set; }
        public string description { get; set; }
        public string allocatedRoom { get; set; }

        //public InterviewSchedule ToEntity()
        //{
        //    List<InterviewSchedule> schedule = new List<InterviewSchedule>();
        //    schedule.Add(new InterviewSchedule
        //    {
        //        StudentId = this.studentId,
        //        CompanyId = this.companyId,
        //        SocietyMemberId = this.societyMemberId,
        //        Date = this.date,
        //        TimeDuration = this.timeDuration,
        //        StartTime = this.startTime,
        //        EndTime = this.endTime,
        //        AllocatedRoom = this.allocatedRoom,

        //    });
        //    return schedule;
        //}
    }
}