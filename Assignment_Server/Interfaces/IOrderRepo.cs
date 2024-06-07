using Assignment_Server.Models;
using Assignment_Server.Models.DTO.Order;

namespace Assignment_Server.Interfaces
{
    public interface IOrderRepo
    {
        Order AddOrder(Order order);
        void AddOrderDetail(IEnumerable<OrderDetail> orderdetails);
        IEnumerable<Order> Orders { get; }
        Order Order(int id);
        IEnumerable<Order> getOrderId(string UserId);
        IEnumerable<OrderDetailDTO> GetOrderDetails(int OrderId);
        decimal Profit();
        void UpdateOrderStatus(int OrderId,string message);
    }
}
