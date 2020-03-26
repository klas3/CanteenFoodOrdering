using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Data;

namespace CanteenFoodOrdering_Server.Repositories
{
    public class DishRepository : IDishRepository
    {
        private Context _context;

        public DishRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateDish(Dish dish)
        {
            await _context.Dishes.AddAsync(dish);
            await _context.DishHistories.AddAsync(new DishHistory
            {
                Name = dish.Name,
                Cost = dish.Cost,
                Description = dish.Description
            });
            await _context.SaveChangesAsync();
        }
    }
}
