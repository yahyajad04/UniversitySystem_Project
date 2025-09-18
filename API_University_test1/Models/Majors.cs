using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_University_test1.Models
{
    public class Majors
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? major_hours { get; set; }
        public double? major_cost_hour { get; set; }
        [JsonIgnore]
        public ICollection<Students> students { get; set; } = new List<Students>();
    }
}
