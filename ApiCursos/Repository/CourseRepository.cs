//=====================================================================
// CourseRepository.cs
// Implementación del repositorio para la entidad Course
// Maneja operaciones CRUD y búsquedas especializadas para cursos
//=====================================================================

using ApiCursos.Data;
using ApiCursos.Models;
using ApiCursos.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiCursos.Repository;

public class CourseRepository : ICourse
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Constructor que recibe el contexto de base de datos
    /// </summary>
    /// <param name="db">Contexto de la base de datos</param>
    public CourseRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Obtiene cursos de forma paginada
    /// </summary>
    /// <param name="pageNumber">Número de página actual</param>
    /// <param name="pageSize">Cantidad de elementos por página</param>
    /// <returns>Colección paginada de cursos</returns>
    public ICollection<Course> GetCourses(int pageNumber, int pageSize)
    {
        return _db.Course.OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    /// <summary>
    /// Obtiene el número total de cursos
    /// </summary>
    /// <returns>Cantidad total de cursos</returns>
    public int GetTotalCourses()
    {
        return _db.Course.Count();
    }

    /// <summary>
    /// Obtiene todos los cursos de una categoría específica
    /// </summary>
    /// <param name="catId">ID de la categoría</param>
    /// <returns>Colección de cursos de la categoría</returns>
    public ICollection<Course> GetAllCoursesByCategory(int catId)
    {
        return _db.Course.Include(ca => ca.Category)
                       .Where(ca => ca.categoryId == catId)
                       .ToList();
    }

    /// <summary>
    /// Busca cursos por nombre o descripción
    /// </summary>
    /// <param name="name">Término de búsqueda</param>
    /// <returns>Colección de cursos que coinciden</returns>
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

    /// <summary>
    /// Obtiene un curso específico por su ID
    /// </summary>
    /// <param name="courseId">ID del curso</param>
    /// <returns>Curso encontrado o null</returns>
    public Course? GetCourse(int courseId)
    {
        return _db.Course.FirstOrDefault(c => c.Id == courseId);
    }

    /// <summary>
    /// Verifica si existe un curso por su ID
    /// </summary>
    /// <param name="id">ID a verificar</param>
    /// <returns>True si existe, False si no</returns>
    public bool CourseExists(int id)
    {
        return _db.Course.Any(c => c.Id == id);
    }

    /// <summary>
    /// Verifica si existe un curso por su nombre
    /// </summary>
    /// <param name="name">Nombre a verificar</param>
    /// <returns>True si existe, False si no</returns>
    public bool CourseExists(string name)
    {
        return _db.Course.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    /// <summary>
    /// Crea un nuevo curso
    /// </summary>
    /// <param name="course">Curso a crear</param>
    /// <returns>True si se creó exitosamente</returns>
    public bool CreateCourse(Course course)
    {
        course.CreationDate = DateTime.Now;
        _db.Course.Add(course);
        return Save();
    }

    /// <summary>
    /// Actualiza un curso existente
    /// </summary>
    /// <param name="course">Curso con los nuevos datos</param>
    /// <returns>True si se actualizó exitosamente</returns>
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

    /// <summary>
    /// Elimina un curso
    /// </summary>
    /// <param name="course">Curso a eliminar</param>
    /// <returns>True si se eliminó exitosamente</returns>
    public bool DeleteCourse(Course course)
    {
        _db.Course.Remove(course);
        return Save();
    }

    /// <summary>
    /// Guarda los cambios en la base de datos
    /// </summary>
    /// <returns>True si se guardaron los cambios exitosamente</returns>
    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }
}