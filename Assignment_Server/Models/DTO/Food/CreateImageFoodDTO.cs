using System.ComponentModel.DataAnnotations;

namespace Assignment_Server.Models.DTO.Food
{
    public class CreateImageFoodDTO
    {
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public int FoodId { get; set; }
    }
}
