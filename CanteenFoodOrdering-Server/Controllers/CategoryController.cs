using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace CanteenFoodOrdering_Server.Controllers
{
    [Authorize(Roles = "Cook")]
    public class CategoryController : Controller
    {
        private ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (category.Name != null && category.Name != "")
            {
                await _categoryRepository.CreateCategory(new Category
                {
                    Name = category.Name
                });

                return Ok();
            }

            return Problem();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            return Json(await _categoryRepository.GetAllCategories());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            if (category.Name != null && category.Name != "")
            {
                Category categoryModel = await _categoryRepository.GetCategoryById(category.CategoryId);

                if (categoryModel == null)
                {
                    return NotFound();
                }

                if (category.Name == categoryModel.Name)
                {
                    return Problem($"Категорія з назвою {category.Name} вже існує");
                }

                categoryModel.Name = category.Name;

                await _categoryRepository.UpdateCategory(categoryModel);

                return Ok();
            }

            return Problem();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoryById(int id)
        {
            Category category = await _categoryRepository.GetCategoryById(id);

            if (category != null)
            {
                await _categoryRepository.DeleteCategory(category);

                return Ok();
            }

            return NotFound();
        }
    }
}