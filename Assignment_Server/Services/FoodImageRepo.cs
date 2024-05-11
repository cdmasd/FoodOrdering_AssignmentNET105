using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Models;

namespace Assignment_Server.Services
{
    public class FoodImageRepo(FoodDbContext db) : IFoodImage
    {
        private readonly FoodDbContext _db = db;
        public bool CreateFoodImage(FoodImage foodImage)
        {
            _db.FoodImages.Add(foodImage);
            _db.SaveChanges();
            return true;
        }

        public IEnumerable<FoodImage> GetFoodImages(int foodid)
        {
            var images = _db.FoodImages.Where(x=> x.FoodId == foodid).ToList();
            return images;
        }
    }
}
