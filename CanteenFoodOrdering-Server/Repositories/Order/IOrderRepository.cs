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
        Task CreateOrderHistory(OrderHistory order);
        Task<List<Order>> GetCustomerOders(User customer);
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetOrders();
        Task UpdateOrder(Order order);
        Task DeleteOrder(Order order);
    }
}
