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
    }
}
