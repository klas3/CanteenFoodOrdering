using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using CanteenFoodOrdering_Server.ViewModels;

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

        public async Task<DishViewModel> GetDishById(int id)
        {
            Dish dish = await _context.Dishes.SingleOrDefaultAsync(dish => dish.DishId == id);

            return new DishViewModel
            {
                DishId = dish.DishId,
                CategoryId = dish.CategoryId,
                Name = dish.Name,
                Cost = dish.Cost,
                Description = dish.Description,
                Photo = Encoding.ASCII.GetString(dish.Photo)
            };
        }

        public async Task<List<DishViewModel>> GetDishes()
        {
            List<DishViewModel> dishes = new List<DishViewModel>();

            (await _context.Dishes.ToListAsync()).ForEach(dish =>
            {
                DishViewModel dishModel = new DishViewModel
                {
                    DishId = dish.DishId,
                    CategoryId = dish.CategoryId,
                    Name = dish.Name,
                    Cost = dish.Cost,
                    Description = dish.Description,
                    Photo = Encoding.ASCII.GetString(dish.Photo)
                };

                dishes.Add(dishModel);
            });

            return dishes;
        }
    }
}
