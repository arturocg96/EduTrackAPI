//=====================================================================
// CategoryRepository.cs
// Implementación del repositorio para la entidad Category
// Maneja todas las operaciones CRUD relacionadas con categorías
//=====================================================================

using ApiCursos.Data;
using ApiCursos.Models;
using ApiCursos.Repository.IRepository;

namespace ApiCursos.Repository;

public class CategoryRepository : ICategory
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Constructor que recibe el contexto de base de datos
    /// </summary>
    /// <param name="db">Contexto de la base de datos</param>
    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Obtiene todas las categorías ordenadas por nombre
    /// </summary>
    /// <returns>Colección de categorías</returns>
    public ICollection<Category> GetCategories()
    {
        return _db.Category.OrderBy(c => c.Name).ToList();
    }

    /// <summary>
    /// Obtiene una categoría específica por su ID
    /// </summary>
    /// <param name="CategoryId">ID de la categoría a buscar</param>
    /// <returns>Categoría encontrada o null</returns>
    public Category? GetCategory(int CategoryId)
    {
        return _db.Category.FirstOrDefault(c => c.Id == CategoryId);
    }

    /// <summary>
    /// Verifica si existe una categoría por su ID
    /// </summary>
    /// <param name="id">ID a verificar</param>
    /// <returns>True si existe, False si no</returns>
    public bool CategoryExists(int id)
    {
        return _db.Category.Any(c => c.Id == id);
    }

    /// <summary>
    /// Verifica si existe una categoría por su nombre
    /// </summary>
    /// <param name="name">Nombre a verificar</param>
    /// <returns>True si existe, False si no</returns>
    public bool CategoryExists(string name)
    {
        return _db.Category.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    /// <summary>
    /// Crea una nueva categoría
    /// </summary>
    /// <param name="category">Categoría a crear</param>
    /// <returns>True si se creó exitosamente</returns>
    public bool CreateCategory(Category category)
    {
        category.CreationDate = DateTime.Now;
        _db.Category.Add(category);
        return Save();
    }

    /// <summary>
    /// Actualiza una categoría existente
    /// </summary>
    /// <param name="category">Categoría con los nuevos datos</param>
    /// <returns>True si se actualizó exitosamente</returns>
    public bool UpdateCategory(Category category)
    {
        category.CreationDate = DateTime.Now;
        var existingCategory = _db.Category.Find(category.Id);
        if (existingCategory != null)
        {
            _db.Entry(existingCategory).CurrentValues.SetValues(category);
        }
        else
        {
            _db.Category.Update(category);
        }
        return Save();
    }

    /// <summary>
    /// Elimina una categoría
    /// </summary>
    /// <param name="category">Categoría a eliminar</param>
    /// <returns>True si se eliminó exitosamente</returns>
    public bool DeleteCategory(Category category)
    {
        _db.Category.Remove(category);
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