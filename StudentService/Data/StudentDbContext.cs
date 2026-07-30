using Microsoft.EntityFrameworkCore;
using StudentService.Model;

namespace StudentService.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
    }
}
