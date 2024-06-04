using Assignment_Server.Interfaces;
using Assignment_Server.Mapper;
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
                var orderDetails = cart.Select(item => new OrderDetail
                {
                    OrderId = createdOrder.OrderId,
                    FoodId = item.Food.FoodId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Food.UnitPrice,
                    Total = item.Total,
                });
                _order.AddOrderDetail(orderDetails);
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _order.Orders.Select(x => x.toOrderDTO());
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOrder([FromRoute]int id)
        {
            var order = _order.Order(id);
            if(order != null)
            {
                return Ok(order.toOrderDTO());
            }
            return BadRequest("Order is not existed");
        }

        [HttpGet("OrderByUser")]
        public IActionResult GetOrderByUser()
        {
            var userId = _usermanager.GetUserId(User);
            var orders = _order.getOrderId(userId);
            if (orders.Any())
                return Ok(orders);
            return BadRequest("Don't have any order");
        }

        [HttpGet("Order-details/{OrderId:int}")]
        public IActionResult GetOrderDetails([FromRoute]int OrderId)
        {
            var orderDetails = _order.GetOrderDetails(OrderId);
            if(orderDetails != null)
            {
                return Ok(orderDetails);
            }
            return BadRequest("Order detail is not exist!");
        }
        [HttpGet("Profit")]
        public decimal getProfit()
        {
            var profit = _order.Profit();
            return profit;
        }
    }
}
