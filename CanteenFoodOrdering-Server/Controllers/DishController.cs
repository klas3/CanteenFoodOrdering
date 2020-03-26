using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Repositories;

namespace CanteenFoodOrdering_Server.Controllers
{
    public class DishController : Controller
    {
        private IDishRepository _dishRepository;

        public DishController(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDish([FromBody] Dish dish)
        {
            if (ModelState.IsValid)
            {
                await _dishRepository.CreateDish(new Dish
                {
                    CategoryId = dish.CategoryId,
                    Name = dish.Name,
                    Cost = dish.Cost,
                    Description = dish.Description
                });

                return Ok();
            }

            return Problem();
        }
    }
}