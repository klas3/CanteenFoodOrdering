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

        [HttpGet]
        [Authorize(Roles = "Cook, Cashier")]
        public async Task<IActionResult> GetDishById(int id)
        {
            return Json(await _dishRepository.GetDishById(id));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllDishes()
        {
            return Json(await _dishRepository.GetDishes());
        }
    }
}