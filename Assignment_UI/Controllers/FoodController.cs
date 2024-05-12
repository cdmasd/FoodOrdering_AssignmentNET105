using Assignment_UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_UI.Controllers
{
    public class FoodController : Controller
    {
        public IActionResult Index()
        {
            var foods = new List<Food>()
            {
                new Food {FoodId = 1, CategoryId = 1, Name = "Cơm xào tỏi", UnitPrice = 54000, View = 0},
                new Food {FoodId = 2, CategoryId = 2, Name = "Cơm xào dưa", UnitPrice = 54000, View = 0},
                new Food {FoodId = 3, CategoryId = 3, Name = "Cơm xào ớt", UnitPrice = 54000, View = 0},
                new Food {FoodId = 4, CategoryId = 4, Name = "Mì xào tỏi", UnitPrice = 54000, View = 0},
                new Food {FoodId = 5, CategoryId = 1, Name = "Mì xào dưa", UnitPrice = 54000, View = 0},
                new Food {FoodId = 6, CategoryId = 2, Name = "Mì xào ớt", UnitPrice = 54000, View = 0},
                new Food {FoodId = 7, CategoryId = 3, Name = "Nui xào tỏi", UnitPrice = 54000, View = 0},
                new Food {FoodId = 8, CategoryId = 4, Name = "Nui xào ớt", UnitPrice = 54000, View = 0}
            };
            return View(foods);
        }
    }
}
