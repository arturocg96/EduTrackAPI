using ApiCursos.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCursos.Data{
   
        public class ApplicationDbContext: DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

            public DbSet<Category> Category { get; set; }
        
            public DbSet<Course> Course { get; set; }

            public DbSet<User> User { get; set; }


    }
}
