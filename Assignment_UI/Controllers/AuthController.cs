using Assignment_UI.Models;
using Assignment_UI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
                string data = JsonConvert.SerializeObject(lg);
                StringContent content = new StringContent((data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Auth/login", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    string jwtToken = await response.Content.ReadAsStringAsync();
                    var userLogined = JsonConvert.DeserializeObject<UserViewModel>(jwtToken);
                    HttpContext.Session.Set<UserViewModel>("userLogined", userLogined);
                    return RedirectToAction("Index", "Home");

                } else
                {
                    ModelState.AddModelError("wronginfo", "Username or password is not correct");
                    return View(lg);
                }
            }
            return View(lg);
        }
        public IActionResult Logout()
        {
            return View();
        }
    }
}
