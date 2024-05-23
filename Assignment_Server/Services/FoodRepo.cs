using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.Food;

namespace Assignment_Server.Services
{
    public class FoodRepo(FoodDbContext db) : IFoodRepo
    {
        private readonly FoodDbContext _db = db;

        public Food AddFood(Food food)
        {
            _db.Foods.Add(food);
            _db.SaveChanges();
            return food;
        }

        public void DeleteFood(int id)
        {
            var food = GetById(id);
            _db.Foods.Remove(food);
            _db.SaveChanges();
        }

        public IEnumerable<Food> Foods => _db.Foods.ToList();

        public IEnumerable<Food> getByCategoryID(int categoryId)
        {
            var foods = _db.Foods.Where(x=> x.CategoryID == categoryId).ToList();
            return foods;
        }

        public IEnumerable<Food> getByFilter(decimal? minrange,decimal? maxrange)
        {
            var foods = _db.Foods.AsQueryable();
            if (minrange.HasValue && maxrange.HasValue)
            {
                foods = foods.Where(x => x.UnitPrice <= maxrange.Value && x.UnitPrice >= minrange.Value);
            }
            var filtered = foods.ToList();
            return filtered;
        }

        public Food GetById(int id) => _db.Foods.Find(id);

        public IEnumerable<Food> SearchByName(string name)
        {
            var food = _db.Foods.Where(x=> x.Name.Contains(name));
            return food.ToList();
        }

        public IEnumerable<Food> Sort(string sort)
        {
            var foods = Foods;
            if(sort == "asc")
            {
                foods = foods.OrderBy(x => x.UnitPrice);
            } else
            {
                foods = foods.OrderByDescending(x => x.UnitPrice);
            }
            return foods;
        }

        public Food UpdateFood(Food dto)
        {
            _db.Foods.Update(dto);
            _db.SaveChanges();
            return dto;
        }
    }
}
