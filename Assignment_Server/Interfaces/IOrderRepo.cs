using Assignment_Server.Models;

namespace Assignment_Server.Interfaces
{
    public interface IOrderRepo
    {
        Order AddOrder(Order order);
        void AddOrderDetail(IEnumerable<OrderDetail> orderdetails);
    }
}
