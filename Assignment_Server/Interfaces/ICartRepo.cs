using Assignment_Server.Models;

namespace Assignment_Server.Interfaces
{
    public interface ICartRepo
    {
        Cart AddCart(string UserId);
        Cart checkCartExist(string UserId);
        void AddCartDetail(CartDetail detail);
        CartDetail CheckExistFood(int FoodId);
        void UpdateCart(CartDetail detail);
        void DeleteCartDetail(string UserId,int FoodId);
        void DeleteAllCartDetail(string UserId);
        IEnumerable<ListCartDetail> getCart(string UserId);
    }
}
