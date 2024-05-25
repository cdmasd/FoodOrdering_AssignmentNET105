using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Server.Models
{
    [PrimaryKey(nameof(OrderId), nameof(FoodId))]
    public class OrderDetail
    {
        [Required,ForeignKey("Order")]
        public int OrderId { get; set; }
        [Required, ForeignKey("Food")]
        public int FoodId { get; set; }
        [Required, Range(1000, 1000000), Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required, Range(1000, 1000000), Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Navigation
        public virtual Food Food { get; set; }
        public virtual Order Order { get; set; }
    }
}
