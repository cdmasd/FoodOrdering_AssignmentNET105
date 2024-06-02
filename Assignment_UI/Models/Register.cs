using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Username is required"),MinLength(5,ErrorMessage = "Username must at least 5 character"),Display(Name = "Username")]
        public string userName { get; set; }
        [Required(ErrorMessage = "Password is required"), MinLength(5, ErrorMessage = "Password must at least 5 character"), Display(Name = "Password"),RegularExpression("^(?=.*\\d)(?=.*[A-Z])[A-Za-z\\d@$!%*#?&]{5,}$",ErrorMessage = "Password must contain at least 1 number and 1 capital character")]
        public string password { get; set; }
        [Required(ErrorMessage = "Email is required"),RegularExpression("^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$",ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Full name is required"),Display(Name = "Full name")]
        public string FullName { get; set; }
    }
}
