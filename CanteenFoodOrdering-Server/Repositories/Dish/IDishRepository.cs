using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.ViewModels;

namespace CanteenFoodOrdering_Server.Repositories
{
    public interface IDishRepository
    {
        Task CreateDish(Dish dish);
        Task<Dish> GetDishById(int id);
        Task<List<Dish>> GetDishes();
        Task UpdateDish(Dish dish);
    }
}
