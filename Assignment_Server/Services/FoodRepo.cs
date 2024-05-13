using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.Food;

namespace Assignment_Server.Services
{
    public class FoodRepo(FoodDbContext db) : IFoodRepo
    {
        private readonly FoodDbContext _db = db;

        public bool CreateFood(Food food)
        {
            _db.Foods.Add(food);
            _db.SaveChanges();
            return true;
        }

        public bool DeleteFood(int id)
        {
            var food = GetById(id);
            if(food != null)
            {
                _db.Foods.Remove(food);
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Food> GetAllFood()
        {
            return _db.Foods.ToList();
        }

        public IEnumerable<Food> getByCategoryID(int categoryId)
        {
            var foods = _db.Foods.Where(x=> x.CategoryID == categoryId).ToList();
            return foods;
        }

        public IEnumerable<Food> getByFilter(decimal? priceRange, int? categoryId)
        {
            var foods = _db.Foods.AsQueryable();
            if (priceRange.HasValue)
            {
                foods = foods.Where(x => x.UnitPrice < priceRange.Value);
            }
            if (categoryId.HasValue)
            {
                foods = foods.Where(x => x.CategoryID == categoryId.Value);
            }
            var filtered = foods.ToList();
            return filtered;
        }

        public Food GetById(int id)
        {
            var food = _db.Foods.Find(id);
            if(food != null)
            {
                return food;
            }
            return null;
        }

        public IEnumerable<Food> SearchByName(string name)
        {
            var food = _db.Foods.Where(x=> x.Name.Contains(name));
            return food.ToList();
        }

        public bool UpdateFood(Food dto)
        {
            _db.Foods.Update(dto);
            _db.SaveChanges();
            return true;
        }
    }
}
