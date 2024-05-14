

namespace Assignment_Server.Models.DTO.Food
{
    public class FoodAndImageDTO
    {
        public FoodDTO food { get; set; }
        public List<FoodImage> images { get; set; }
    }
}
