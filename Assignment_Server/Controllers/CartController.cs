using Assignment_Server.Data;
using Assignment_Server.Mapper;
using Assignment_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Assignment_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(UserManager<User> um, FoodDbContext db) : ControllerBase
    {
        readonly FoodDbContext _db = db;
        private readonly UserManager<User> _usermanager = um;

        [Authorize(Roles = "customer"),HttpPost("{id:int}")]
        public IActionResult AddtoCart([FromRoute]int id, [FromBody]int quantity)
        {
            var userId = _usermanager.GetUserId(User);
            var food = _db.Foods.Find(id);
            if (food == null)
            {
                return NotFound("no existed food id");
            }
            var cart = _db.Carts.SingleOrDefault(x => x.UserId == userId);
            if(cart == null)
            {
                var newCart = new Cart()
                {
                    UserId = userId
                };
                _db.Carts.Add(newCart);
                _db.SaveChanges();
                cart = _db.Carts.SingleOrDefault(x => x.UserId == userId);
            }
            var cartdetail = _db.CartDetails.SingleOrDefault(x => x.FoodId == id);
            if(cartdetail == null)
            {
                var newCartDetail = new CartDetail()
                {
                    CartId = cart.CartId,
                    FoodId = id,
                    Quantity = quantity,
                    Total = food.UnitPrice * quantity
                };
                _db.CartDetails.Add(newCartDetail);
            } else
            {
                cartdetail.Quantity += quantity;
                cartdetail.Total += food.UnitPrice * quantity;
            }
            _db.SaveChanges();
            return StatusCode(201, $"AddToCart FoodId = {id}");
        }




        [Authorize(Roles = "customer"), HttpGet("GetCart")]
        public IActionResult GetCart()
        {
            var user = _usermanager.GetUserId(User);
            var cartDetail = from c in _db.Carts
                             join cd in _db.CartDetails on c.CartId equals cd.CartId
                             join p in _db.Foods on cd.FoodId equals p.FoodId
                             where c.UserId == user
                             select new
                             {
                                 Food = p.toFoodDTO(),
                                 quantity = cd.Quantity,
                                 total = cd.Total
                             };
            return Ok(cartDetail);
        }





        [Authorize(Roles = "customer"),HttpDelete("{id:int}")]
        public IActionResult RemoveFromCart([FromRoute]int id)
        {
            var userId = _usermanager.GetUserId(User);
            var cart = _db.Carts.SingleOrDefault(x => x.UserId ==  userId);
            var delFood = _db.CartDetails.SingleOrDefault(x=> x.FoodId == id && x.CartId == cart.CartId);
            if(delFood != null)
            {
                _db.CartDetails.Remove(delFood);
                _db.SaveChanges();
                return Ok("Deleted");
            }
            return NotFound();
        }



        [Authorize(Roles = "customer"),HttpDelete]
        public IActionResult DeleteCart()
        {
            var userId = _usermanager.GetUserId(User);
            var cart = _db.Carts.SingleOrDefault(x => x.UserId == userId);
           if(cart != null)
            {
                _db.Carts.Remove(cart);
                _db.SaveChanges();
                return Ok("Deleted Cart");
            }
           return NotFound();
        }
    }
}
