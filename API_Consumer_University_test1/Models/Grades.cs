namespace API_Consumer_University_test1.Models
{
    public class Grades
    {
        public int? Id { get; set; }
        public double? first { get; set; }
        public double? second { get; set; }
        public double? final { get; set; }
        public double? total { get; set; }
        public int? StudentId { get; set; }
        public int? CourseId { get; set; }
        public Students? Student { get; set; }
        public Courses? Course { get; set; }
    }
}
