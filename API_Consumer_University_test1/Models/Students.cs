using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Consumer_University_test1.Models
{
    public class Students
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }
        [NotMapped]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public int? hours_term { get; set; }
        public double? reciept { get; set; }
        public ICollection<Courses> courses { get; set; } = new List<Courses>();
        public Majors? Major { get; set; }
        public int? MajorsId { get; set; }
        public string? UserId { get; set; }
    }
}
