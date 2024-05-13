using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Server.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        [Required,ForeignKey("Order")]
        public int OrderId { get; set; }
        [Required, ForeignKey("Food")]
        public int FoodId { get; set; }
        [Required, Range(1000, 1000000), Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        [Range(1, 100), Required]
        public int Discount { get; set; } = 0;
        [Required, Range(1000, 1000000), Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Navigation
        public virtual Food Food { get; set; }
        public virtual Order Order { get; set; }
    }
}
