using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models.DTO.Food
{
    public class CreateFoodDTO
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required, Range(1000, 1000000), Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public int CategoryID { get; set; }
    }
}
