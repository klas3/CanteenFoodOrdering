using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Models;

namespace CanteenFoodOrdering_Server.Repositories
{
    public interface IOrderedDishRepository
    {
        Task CreateOrderedDish(OrderedDish orderedDish);
        Task DeleteOrderedDish(OrderedDish orderedDish);
        Task<List<OrderedDish>> GetOrderedDishesByOrderId(int id);
    }
}
