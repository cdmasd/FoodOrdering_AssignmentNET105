using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.ViewModel
{
    public class UserInfo
    {
        [MaxLength(100),Display(Name = "Full name")]
        public string fullName { get; set; }
        [MaxLength(250)]
        public string Address { get; set; }
        public string? avatarUrl { get; set; }
        [RegularExpression("^(03|05|07|08|09)\\d{8}$", ErrorMessage = "Phone number is not valid"),Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
