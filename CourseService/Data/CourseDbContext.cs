using CourseService.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Data
{
    public class CourseDBContext : DbContext
    {
        public CourseDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<InstructorCourses>  instructorCourses{ get; set; }
    }
}
