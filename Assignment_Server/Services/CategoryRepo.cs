using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Server.Services
{
    public class CategoryRepo(FoodDbContext db) : ICategoryRepo
    {
        private readonly FoodDbContext _db = db;

        public Category AddCategory(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            return category;
        }

        public void DeleteCategory(int id)
        {
            var category = GetById(id);
            if(category != null)
            {
                _db.Categories.Remove(category);
                _db.SaveChanges();
            }
        }

        public IEnumerable<Category> Categories => _db.Categories.ToList();

        public Category GetById(int id) => _db.Categories.Find(id) ;


        public Category UpdateCategory(Category category)
        {
            _db.Categories.Update(category);
            _db.SaveChanges();
            return category;
        }
    }
}
