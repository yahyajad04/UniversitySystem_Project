namespace API_University_test1.Models
{
    public class Coursedto
    {
        public int Id { get; set; }
        public string Course_Name { get; set; }
        public string Teacher_Name { get; set; }
        public int Course_Hours { get; set; }
        public List<Studentdto> Students { get; set; }
    }
}
