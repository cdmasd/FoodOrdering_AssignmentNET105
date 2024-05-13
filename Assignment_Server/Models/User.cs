using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; } = "default.png";

        // Navigation
        public virtual Cart Cart { get; set; }
        public virtual ICollection<Order> Orders { get;}
    }
}
