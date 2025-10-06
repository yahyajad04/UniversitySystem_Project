using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_University_test1.Models
{
    public class Teachers
    {

        [Key]
        public int Id { get; set; }
        public string? Teacher_Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? Salary { get; set; }
        [JsonIgnore]
        public ICollection<Courses>? T_courses { get; set; } = new List<Courses>();
        public string? PHD { get; set; }
        public string? PHD_University { get; set; }
        public string? UserId { get; set; }
    }
}
