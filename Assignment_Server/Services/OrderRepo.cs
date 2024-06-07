using Assignment_Server.Data;
using Assignment_Server.Data.migrations;
using Assignment_Server.Interfaces;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.Order;

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

        public IEnumerable<OrderDetailDTO> GetOrderDetails(int OrderId)
        {
            var orderDetails = _db.OrderDetails.Where(x => x.OrderId == OrderId).ToList();
            List<OrderDetailDTO> ListDetail = new List<OrderDetailDTO>();
            foreach(var detail in orderDetails)
            {
                Food food = _db.Foods.Find(detail.FoodId);
                OrderDetailDTO detailDTO = new OrderDetailDTO();
                detailDTO.Name = food.Name;
                detailDTO.UnitPrice = detail.UnitPrice;
                detailDTO.Quantity = detail.Quantity;
                detailDTO.Total = detail.Total;
                ListDetail.Add(detailDTO);
            }
            return ListDetail;
        }

        public decimal Profit()
        {
            var profit = _db.OrderDetails.Sum(x => x.Total);
            return profit;
        }

        public void UpdateOrderStatus(int OrderId,string message)
        {
            var order = _db.Orders.SingleOrDefault(Order => Order.OrderId == OrderId);
            order.OrderStatus = message;
            if(message == "Đã giao")
            {
                order.PaymentStatus = "Đã thanh toán";
            }
            _db.Orders.Update(order);
            _db.SaveChanges();
        }

        public IEnumerable<Order> Orders => _db.Orders.ToList();
    }
}
