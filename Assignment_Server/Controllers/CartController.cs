using Assignment_Server.Data;
using Assignment_Server.Mapper;
using Assignment_Server.Models.DTO;
using Assignment_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Assignment_Server.Models.DTO.Cart;
using Assignment_Server.Interfaces;

namespace Assignment_Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(UserManager<User> um, ICartRepo db, IFoodRepo food, FoodDbContext dbb) : ControllerBase
    {
        readonly FoodDbContext _db = dbb;
        readonly ICartRepo _cart = db;
        private readonly UserManager<User> _usermanager = um;
        private IFoodRepo _food = food;

        [HttpPost("cart-details")]
        public IActionResult AddtoCart([FromBody]CartDetailDTO detail)
        {
            var userId = _usermanager.GetUserId(User);
            var food = _food.GetById(detail.FoodId);
            if (food == null)
            {
                return BadRequest("no existed food id");
            }
            var cart = _cart.checkCartExist(userId);
            var cartdetail = _cart.CheckExistFood(detail.FoodId);
            if(cartdetail == null)
            {
                var newCartDetail = new CartDetail()
                {
                    CartId = cart.CartId,
                    FoodId = detail.FoodId,
                    Quantity = detail.Quantity,
                    Total = food.UnitPrice * detail.Quantity
                };
                _cart.AddCartDetail(newCartDetail);
            } else
            {
                cartdetail.Quantity += detail.Quantity;
                cartdetail.Total += food.UnitPrice * detail.Quantity;
                _cart.UpdateCart(cartdetail);
            }
            return StatusCode(201, $"Food {detail.FoodId} added");
        }




        [HttpGet("cart-details")]
        public IActionResult GetCart()
        {
            var user = _usermanager.GetUserId(User);
            var cartDetail = _cart.getCart(user);
            return Ok(cartDetail);
        }





        [HttpDelete("cart-details/{id:int}")]
        public IActionResult RemoveFromCart([FromRoute]int id)
        {
            var userId = _usermanager.GetUserId(User);
            _cart.DeleteCartDetail(userId, id);
            return Ok("Removed");
        }



        [HttpDelete("cart-details")]
        public IActionResult DeleteAllFromCartDetail()
        {
            try
            {
                var userId = _usermanager.GetUserId(User);
                _cart.DeleteAllCartDetail(userId);
                return Ok("All removed");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
