using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Server.Services
{
    public class CategoryRepo(FoodDbContext db) : ICategoryRepo
    {
        private readonly FoodDbContext _db = db;

        public bool CreateCategory(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            return true;
        }

        public bool DeleteCategory(int id)
        {
            var category = GetById(id);
            if(category != null)
            {
                _db.Categories.Remove(category);
                _db.SaveChanges();
                return true;
            }
            
            return false;
        }

        public IEnumerable<Category> GetAll()
        {
            var categories = _db.Categories.ToList();
            return categories;
        }

        public Category GetById(int id)
        {
            var category = _db.Categories.Find(id);
            if(category == null)
            {
                return null;
            }
            return category;
        }

        public bool UpdateCategory(Category category)
        {
            _db.Categories.Update(category);
            _db.SaveChanges();
            return true;
        }
    }
}
