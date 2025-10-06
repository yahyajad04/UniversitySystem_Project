namespace API_University_test1.Models
{
    public class Coursedto
    {
        public int Id { get; set; }
        public string Course_Name { get; set; }
        public int? TeacherId { get; set; }
        public Teachers? Teacher { get; set; }
        public int Course_Hours { get; set; }
        public List<Studentdto> Students { get; set; }
        public int? isApproved { get; set; }
        public int? isDone { get; set; }
    }
}
