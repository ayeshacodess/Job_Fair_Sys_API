using Job_Fair_Sys_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Fair_Sys_API.Models
{
    public class RegularAndJumpedViewModel
    {
        public Boolean isJumped { get; set; }

        //public static RegularAndJumpedViewModel ToViewModel(StudentSelectedCompany s)
        //{
        //    var model = new RegularAndJumpedViewModel
        //    {
        //        isJumped = s.IsJumped;
        //    };

        //    return model;
        //}

    }
}