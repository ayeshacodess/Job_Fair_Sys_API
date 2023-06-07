using System;

namespace Job_Fair_Sys_API.Models
{
    public class ScheduleViewModel
    {
        public int id { get; set; }
        public int studentId { get; set; }
        public string studentName { get; set; }
        public string aridNumber { get; set; }
        public int companyId { get; set; }
        public string compnayName { get; set; }
        public int createorId { get; set; }
        public string  creatorRole { get; set; }
        public System.DateTime date { get; set; }
        public Nullable<System.DateTime> startTime { get; set; }
        public Nullable<System.DateTime> endTime { get; set; }
        public bool interviewed { get; set; }
        public string description { get; set; }
        public string allocatedRoom { get; set; }
        public bool isShortListed { get; set; } = false;

    }
}