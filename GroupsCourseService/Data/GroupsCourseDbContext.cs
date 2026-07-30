using GroupsCourseService.Model;
using Microsoft.EntityFrameworkCore;


namespace GroupsCourseService.Data
{
    public class GroupsCourseDbContext : DbContext
    {

        public GroupsCourseDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Enrollment>  Enrollments { get; set; }


    }
}
