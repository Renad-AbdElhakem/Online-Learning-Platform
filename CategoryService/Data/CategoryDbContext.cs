using CategoryService.Model;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Data
{
    public class CategoryDbContext : DbContext
    {
        public CategoryDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Category> categories { get; set; }
    }
}
