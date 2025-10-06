using API_Consumer_University_test1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API_Consumer_University_test1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<API_Consumer_University_test1.Models.Teachers> Teachers { get; set; } = default!;
        public DbSet<API_Consumer_University_test1.Models.Courses> Courses { get; set; } = default!;
        public DbSet<API_Consumer_University_test1.Models.Grades> Grades { get; set; } = default!;
        public DbSet<API_Consumer_University_test1.Models.Students> Students { get; set; } = default!;
        //public DbSet<Students> students { get; set; }
        //public DbSet<Courses> courses { get; set; }
        //public DbSet<API_Consumer_University_test1.Models.Majors> Majors { get; set; } = default!;
    }
}
