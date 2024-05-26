using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.Models
{
    public class Order
    {
        public string FullName { get; set; } = string.Empty;
        [Required, MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required, RegularExpression("^(03|06|07|08|09)[0-9]{8}$", ErrorMessage = "Phone is not valid")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string PaymentType { get; set; } = string.Empty;
        public string? note { get; set; }
    }
}
