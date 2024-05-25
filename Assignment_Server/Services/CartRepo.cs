using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Mapper;
using Assignment_Server.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Assignment_Server.Services
{
    public class CartRepo(FoodDbContext db) : ICartRepo
    {
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

        public void DeleteCartDetail(string UserId,int FoodId)
        {
            var cart = _db.Carts.SingleOrDefault(x => x.UserId == UserId);
            var delFood = _db.CartDetails.SingleOrDefault(x => x.FoodId == FoodId && x.CartId == cart.CartId);
            if (delFood != null)
            {
                _db.CartDetails.Remove(delFood);
                _db.SaveChanges();
            }
        }

        public void DeleteAllCartDetail(string UserId)
        {
            var cart = _db.Carts.SingleOrDefault(x => x.UserId == UserId);
            var delFood = _db.CartDetails.Where(x => x.CartId == cart.CartId).ToList();
            foreach (var item in delFood)
            {
                _db.CartDetails.Remove(item);
            }
            _db.SaveChanges();
        }

        public IEnumerable<ListCartDetail> getCart(string UserId)
        {
            var cartDetail = from c in _db.Carts
                             join cd in _db.CartDetails on c.CartId equals cd.CartId
                             join p in _db.Foods on cd.FoodId equals p.FoodId
                             where c.UserId == UserId
                             select new ListCartDetail
                             {
                                 Food = p.toFoodDTO(),
                                 Quantity = cd.Quantity,
                                 Total = cd.Total
                             };
            return cartDetail;
        }
    }
}
