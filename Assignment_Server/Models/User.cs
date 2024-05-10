using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? AvatarUrl { get; set; }
    }
}
