using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models.DTO.User
{
    public class RegisterDTO
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required, EmailAddress(ErrorMessage= "Email is not valid")]
        public string? Email { get; set; }
        [Required]
        public string? FullName { get; set; }
    }
}
