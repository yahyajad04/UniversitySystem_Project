using System.ComponentModel.DataAnnotations;

namespace API_University_test1.Models
{
    public class Courses
    {
        [Key]
        public int Id { get; set; }
        public string Course_Name { get; set; }
        public string Description { get; set; }
        public string Teacher_Name { get; set; }
        public int Course_Hours { get; set; }
        public ICollection<Students> Students { get; set; } = new List<Students>();
    }
}
