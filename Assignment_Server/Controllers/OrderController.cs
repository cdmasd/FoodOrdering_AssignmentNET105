using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderRepo order, UserManager<User> userManager, ICartRepo cartrepo) : ControllerBase
    {
        readonly IOrderRepo _order = order;
        readonly UserManager<User> _usermanager = userManager;
        readonly ICartRepo _cartRepo = cartrepo;

        [HttpPost]
        public async Task<IActionResult> Payment(CreateOrderDTO create)
        {
            var userId = _usermanager.GetUserId(User);
            var cart = _cartRepo.getCart(userId);

            if (ModelState.IsValid)
            {
                Order order = new Order
                {
                    UserId = userId,
                    FullName = create.FullName,
                    Address = create.Address,
                    PaymentType = create.PaymentType,
                    PhoneNumber = create.PhoneNumber,
                    note = create.note
                };
                var createdOrder = _order.AddOrder(order);
                List<OrderDetail> orderDetail = new List<OrderDetail>();
                foreach (var item in cart)
                {
                    var newitem = new OrderDetail
                    {
                        OrderId = createdOrder.OrderId,
                        FoodId = item.Food.FoodId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Food.UnitPrice,
                        Total = item.Total,
                    };
                    orderDetail.Add(newitem);
                }
                foreach(var item in orderDetail)
                {
                    _order.AddOrderDetail(item);
                }
                return Ok();
            }
            return BadRequest(ModelState);
        }
    }
}
