using Assignment_UI.Models;
using Assignment_UI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Assignment_UI.Controllers
{
    public class FoodController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7294/api");
        readonly HttpClient _client;
        public FoodController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        public IActionResult Index(int? page)
        {
            if(page > 4)
            {
                return NotFound();
            }
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            var foods = new List<Food>();
            HttpResponseMessage response = _client.GetAsync(baseAddress + $"/Food/?page={pageNumber}&pageSize={pageSize}").Result;
            if(response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                foods = JsonConvert.DeserializeObject<List<Food>>(data);
                var foodPaging = new ProductVM()
                {   
                    Foods = foods,
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    totalItem = foods.Count,
                    pageCount = (int)Math.Ceiling(36 / (double)pageSize)
                };
                return View(foodPaging);
            }
            return View();
        }

        public IActionResult FilterLoai(int id)
        {
            var foods = new List<Food>();
            var response = _client.GetAsync(baseAddress + $"/Food/SearchCategoryId{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                foods = JsonConvert.DeserializeObject<List<Food>>(data);
                var foodPaging = new ProductVM()
                {
                    Foods = foods
                };
                return View(foodPaging);
            }
            return NotFound();
        }

        public IActionResult SearchName(string query)
        {
            var foods = new List<Food>();
            var response = _client.GetAsync(baseAddress + $"/Food/SearchName{query}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                foods = JsonConvert.DeserializeObject<List<Food>>(data);
                var foodPaging = new ProductVM()
                {
                    Foods = foods
                };
                return View(foodPaging);
            }
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
