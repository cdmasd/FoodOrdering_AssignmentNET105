using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.Models
{
    public class Login
    {
        [Required(ErrorMessage = "This field can not empty")]
        [DisplayName("Username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "This field can not empty")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool keepLogined { get; set; }
    }
}
