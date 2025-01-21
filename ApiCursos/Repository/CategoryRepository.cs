using ApiCursos.Data;
using ApiCursos.Models;
using ApiCursos.Repository.IRepository;

namespace ApiCursos.Repository
{
    public class CategoryRepository : ICategory
    {
        private readonly ApplicationDbContext? _db;

        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool UpdateCategory(Category category)
        {
            category.CreationDate = DateTime.Now;
            //Arreglar problema del PUT
            var categoryThatExists = _db.Category.Find(category.Id);
            if (categoryThatExists != null) {
                _db.Entry(categoryThatExists).CurrentValues.SetValues(category);
            }
            else
            {
                _db?.Category.Update(category);
            }                  

            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _db?.Category.Remove(category);
            return Save();
        }

        public bool CreateCategory(Category category)
        {
            category.CreationDate = DateTime.Now;
            _db?.Category.Add(category);
            return Save();

        }


        public bool CategoryExists(int id)
        {
            return _db.Category.Any(c => c.Id == id);
        }

        public bool CategoryExists(string name)
        {
            bool value = _db.Category.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public Category GetCategory(int CategoryId)
        {
            return _db.Category.FirstOrDefault(c => c.Id == CategoryId);
        }



        public ICollection<Category> GetCategories()
        {
            return _db.Category.OrderBy(c => c.Name).ToList();
        }

     

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        
    }
}
