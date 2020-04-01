using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Data;
using CanteenFoodOrdering_Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CanteenFoodOrdering_Server.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private Context _context;

        public OrderRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _context.Orders.SingleOrDefaultAsync(order => order.OrderId == id);
        }

        public async Task<List<Order>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }
    }
}
