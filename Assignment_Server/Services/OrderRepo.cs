﻿using Assignment_Server.Data;
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

        public void AddOrderDetail(OrderDetail orderdetail)
        {
            _db.OrderDetails.Add(orderdetail);
            _db.SaveChanges();
        }
    }
}