using InstructorService.Model;
using Microsoft.EntityFrameworkCore;

namespace InstructorService.Data
{
    public class InstructorDbContext :DbContext
    {
        public InstructorDbContext(DbContextOptions options):base(options)
        {
            
        }


      public DbSet<Instructor> Instructors { get; set; }
    }
}
