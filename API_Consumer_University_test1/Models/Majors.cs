namespace API_Consumer_University_test1.Models
{
    public class Majors
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? major_hours { get; set; }
        public double? major_cost_hour { get; set; }
        public ICollection<Students> students { get; set; } = new List<Students>();
    }
}
