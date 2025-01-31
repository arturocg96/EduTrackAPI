using ApiCursos.Models;

namespace ApiCursos.Repository.IRepository
{
    public interface ICourse
    {
        ICollection<Course> GetCourses(int pageNumber, int pageSize);
        int GetTotalCourses();

        ICollection<Course> GetAllCoursesByCategory(int catId);

        IEnumerable<Course> SearchCourse(string name);

        Course GetCourse(int courseId);

        bool CourseExists(int id);

        bool CourseExists(string name);

        bool CreateCourse(Course course);

        bool UpdateCourse(Course course);

        bool DeleteCourse(Course course);

        bool Save();
    }
}