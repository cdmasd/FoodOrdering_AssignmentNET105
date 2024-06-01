using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.ViewModel.Order
{
    public class OrderVM
    {
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }
        [Display(Name = "Full name")]
        public string FullName { get; set; }
        public string Address { get; set; }
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Payment type")]
        public string PaymentType { get; set; }
        [Display(Name = "Payment status")]
        public string PaymentStatus { get; set; }
        [Display(Name = "Note")]
        public string? note { get; set; }
        [Display(Name = "Order date")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Order status")]
        public string OrderStatus { get; set; }
    }
}
