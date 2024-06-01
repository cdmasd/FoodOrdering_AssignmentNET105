using Assignment_UI.Models;
using Assignment_UI.ViewModel.Order;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Assignment_UI.Controllers
{
    public class OrderController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7294/api");
        readonly HttpClient _client;
        public OrderController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }


        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                TempData["error"] = "Please login first";
                return RedirectToAction("Login", "Auth");
            }
            await getCartDetails();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Payment(Order order)
        {
            if(ModelState.IsValid)
            {
                if(order.PaymentType == "COD")
                {
                    return await COD(order);
                }
                return View();
            }else
            {
                await getCartDetails();
                return View(order);
            }
        }

        public async Task<IActionResult> ViewOrder(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + $"/Order/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<OrderVM>(data);
                return View(order);
            }
            else
            {
                TempData["error"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Order");
            }
        }

        private async Task<IActionResult> COD(Order order)
        {
            var resquest = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/Order");
            resquest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            resquest.Content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(resquest);
            if (response.IsSuccessStatusCode)
            {
                var deleteCart = new HttpRequestMessage(HttpMethod.Delete, _client.BaseAddress + $"/Cart/cart-details");
                deleteCart.Headers.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var result = await _client.SendAsync(deleteCart);
                if (result.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString("CartCount", "0");
                    TempData["success"] = "Đơn hàng đã được đặt";
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["error"] = "Phát sinh lỗi trong quá trình đặt đơn";
            return RedirectToAction("Payment", "Order");
        }

        private async Task getCartDetails()
        {
            var resquest = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/Cart/cart-details");
            resquest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(resquest);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                ViewBag.cartdetails = JsonConvert.DeserializeObject<List<CartDetail>>(data);
            }
        }

    }
}
