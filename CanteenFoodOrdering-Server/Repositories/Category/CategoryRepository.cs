using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Data;
using CanteenFoodOrdering_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace CanteenFoodOrdering_Server.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private Context _context;

        public CategoryRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateCategory(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _context.Categories.SingleOrDefaultAsync(category => category.CategoryId == id);
        }
    }
}
