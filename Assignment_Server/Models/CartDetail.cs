using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Server.Models
{
    [PrimaryKey(nameof(CartId),nameof(FoodId))]
    public class CartDetail
    {
        [Required,ForeignKey("Cart")]
        public int CartId { get; set; }
        [Required,ForeignKey("Food")]
        public int FoodId { get; set; }
        [Required,Range(1,100)]
        public int Quantity { get; set; }
        [Required, Range(1000, 1000000), Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Navigation Food
        public virtual Food Food { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
