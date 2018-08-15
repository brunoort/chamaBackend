using System.Collections.Generic;
using System.Linq;
using Chama.WebApi.ContextDatabase;
using Chama.WebApi.Models;

namespace Chama.WebApi.Repositories
{
    public interface ICoursesRepository
    {
        IEnumerable<CourseModel> Get();
        CourseModel GetById(int id);
        void Register(CourseModel model);
        void SignUp(CourseModel course, UserModel student);
        void Process();
        List<ProcessedCourseModel> GetProcessedCourses();
    }

    public class CoursesRepository : ICoursesRepository
    {
        private readonly ChamaContext _context;

        public CoursesRepository(ChamaContext context)
        {
            _context = context;
        }       

        public void Register(CourseModel model)
        {
            _context.Courses.Add(model);
            _context.SaveChanges();
        }

        public IEnumerable<CourseModel> Get()
        {
            return (from course in _context.Courses
                    join teacher in _context.Users on course.TeacherId equals teacher.Id
                    select new CourseModel()
                    {
                        Id = course.Id,
                        Name = course.Name,
                        MaxStudents = course.MaxStudents,
                        TeacherId = course.TeacherId,
                        Teacher = teacher,
                        Students = _context.StudentByCourse.Where(s => s.CourseId == course.Id).Select(s => s.Student).ToList()
                    });
        }

        public CourseModel GetById(int id)
        {
            return Get().Where(w => w.Id == id).FirstOrDefault();
        }

        public void SignUp(CourseModel course, UserModel student)
        {      
            var studentByCourseModel = new StudentByCourseModel();

            studentByCourseModel.CourseId = course.Id;
            studentByCourseModel.StudentId = student.Id;

            _context.StudentByCourse.Attach(studentByCourseModel);
            _context.SaveChanges();
        }

        public void Process()
        {
            var courses = Get();

            if (courses != null)
            {
                _context.ProcessedCourses.RemoveRange(_context.ProcessedCourses);

                foreach (var item in courses)
                {
                    ProcessedCourseModel model = new ProcessedCourseModel()
                    {
                        CourseId = item.Id,
                        CurrentNumberStudents = item.Students.Count(),
                        AverageAge = item.Students.Average(c => c.Age).GetValueOrDefault(),
                        CourseName = item.Name,
                        MaxAge = item.Students.Max(c => c.Age).GetValueOrDefault(),
                        MinAge = item.Students.Min(c => c.Age).GetValueOrDefault(),
                        MaxStudents = item.MaxStudents,
                        TeacherId = item.Teacher.Id,
                        TeacherName = item.Teacher.Name
                    };

                    _context.ProcessedCourses.Attach(model);
                    _context.SaveChanges();
                }
            }
        }

        public List<ProcessedCourseModel> GetProcessedCourses()
        {
            return (from course in _context.ProcessedCourses
                    select new ProcessedCourseModel()
                    {
                        CourseId = course.CourseId,
                        CurrentNumberStudents = course.CurrentNumberStudents,
                        AverageAge = course.AverageAge,
                        CourseName = course.CourseName,
                        MaxAge = course.MaxAge,
                        MinAge = course.MinAge,
                        MaxStudents = course.MaxStudents,
                        TeacherId = course.TeacherId,
                        TeacherName = course.TeacherName,
                        Students = _context.StudentByCourse.Where(s => s.CourseId == course.CourseId).Select(s => s.Student).ToList()
                    }).ToList();
        }

    }
}
