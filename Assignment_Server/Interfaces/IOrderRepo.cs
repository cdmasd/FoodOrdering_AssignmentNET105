using Assignment_Server.Models;

namespace Assignment_Server.Interfaces
{
    public interface IOrderRepo
    {
        Order AddOrder(Order order);
        void AddOrderDetail(IEnumerable<OrderDetail> orderdetails);
        IEnumerable<Order> Orders { get; }
        Order Order(int id);
        IEnumerable<Order> getOrderId(string UserId);
    }
}
