using Assignment_UI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

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
        public async Task<IActionResult> Index(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/Cart/GetCart");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);
            var cart = new List<CartDetail>();
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                cart = JsonConvert.DeserializeObject<List<CartDetail>>(data);
                return View(cart);
            }
            else
            {
                TempData["error"] = "Please login first";
            }
            return RedirectToAction("Login", "Auth");
        }
    }
}
