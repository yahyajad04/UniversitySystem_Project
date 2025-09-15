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
        //public DbSet<Students> students { get; set; }
        //public DbSet<Courses> courses { get; set; }
        //public DbSet<API_Consumer_University_test1.Models.Majors> Majors { get; set; } = default!;
    }
}
