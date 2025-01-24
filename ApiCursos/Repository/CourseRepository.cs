using ApiCursos.Data;
using ApiCursos.Models;
using ApiCursos.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiCursos.Repository
{
    public class CourseRepository : ICourse
    {
        private readonly ApplicationDbContext? _db;

        public CourseRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CourseExists(int id)
        {
            return _db.Course.Any(c => c.Id == id);
        }

        public bool CourseExists(string name)
        {
            bool value = _db.Course.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool CreateCourse(Course course)
        {
            course.CreationDate = DateTime.Now;
            _db.Course.Add(course);
            return Save();
        }

        public bool DeleteCourse(Course course)
        {
            _db.Course.Remove(course);
            return Save();
        }

        public ICollection<Course> GetAllCoursesByCategory(int catId)
        {
            return _db.Course.Include(ca => ca.Category).Where(ca => ca.categoryId == catId).ToList();
        }

        public Course GetCourse(int courseId)
        {
            return _db.Course.FirstOrDefault(c => c.Id == courseId);
        }

        public ICollection<Course> GetCourses()
        {
            return _db.Course.OrderBy(c => c.Name).ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public IEnumerable<Course> SearchCourse(string name)
        {
            IQueryable<Course> query = _db.Course;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e=> e.Name.Contains(name) || e.Description.Contains(name));
            }

            return query;
        }

        public bool UpdateCourse(Course course)
        {
           course.CreationDate = DateTime.Now;

           //Arreglar problema del PATCH

           var courseThatExists = _db.Course.Find(course.Id);

            if (courseThatExists != null)
            {
                _db.Entry(courseThatExists).CurrentValues.SetValues(course);
            }
            else
            {
                _db.Course.Update(course);
            }

            return Save();
        
        }
    }
}
