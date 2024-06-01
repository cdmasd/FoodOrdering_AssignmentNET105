using Assignment_Server.Data;
using Assignment_Server.Data.migrations;
using Assignment_Server.Interfaces;
using Assignment_Server.Models;

namespace Assignment_Server.Services
{
    public class OrderRepo(FoodDbContext db) : IOrderRepo
    {
        FoodDbContext _db = db;
        public Order AddOrder(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
            return order;
        }

        public void AddOrderDetail(IEnumerable<OrderDetail> orderdetails)
        {
            _db.OrderDetails.AddRange(orderdetails);
            _db.SaveChanges();
        }

        public Order Order(int id)
        {
            var order = _db.Orders.Find(id);
            return order;
        }

        public IEnumerable<Order> getOrderId(string UserId)
        {
            var orders = _db.Orders.Where(o => o.UserId == UserId);
            return orders;
        }

        public IEnumerable<Order> Orders => _db.Orders.ToList();
    }
}
