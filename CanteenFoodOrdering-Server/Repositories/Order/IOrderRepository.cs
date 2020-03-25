using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Models;

namespace CanteenFoodOrdering_Server.Repositories
{
    public interface IOrderRepository
    {
        Task CreateOrder(Order order);
        Task<Order> GetOrderById(int id);
        Task SetOrderPaymentStatus(bool status, int orderId);
    }
}
