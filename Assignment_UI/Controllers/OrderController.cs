using Assignment_Server.Models.Momo;
using Assignment_UI.Models;
using Assignment_UI.ViewModel;
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
                if(order.PaymentType == "MOMO")
                {
                    HttpContext.Session.Set<Order>("order", order);
                    return await Momo();
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
        public async Task<IActionResult> OrderDetail(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,baseAddress + $"/Order/Order-details/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var orderDetails = JsonConvert.DeserializeObject<IEnumerable<OrderDetails>>(data);
                return View(orderDetails);
            } else
            {
                TempData["error"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Order", "Auth");
            }
        }
        public async Task<IActionResult> Cancel(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + $"/Order/update/{id}?message=%C4%90%C3%A3%20hu%E1%BB%B7%20%C4%91%C6%A1n");
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["error"] = "Huỷ đơn hàng thành công!";
                return RedirectToAction("Order", "Auth");
            }
            TempData["error"] = response.Content.ReadAsStringAsync();
            return RedirectToAction("ViewOrder");
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

        #region Momo
        private async Task<IActionResult> Momo()
        {
            await getCartDetails();
            List<CartDetail> cartDetails = ViewBag.cartdetails;
            string accessKey = "M8brj9K6E22vXoDB";
            string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
            MomoRequest momoRequest = new MomoRequest();
            momoRequest.partnerCode = "MOMO5RGX20191128";
            momoRequest.partnerName = "Test Momo API Payment";
            momoRequest.storeId = "Momo Test Store";
            momoRequest.orderInfo = "Payment with momo";
            momoRequest.redirectUrl = "https://localhost:7211/Order/MomoReturn";
            momoRequest.ipnUrl = "https://localhost:7211";
            momoRequest.requestType = "captureWallet";
            momoRequest.amount = ((int)Math.Round(cartDetails.Sum(x => x.Total))).ToString();
            momoRequest.orderId = Guid.NewGuid().ToString();
            momoRequest.requestId = Guid.NewGuid().ToString();
            momoRequest.extraData = "";
            //Before sign HMAC SHA256 signature
            string rawHash = "accessKey=" + accessKey +
                "&amount=" + momoRequest.amount +
                "&extraData=" + momoRequest.extraData +
                "&ipnUrl=" + momoRequest.ipnUrl +
                "&orderId=" + momoRequest.orderId +
                "&orderInfo=" + momoRequest.orderInfo +
                "&partnerCode=" + momoRequest.partnerCode +
                "&redirectUrl=" + momoRequest.redirectUrl +
                "&requestId=" + momoRequest.requestId +
                "&requestType=" + momoRequest.requestType
                ;
            MomoSecurity crypto = new MomoSecurity();
            //sign signature SHA256
            momoRequest.signature = crypto.signSHA256(rawHash, serectkey);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://test-payment.momo.vn/v2/gateway/api/create");
            request.Content = new StringContent(JsonConvert.SerializeObject(momoRequest), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var MomoResponse = JsonConvert.DeserializeObject<MomoResponse>(data);
                return Redirect(MomoResponse.payUrl);
            }
            return NotFound("Something went wrong");
        }

        public async Task<IActionResult> MomoReturn()
        {
            var ResultCode = HttpContext.Request.Query["resultCode"];
            if(ResultCode == "0")
            {
                var order = HttpContext.Session.Get<Order>("order");
                order.PaymentStatus = "Đã thanh toán";
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
            }
            TempData["error"] = "Phát sinh lỗi trong quá trình đặt đơn";
            return RedirectToAction("Payment", "Order");
        }
        #endregion

    }
}
