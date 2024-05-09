using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public virtual ICollection<Food> Foods { get; set; }
    }
}
