namespace Assignment_UI.ViewModel.Order
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string PaymentType { get; set; }
        public string? note { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
    }
}
