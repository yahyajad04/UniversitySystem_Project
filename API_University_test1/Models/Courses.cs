using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_University_test1.Models
{
    public class Courses
    {
        [Key]
        public int Id { get; set; }
        public string? Course_Name { get; set; }
        public string? Description { get; set; }
        public Teachers? Teacher { get; set; }
        public int? TeacherId { get; set; }
        public int Course_Hours { get; set; }
        public ICollection<Students>? Students { get; set; } = new List<Students>();
        public ICollection<Majors>? Majors { get; set; } = new List<Majors>();
        [NotMapped]
        public List<int>? MajorsId { get; set; } = new List<int>();
        public ICollection<Grades>? Grades { get; set; } = new List<Grades>();
        public int? isApproved { get; set; }
        public int? isDone { get; set; }
    }
}
