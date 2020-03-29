using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Data;
using Microsoft.EntityFrameworkCore;

namespace CanteenFoodOrdering_Server.Repositories
{
    public class OrderedDishRepository : IOrderedDishRepository
    {
        private Context _context;

        public OrderedDishRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateOrderedDish(OrderedDish orderedDish)
        {
            await _context.OrderedDishes.AddAsync(orderedDish);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderedDish>> GetOrderedDishesByOrderId(int id)
        {
            return await _context.OrderedDishes.Include(orderedDish => orderedDish.Order)
                .Where(orderedDish => orderedDish.Order.OrderId == id)
                .ToListAsync();
        }
    }
}
