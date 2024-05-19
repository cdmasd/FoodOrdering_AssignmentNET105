using Assignment_UI.Models;
using Assignment_UI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Assignment_UI.Controllers
{
    public class AuthController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7294/api");
        readonly HttpClient _client;

        public AuthController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login lg)
        {
            if(ModelState.IsValid)
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(lg), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_client.BaseAddress + "/Auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    string jwtToken = await response.Content.ReadAsStringAsync();
                    var userLogined = JsonConvert.DeserializeObject<UserViewModel>(jwtToken);
                    HttpContext.Session.Set<UserViewModel>("userLogined", userLogined);
                    HttpContext.Session.SetString("Token", userLogined.Token);
                    return RedirectToAction("Index", "Home");

                } else
                {
                    ModelState.AddModelError("wronginfo", "Username or password is not correct");
                    return View(lg);
                }
            }
            return View(lg);
        }
        public async Task<IActionResult> Logout(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/Auth/Logout");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Logout successfully";
                HttpContext.Session.Remove("userLogined");
                HttpContext.Session.Remove("Token");
            } else
            {
                TempData["error"] = "Logout fail";
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
