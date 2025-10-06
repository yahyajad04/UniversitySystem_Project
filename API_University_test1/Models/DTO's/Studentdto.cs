namespace API_University_test1.Models
{
    public class Studentdto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public Majors? Major { get; set; }
        public int? hours_term { get; set; }
        public double? reciept { get; set; }
        public List<Coursedto> Courses { get; set; }
        public ICollection<Grades> Grades { get; set; } = new List<Grades>();
    }
}
