using Content_Service.Model;
using LectureService.Model;
using Microsoft.EntityFrameworkCore;

namespace LectureService.Data
{
    public class ContentDbContext : DbContext
    {
        public ContentDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Material>  Materials { get; set; }
    }
}
