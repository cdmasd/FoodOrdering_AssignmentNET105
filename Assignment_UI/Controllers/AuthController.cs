using Assignment_UI.Models;
using Assignment_UI.ViewModel;
using Assignment_UI.ViewModel.Order;
using Microsoft.AspNetCore.Authorization;
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
            if (HttpContext.Session.GetString("Token") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login lg)
        {
            if (ModelState.IsValid)
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(lg), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_client.BaseAddress + "/Auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    string jwtToken = await response.Content.ReadAsStringAsync();
                    var userLogined = JsonConvert.DeserializeObject<UserViewModel>(jwtToken);
                    HttpContext.Session.Set<UserViewModel>("userLogined", userLogined);
                    HttpContext.Session.SetString("Token", userLogined.Token);

                    // Get cart
                    var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, _client.BaseAddress + "/Cart/cart-details");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userLogined.Token);
                    var getCart = await _client.SendAsync(request);
                    var cart = new List<CartDetail>();
                    if (getCart.IsSuccessStatusCode)
                    {
                        string data = await getCart.Content.ReadAsStringAsync();
                        cart = JsonConvert.DeserializeObject<List<CartDetail>>(data);
                        HttpContext.Session.SetString("CartCount", cart.Count.ToString());
                    }
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError("wronginfo", "Username or password is not correct");
                    return View(lg);
                }
            }
            return View(lg);
        }
        public async Task<IActionResult> Logout(string token)
        {
            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, _client.BaseAddress + "/Auth/Logout");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Logout successfully";
                HttpContext.Session.Remove("userLogined");
                HttpContext.Session.Remove("Token");
                HttpContext.Session.Remove("CartCount");
            }
            else
            {
                TempData["error"] = "Logout fail";
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateInfo()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                TempData["error"] = "Please login first";
                return RedirectToAction("Login", "Auth");
            }
            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, _client.BaseAddress + "/Auth/get-info");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<UserInfo>(data);
                var updateinfo = new UpdateUserViewModel
                {
                    UserInfo = userInfo
                };
                return View(updateinfo);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInfo(UpdateUserViewModel updateUser)
        {
            if (ModelState.IsValid)
            {
                if (updateUser.AvatarFile != null && updateUser.AvatarFile.Length > 0)
                {
                    updateUser.UserInfo.avatarUrl = await UploadImage(updateUser.AvatarFile);
                }
                var token = HttpContext.Session.GetString("Token");
                var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + "/Auth/update");
                request.Content = new StringContent(JsonConvert.SerializeObject(updateUser.UserInfo), Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "User updated!";
                }
                else
                {
                    TempData["error"] = await response.Content.ReadAsStringAsync();
                }
            }
            return View(updateUser);
        }

        public async Task<IActionResult> Order()
        {
            var Orders = new List<OrderVM>();
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/Order");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                Orders = JsonConvert.DeserializeObject<List<OrderVM>>(data);
                return View(Orders);
            } else
            {
                ViewBag.none = await response.Content.ReadAsStringAsync();
                return View();
            }
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

        [HttpGet]
        public async Task<IActionResult> LoginGoogle()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/Auth/login-google");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GoogleResponse");
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            // Lấy code từ query string trả về từ Google
            string code = Request.Query["code"];

            var response = await _client.GetAsync(_client.BaseAddress + $"/Auth/signin-google?code={code}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var userLogined = JsonConvert.DeserializeObject<UserViewModel>(content);

                // Lưu thông tin người dùng vào session tương tự như đăng nhập thông thường
                HttpContext.Session.Set<UserViewModel>("userLogined", userLogined);
                HttpContext.Session.SetString("Token", userLogined.Token);

                // Get cart
                var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, _client.BaseAddress + "/Cart/cart-details");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userLogined.Token);
                var getCart = await _client.SendAsync(request);
                var cart = new List<CartDetail>();
                if (getCart.IsSuccessStatusCode)
                {
                    string data = await getCart.Content.ReadAsStringAsync();
                    cart = JsonConvert.DeserializeObject<List<CartDetail>>(data);
                    HttpContext.Session.SetString("CartCount", cart.Count.ToString());
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Xử lý lỗi nếu có
                ModelState.AddModelError("", "Đăng nhập bằng Google thất bại.");
                return View("Login");
            }
        }
    }
}
