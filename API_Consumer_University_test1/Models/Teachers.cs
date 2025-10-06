using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_Consumer_University_test1.Models
{
    public class Teachers
    {
        public int Id { get; set; }
        public string? Teacher_Name { get; set; }
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
        public int? Salary { get; set; }
        public ICollection<Courses>? T_courses { get; set; } = new List<Courses>();
        public string? PHD { get; set; }
        public string? PHD_University { get; set; }
        public string? UserId { get; set; }
    }
}
