using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models.DTO.User
{
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool keepLogined { get; set; }
    }
}
