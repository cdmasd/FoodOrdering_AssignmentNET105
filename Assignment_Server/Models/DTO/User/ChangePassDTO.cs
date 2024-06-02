using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models.DTO.User
{
    public class ChangePassDTO
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(5, ErrorMessage = "Password must at least 5 character")]
        [RegularExpression("^(?=.*\\d)(?=.*[A-Z])[A-Za-z\\d@$!%*#?&]{5,}$", ErrorMessage = "Password must contain at least 1 number and 1 capital character")]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
