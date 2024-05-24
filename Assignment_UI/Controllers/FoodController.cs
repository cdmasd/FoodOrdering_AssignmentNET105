using Assignment_UI.Models;
using Assignment_UI.ViewModel;
using Microsoft.AspNetCore.Authorization;
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
                return View();
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
            var response = _client.GetAsync(baseAddress + $"/Food/categoryid={id}").Result;
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
            var response = _client.GetAsync(baseAddress + $"/Food/name={query}").Result;
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

        public IActionResult Filter(int? value)
        {
            decimal minrange = 0;
            decimal maxrange = 0;
            if(value == 0)
            {
                return RedirectToAction("Index");
            } else if (value == 1)
            {
                minrange = 0;
                maxrange = 16000;
            } 
            else if (value == 2)
            {
                minrange = 16000;
                maxrange = 31000;
            }
            else if (value == 3)
            {
                minrange = 32000;
                maxrange = 61000;
            }
            else if (value == 4)
            {
                minrange = 61000;
                maxrange = 100000;
            }
            else if (value == 5)
            {
                minrange = 100000;
                maxrange = 900000;
            }
            var foods = new List<Food>();
            var response = _client.GetAsync(baseAddress + $"/Food/filter?minrange={minrange}&maxrange={maxrange}").Result;
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

        public IActionResult Sort(int? page, string sort)
        {
            if(sort == "0")
            {
                return RedirectToAction("Index");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            var foods = new List<Food>();
            HttpResponseMessage response = _client.GetAsync(baseAddress + $"/Food/sort={sort}?page={pageNumber}").Result;
            ViewBag.Sort = sort;
            if (response.IsSuccessStatusCode)
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

        public IActionResult Details(int? page,int id)
        {
            int pageNumber = (page ?? 4);
            int pageSize = 4;
            var food = new Food();
            var foods = new List<Food>();
            var response = _client.GetAsync(baseAddress + $"/Food/{id}").Result;
            var getAll = _client.GetAsync(baseAddress + $"/Food/?page={pageNumber}&pageSize={pageSize}").Result;
            if (!response.IsSuccessStatusCode || !getAll.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var datafood = response.Content.ReadAsStringAsync().Result;
            food = JsonConvert.DeserializeObject<Food>(datafood);
            var listfoods = getAll.Content.ReadAsStringAsync().Result;
            foods = JsonConvert.DeserializeObject<List<Food>>(listfoods);
            var details = new DetailViews()
            {
                food = food,
                foods = foods
            };
            return View(details);
        }
    }
}
