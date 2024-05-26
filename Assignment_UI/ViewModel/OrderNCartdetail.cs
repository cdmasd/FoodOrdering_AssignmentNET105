using Assignment_UI.Models;

namespace Assignment_UI.ViewModel
{
    public class OrderNCartdetail
    {
        public Order Order { get; set; }
        public IEnumerable<CartDetail> CartDetail { get; set; }
    }
}
