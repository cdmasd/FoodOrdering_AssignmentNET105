using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.ViewModel
{
    public class CartDetailVM
    {
        [Required]
        public int FoodId { get; set; }
        [Required, Range(1, 100)]
        public int Quantity { get; set; }
    }
}
