using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models.DTO.User
{
    public class UserInfo
    {
        public string fullName { get; set; }
        public string Address { get; set; }
        public string avatarUrl { get; set; }
        public string PhoneNumber { get; set; }
    }
}
