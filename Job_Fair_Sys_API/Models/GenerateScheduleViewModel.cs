﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Fair_Sys_API.Models
{
    public class GenerateScheduleViewModel
    {
        public int UserProfile { get; set; }
        public int selectedCompany { get; set; }
        public string allocatedRoom { get; set; }
        public int timeDuration { get; set; }
        public string role { get; set; }
    }
}