using Assignment_Server.Models.Momo;
using Assignment_UI.Models;
using Assignment_UI.ViewModel;
using Assignment_UI.ViewModel.Order;
using Assignment_UI.ViewModel.VNPay;
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
                if(order.PaymentType == "VNPAY")
                {
                    HttpContext.Session.Set<Order>("order", order);
                    return await VNPAY();
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
                return await COD(order);
            }
            TempData["error"] = "Phát sinh lỗi trong quá trình đặt đơn";
            return RedirectToAction("Payment", "Order");
        }
        #endregion

        #region VNPay
        private async Task<IActionResult> VNPAY()
        {
            await getCartDetails();
            List<CartDetail> cartDetails = ViewBag.cartdetails;


            //Get Config Info
            string vnp_Returnurl = "https://localhost:7211/Order/VNPReturn"; //URL nhan ket qua tra ve 
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = "2DQGKKDA"; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = "UMTCVNGYSCWSWVEJCADPUEECDUCVHVNU"; //Secret Key

            //Get payment input
            OrderInfo order = new OrderInfo();
            order.OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            order.Amount = ((long)Math.Round(cartDetails.Sum(x => x.Total))); // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending" khởi tạo giao dịch chưa có IPN
            order.CreatedDate = DateTime.Now;
            //Save order to db

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ":1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày


            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(paymentUrl);
        }

        public async Task<IActionResult> VNPReturn()
        {
            var ResponseCode = HttpContext.Request.Query["vnp_ResponseCode"];
            if(ResponseCode == "00")
            {
                var order = HttpContext.Session.Get<Order>("order");
                order.PaymentStatus = "Đã thanh toán";
                return await COD(order);
            }
            TempData["error"] = "Phát sinh lỗi trong quá trình đặt đơn";
            return RedirectToAction("Payment", "Order");
        }
        #endregion

    }
}
