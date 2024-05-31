using Assignment_UI.Models;
using Assignment_UI.ViewModel;
using Microsoft.AspNetCore.Mvc;
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
        #endregion










        #region Method
        public async Task<List<Category>> getCategories()
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
