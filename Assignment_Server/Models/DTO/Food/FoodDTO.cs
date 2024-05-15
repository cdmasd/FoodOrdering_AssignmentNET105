using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Server.Models.DTO.Food
{
    public class FoodDTO
    {
        public int FoodId { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required, Range(1000, 1000000), Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public string mainImage { get; set; }
        public int View { get; set; }

        [Required]
        public int CategoryID { get; set; }
    }
}
