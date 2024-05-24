using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Assignment_Server.Services
{
    public class CartRepo(UserManager<User> um, FoodDbContext db) : ICartRepo
    {
        readonly UserManager<User> _userManager = um;
        readonly FoodDbContext _db = db;

        public Cart AddCart(string UserId)
        {
            var cart = new Cart()
            {
                UserId = UserId
            };
            _db.Carts.Add(cart);
            _db.SaveChanges();
            return cart;
        }
        public Cart checkCartExist(string UserId)
        {
            var Cart = _db.Carts.SingleOrDefault(x => x.UserId == UserId);
            if (Cart == null)
            {
                Cart = AddCart(UserId);
            }
            return Cart;
        }

        public void AddCartDetail(CartDetail detail)
        {
            _db.CartDetails.Add(detail);
            _db.SaveChanges();
        }

        

        public CartDetail CheckExistFood(int FoodId)
        {
           var detail = _db.CartDetails.SingleOrDefault(x => x.FoodId == FoodId);
            if (detail == null)
                return null;
            return detail;
        }

        public void UpdateCart(CartDetail cartdetail)
        {
                _db.CartDetails.Update(cartdetail);
                _db.SaveChanges();
        }
    }
}
