using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Models;

namespace CanteenFoodOrdering_Server.Repositories
{
    public interface ICategoryRepository
    {
        Task CreateCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(Category category);
        Task<Category> GetCategoryById(int id);
        Task<List<Category>> GetAllCategories();
    }
}
