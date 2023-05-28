using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Fair_Sys_Data.Repositories
{
    public class StudentRepository : BaseRepository<Student>
    {
        public StudentRepository() : base()
        {
        }

        public List<Student> GetStudents()
        {
            var students = _dbContext.Students.Where(x => x.IsCVUploaded == true).ToList();
            return students;
        }

        public Student GetStudent(int id)
        {
            var student = _dbContext.Students.Find(id);
            return student;
        }
        public Student GetStudentByAridNo(string aridNo)
        {
            var student = _dbContext.Students.FirstOrDefault(x => x.AridNumber == aridNo);
            return student;
        }

        public Student AddStudent(Student student)
        {
           var newStudent = _dbContext.Students.Add(student);
            _dbContext.SaveChanges();
            return newStudent;
        }

        public string DeleteStudent(Student student)
        {
            try
            {
                User stdUser = _dbContext.Users.FirstOrDefault(s => s.Username == student.AridNumber);
                _dbContext.Users.Remove(stdUser);
                var stdSkills = _dbContext.StudentSkills.Where(s => s.StudentId == student.Id);
                foreach (var item in stdSkills)
                {
                    _dbContext.StudentSkills.Remove(item);

                }
                var studentSelectedCompanies = _dbContext.StudentSelectedCompanies.Where(x => x.Student_Id == student.Id);
                    foreach (var item in studentSelectedCompanies)
                {
                    _dbContext.StudentSelectedCompanies.Remove(item);
                }
                _dbContext.Students.Remove(student);
                _dbContext.SaveChanges();
                return "Successfully Deleted!";
                //var stud = _dbContext.Students.Where(l=>l.IsCVUploaded==true).FirstOrDefault();
                //return stud;
            }
            catch (Exception ex)
            {
                return ex.Message;
                //throw;
            }
          
            
        }
    }
}
