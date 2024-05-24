using Assignment_UI.Models;
using Assignment_UI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Assignment_UI.Controllers
{
    public class CartController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7294/api");
        readonly HttpClient _client;

        public CartController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                TempData["error"] = "Please login first";
                return RedirectToAction("Login", "Auth");
            }
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/Cart/cart-details");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);
            var cart = new List<CartDetail>();
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                cart = JsonConvert.DeserializeObject<List<CartDetail>>(data);
            }
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                TempData["error"] = "Please login first";
                return RedirectToAction("Login", "Auth");
            }
            var details = new CartDetailVM() { FoodId = id, Quantity = quantity };
            var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/Cart/cart-details");
            request.Content = new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = await response.Content.ReadAsStringAsync();
            } else
            {
                TempData["error"] = await response.Content.ReadAsStringAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
