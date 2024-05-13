using Assignment_Server.Models;
using Assignment_Server.Models.DTO.Food;

namespace Assignment_Server.Interfaces
{
    public interface IFoodRepo
    {
        bool CreateFood(Food food);
        bool UpdateFood(Food food);
        bool DeleteFood(int id);
        IEnumerable<Food> GetAllFood();
        Food GetById(int id);
        IEnumerable<Food> SearchByName(string name);
        IEnumerable<Food> getByFilter(decimal? priceRange, int? categoryId);
        IEnumerable<Food> getByCategoryID(int categoryId);
    }
}
