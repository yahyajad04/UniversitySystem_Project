using API_University_test1.Models;
using Microsoft.EntityFrameworkCore;

namespace API_University_test1.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Students> Students { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Majors> Majors { get; set; }
        public DbSet<Teachers> Teachers { get; set; }
        public DbSet<Grades> Grades { get; set; }
    }
}
