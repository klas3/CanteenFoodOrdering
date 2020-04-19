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
        Task<List<Order>> GetOdersByUserId(User customer);
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetOrders();
        Task<List<Order>> GetPaidOrders();
        Task UpdateOrder(Order order);
        Task DeleteOrder(Order order);
        Task<List<Order>> GetUnpaidOrders();
        Task<List<Order>> GetCashierOrders(string userId);
        Task<List<OrderedDishHistory>> GetArchivedOrdersDishes(DateTime date);
    }
}
