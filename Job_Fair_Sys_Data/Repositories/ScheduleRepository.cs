using System;
using System.Collections.Generic;
using System.Linq;

namespace Job_Fair_Sys_Data.Repositories
{
    public class ScheduleRepository : BaseRepository<InterviewSchedule>
    {
        public ScheduleRepository() : base()
        {
        }

        public List<InterviewSchedule> GetInterviewedAndShortListedStudents()
        {
            var schedule = _dbContext.InterviewSchedules.Where(x => x.IsShortListed ?? false  || x.Interviewed ).ToList();
            return schedule;
        }
        public List<InterviewSchedule> GetAStudentSchedule(int id)
        {
            var studentSchedule = _dbContext.InterviewSchedules.Where(x => x.StudentId == id).ToList();
            return studentSchedule;
        }

        public List<InterviewSchedule> GetACompanySchedule(int id)
        {
            var companySchedule = _dbContext.InterviewSchedules.Where(x => x.CompanyId == id).ToList();
            return companySchedule;
        }
        public List<InterviewSchedule> GetSchedules()
        {
            var scd = _dbContext.InterviewSchedules.ToList();
            return scd;
        }

        public List<StudentSelectedCompany> GetStudents(int companyId)
        {
            var students = _dbContext.StudentSelectedCompanies.Where(x =>  x.Company_Id == companyId).ToList();
            return students;
        }

        public void AddSchedules(List<InterviewSchedule> schedule)
        {
            foreach (var item in schedule)
            {
                _dbContext.InterviewSchedules.Add(item);
            }
            _dbContext.SaveChanges();
        }

        public List<InterviewSchedule> GetStudentsByInterviewSchedule(int userId)
        {
            var students = _dbContext.InterviewSchedules.Where(x => x.SocietyMemberId == userId).ToList();
            return students;
        }

        public void GetRecordAndAddShortList(bool isSHortlIst, int studentId, int companyId, int scheduleid)
        {
            var record = _dbContext.InterviewSchedules.FirstOrDefault(x => x.StudentId == studentId && x.CompanyId == companyId && x.Id == scheduleid);
            if (record != null)
            {
                record.IsShortListed = isSHortlIst;
                _dbContext.SaveChanges();
            }
        }

        public void GetRecordAndAddInterviewed(bool isInterviewed, int studentId, int companyId, int scheduleid)
        {
            var record = _dbContext.InterviewSchedules.FirstOrDefault(x => x.StudentId == studentId && x.CompanyId == companyId && x.Id == scheduleid);
            if (record != null)
            {
                record.Interviewed = isInterviewed;
                _dbContext.SaveChanges();
            }
        }
    }
}