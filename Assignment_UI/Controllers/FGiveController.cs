﻿using Assignment_UI.Models;
using Assignment_UI.ViewModel;
using Assignment_UI.ViewModel.Category;
using Assignment_UI.ViewModel.Food;
using Assignment_UI.ViewModel.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Assignment_UI.Controllers
{
    public class FGiveController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7294/api");
        readonly HttpClient _client;
        public FGiveController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await getCategories();
            ViewBag.Catecount = categories.Count;
            var foods = await GetFood();
            ViewBag.Foodcount = foods.Count;
            var orders = await GetOrders();
            ViewBag.Ordercount = orders.Count;
            ViewBag.Profit = await getProfit();
            return View();
        }

        #region Category
        public async Task<IActionResult> Categories()
        {
            var categories = await getCategories();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCate(CreateCategoryUploadImage categoryCreate)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "fail to add new category";
                return View(categoryCreate);
            }
            if (categoryCreate.ImgFile != null && categoryCreate.ImgFile.Length > 0)
            {
                categoryCreate.category.ImageUrl = await UploadImage(categoryCreate.ImgFile);
            }
            var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/Category");
            request.Content = new StringContent(JsonConvert.SerializeObject(categoryCreate.category), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Added new category";
                return RedirectToAction("Categories");
            }else
            {
                TempData["error"] = response.Content.ReadAsStringAsync();
                return View(categoryCreate);
            }
        }

        public async Task<IActionResult> DetailCate(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/Category/{id}").Result;
            Category category;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                category = JsonConvert.DeserializeObject<Category>(data);
                return View(category);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCate(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/Category/{id}").Result;
            Category category;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                category = JsonConvert.DeserializeObject<Category>(data);
                var upCate = new UpdateCategory
                {
                    Category = category
                };
                return View(upCate);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCate(UpdateCategory upCategory)
        {

            if (!ModelState.IsValid)
            {
                return View(upCategory);
            }
            if(upCategory.ImageFile != null && upCategory.ImageFile.Length > 0)
            {
                upCategory.Category.ImageUrl = await UploadImage(upCategory.ImageFile);
            }
            var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + "/Category");
            request.Content = new StringContent(JsonConvert.SerializeObject(upCategory.Category),Encoding.UTF8,"application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Category Updated!";
                return RedirectToAction("Categories");
            }
            else
            {
                TempData["error"] = response.Content.ReadAsStringAsync();
            }
            return View(upCategory);
        }

        public async Task<IActionResult> DeleteCate(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, _client.BaseAddress + $"/Category/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Category Deleted";
            } else
            {
                TempData["error"] = response.Content.ReadAsStringAsync();
            }
            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> SearchNameCate(string query)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/Category/search-name/{query}");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(data);
                return View(categories);
            }
            return RedirectToAction("Categories", "Fgive");
        }
        #endregion

        #region Food

        public async Task<IActionResult> Food(int? page)
        {
            var foodPagination = await GetFoodPagination(page);
            return View(foodPagination);
        }

        [HttpGet]
        public IActionResult CreateFood()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood(UploadCreateFood createFood)
        {
            if (!ModelState.IsValid)
            {
                return View(createFood);
            }
            if(createFood.ImageFile != null && createFood.ImageFile.Length > 0)
            {
                createFood.Food.mainImage = await UploadImage(createFood.ImageFile);
            }
            var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/Food");
            request.Content = new StringContent(JsonConvert.SerializeObject(createFood.Food),Encoding.UTF8,"application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                TempData["success"] = "Added new food!";
                return RedirectToAction("Food");
            }
            else
            {
                TempData["error"] = response.Content.ReadAsStringAsync();
                return View(createFood);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateFood(int id)
        {
            Food food = new Food();
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/Food/{id}");
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                food = JsonConvert.DeserializeObject<Food>(data);
            }
            var updateFood = new UpdateFood() { food = food };
            return View(updateFood);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFood(UpdateFood Update)
        {
            if (!ModelState.IsValid)
            {
                return View(Update);
            }
            if(Update.ImageFile != null && Update.ImageFile.Length > 0)
            {
                Update.food.mainImage = await UploadImage(Update.ImageFile);
            }
            var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + "/Food");
            request.Content = new StringContent (JsonConvert.SerializeObject(Update.food), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Food Updated!";
                return RedirectToAction("Food");
            }
            TempData["error"] = "Fail to Update!";
            return View(Update);
        }

        public async Task<IActionResult> DetailFood(int id)
        {
            Food food = new Food();
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/Food/{id}");
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                food = JsonConvert.DeserializeObject<Food>(data);
            }
            return View(food);
        }

        public async Task<IActionResult> DeleteFood(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, _client.BaseAddress + $"/Food/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Food deleted!";
            }
            return RedirectToAction("Food");
        }

        public IActionResult SearchNameFood(string query)
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

        #endregion


        #region Order
        public async Task<IActionResult> Order()
        {
            var orders = await GetOrders();
            return View(orders);
        }
        public async Task<IActionResult> OrderDetail(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,_client.BaseAddress + $"/Order/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<OrderVM>(data);
                return View(order);
            } else
            {
                TempData["error"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Order");
            }
        }
        public async Task<IActionResult> ViewFoodDetail(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, baseAddress + $"/Order/Order-details/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var orderDetails = JsonConvert.DeserializeObject<IEnumerable<OrderDetails>>(data);
                return View(orderDetails);
            }
            else
            {
                TempData["error"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Order", "FGive");
            }
        }

        public async Task<IActionResult> DoneOrder(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + $"/Order/update/{id}?message=%C4%90%C3%A3%20giao");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["error"] = "Đổi trạng thái thành công!";
                return RedirectToAction("Order", "Fgive");
            }
            TempData["error"] = response.Content.ReadAsStringAsync();
            return RedirectToAction("ViewFoodDetail");
        }
        #endregion







        #region Method
        private async Task<decimal> getProfit()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/Order/Profit");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string profit = await response.Content.ReadAsStringAsync();
                return decimal.Parse(profit);
            }
            return 0;
        }

        private async Task<List<OrderVM>> GetOrders()
        {
            var Orders = new List<OrderVM>();
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/Order");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                Orders = JsonConvert.DeserializeObject<List<OrderVM>>(data);
                return Orders;
            }
            return null;
        }

        private async Task<List<Food>> GetFood()
        {
            var foods = new List<Food>();
            HttpResponseMessage response = _client.GetAsync(baseAddress + $"/Food/?page=1&pageSize=50").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                foods = JsonConvert.DeserializeObject<List<Food>>(data);
                return foods;
            }
            return null;
        }

        private async Task<ProductVM> GetFoodPagination(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            var foods = new List<Food>();
            HttpResponseMessage response = _client.GetAsync(baseAddress + $"/Food/?page={pageNumber}&pageSize={pageSize}").Result;
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
                    pageCount = (int)Math.Ceiling(31 / (double)pageSize)
                };
                return foodPaging;
            }
            return null;
        }

        private async Task<List<Category>> getCategories()
        {
            List<Category> categories = new List<Category>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Category").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                categories = JsonConvert.DeserializeObject<List<Category>>(data);
            }
            return categories;
        }

        private async Task<string> UploadImage(IFormFile file)
        {
            string path = "";
            var uploadImage = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/ImageUpload/upload");
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);
            uploadImage.Content = content;
            var uploadResponse = await _client.SendAsync(uploadImage);
            if (uploadResponse.IsSuccessStatusCode)
            {
                string data = await uploadResponse.Content.ReadAsStringAsync();
                data = data.Remove(0, 1);
                path = data.Remove(data.Length - 1, 1);
            }
            return path;
        }

        #endregion
    }
}
