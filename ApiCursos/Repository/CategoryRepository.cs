using ApiCursos.Data;
using ApiCursos.Models;
using ApiCursos.Repository.IRepository;
namespace ApiCursos.Repository
{
    public class CategoryRepository : ICategory
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public ICollection<Category> GetCategories()
        {
            return _db.Category.OrderBy(c => c.Name).ToList();
        }
        public Category? GetCategory(int CategoryId)
        {
            return _db.Category.FirstOrDefault(c => c.Id == CategoryId);
        }
        public bool CategoryExists(int id)
        {
            return _db.Category.Any(c => c.Id == id);
        }
        public bool CategoryExists(string name)
        {
            return _db.Category.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        }
        public bool CreateCategory(Category category)
        {
            category.CreationDate = DateTime.Now;
            _db.Category.Add(category);
            return Save();
        }
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
        public bool DeleteCategory(Category category)
        {
            _db.Category.Remove(category);
            return Save();
        }
        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }
    }
}