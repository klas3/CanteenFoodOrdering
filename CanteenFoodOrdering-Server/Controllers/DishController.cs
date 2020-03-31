using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using CanteenFoodOrdering_Server.ViewModels;

namespace CanteenFoodOrdering_Server.Controllers
{
    [Authorize]
    public class DishController : Controller
    {
        private IDishRepository _dishRepository;

        public DishController(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateDish([FromBody] Dish dish)
        {
            if (ModelState.IsValid)
            {
                await _dishRepository.CreateDish(dish);

                return Ok();
            }

            return Problem();
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetDishById(int id)
        {
            return Json(await _dishRepository.GetDishById(id));
        }

        [HttpGet]
        [Authorize(Roles = "Cook, Cashier, Customer")]
        public async Task<IActionResult> GetAllDishes()
        {
            return Json(await _dishRepository.GetDishes());
        }
    }
}