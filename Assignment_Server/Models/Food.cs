using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Server.Models
{
    public class Food
    {
        [Key]
        public int FoodId { get; set; }
        [Required,MaxLength(50)]
        public string Name { get; set; }
        [Required, Range(1000, 1000000),Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public int View { get; set; }

        [ForeignKey("Category"),Required]
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<FoodImage> FoodImages { get; set; }
    }
}
