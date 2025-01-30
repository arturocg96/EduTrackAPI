using ApiCursos.Data;
using ApiCursos.Models;
using ApiCursos.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
namespace ApiCursos.Repository
{
    public class CourseRepository : ICourse
    {
        private readonly ApplicationDbContext _db;
        public CourseRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public ICollection<Course> GetCourses()
        {
            return _db.Course.OrderBy(c => c.Name).ToList();
        }
        public ICollection<Course> GetAllCoursesByCategory(int catId)
        {
            return _db.Course.Include(ca => ca.Category)
                           .Where(ca => ca.categoryId == catId)
                           .ToList();
        }
        public IEnumerable<Course> SearchCourse(string name)
        {
            IQueryable<Course> query = _db.Course;
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.Name.Contains(name) ||
                                       e.Description.Contains(name));
            }
            return query;
        }
        public Course? GetCourse(int courseId)
        {
            return _db.Course.FirstOrDefault(c => c.Id == courseId);
        }
        public bool CourseExists(int id)
        {
            return _db.Course.Any(c => c.Id == id);
        }
        public bool CourseExists(string name)
        {
            return _db.Course.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        }
        public bool CreateCourse(Course course)
        {
            course.CreationDate = DateTime.Now;
            _db.Course.Add(course);
            return Save();
        }
        public bool UpdateCourse(Course course)
        {
            course.CreationDate = DateTime.Now;
            var existingCourse = _db.Course.Find(course.Id);
            if (existingCourse != null)
            {
                _db.Entry(existingCourse).CurrentValues.SetValues(course);
            }
            else
            {
                _db.Course.Update(course);
            }
            return Save();
        }
        public bool DeleteCourse(Course course)
        {
            _db.Course.Remove(course);
            return Save();
        }
        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }
    }
}