using Assignment_Server.Models.DTO.Food;

namespace Assignment_Server.Models
{
    public class ListCartDetail
    {
        public FoodDTO Food { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
