using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_University_test1.Models
{
    public class Students
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? hours_term { get; set; }
        public double? reciept { get; set; }
        public ICollection<Courses> courses { get; set; } = new List<Courses>();
        public int Total_Hours { get; set; }
        public Majors? Major {  get; set; }
        public int? MajorsId { get; set; }
        public string? UserId { get; set; }
        public ICollection<Grades>? Grades { get; set; } = new List<Grades>();

    }
}
