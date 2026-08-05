using AssignmentService.Model;
using Microsoft.EntityFrameworkCore;

namespace AssignmentService.Data
{
    public class AssignmentDbContext : DbContext
    {
        public AssignmentDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
    }
}
