using Assignment_Server.Models;
using Assignment_Server.Models.DTO.Category;
using Assignment_Server.Models.DTO.Food;
using Assignment_Server.Models.DTO.Order;
using Assignment_Server.Models.DTO.User;

namespace Assignment_Server.Mapper
{
    public static class Mappers
    {
        public static CategoryDTO toCategoryDTO(this Category cate)
        {
            return new CategoryDTO
            {
                CategoryId = cate.CategoryId,
                Name = cate.Name,
                ImageUrl = cate.ImageUrl
            };
        }

        public static FoodDTO toFoodDTO(this Food food)
        {
            return new FoodDTO
            {
                FoodId = food.FoodId,
                Name = food.Name,
                mainImage = food.mainImage,
                UnitPrice = food.UnitPrice,
                View = food.View,
                CategoryID = food.CategoryID
            };
        }

        public static OrderDTO toOrderDTO(this Order order)
        {
            return new OrderDTO
            {
                OrderId = order.OrderId,
                FullName = order.FullName,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                PaymentType = order.PaymentType,
                note = order.note,
                OrderDate = order.OrderDate,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus
            };
        }
    }
}
