using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models.DTO.Category
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public string ImageUrl { get; set; }
    }
}
