using ApiCursos.Models;

namespace ApiCursos.Repository.IRepository
{
    public interface ICategory
    {
        ICollection<Category> GetCategories();

        Category GetCategory(int CategoryId);

        bool CategoryExists(int id);

        bool CategoryExists(string CategoryName);

        bool CreateCategory(Category category);

        bool UpdateCategory(Category category);

        bool DeleteCategory(Category category);

        bool Save();
    }
}