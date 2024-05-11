using Assignment_Server.Models;

namespace Assignment_Server.Interfaces
{
    public interface IFoodImage
    {
        bool CreateFoodImage(FoodImage foodImage);
        IEnumerable<FoodImage> GetFoodImages(int foodid);
    }
}
