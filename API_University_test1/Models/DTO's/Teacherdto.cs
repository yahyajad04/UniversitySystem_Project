namespace API_University_test1.Models.DTO_s
{
    public class Teacherdto
    {
        public int Id { get; set; }
        public string? Teacher_Name { get; set; }
        public string? Email { get; set; }
        public ICollection<Courses>? T_courses { get; set; } = new List<Courses>();
        public string? UserId { get; set; }
    }
}
