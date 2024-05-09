using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Server.Models
{
    public class FoodImage
    {
        [Key]
        public int ImageId { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required, ForeignKey("Food")]
        public int FoodId { get; set; }
        public virtual Food Food { get; set; }
    }
}
