using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models.DTO.Cart
{
    public class CartDetailDTO
    {
        [Required]
        public int FoodId { get; set; }
        [Required, Range(1, 100)]
        public int Quantity { get; set; }
    }
}
