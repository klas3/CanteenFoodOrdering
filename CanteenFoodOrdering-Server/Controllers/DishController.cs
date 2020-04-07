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
        private const int maxPhotoFileSize = 400 * 1024;

        public DishController(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Cook")]
        public async Task<IActionResult> CreateDish([FromBody] Dish dish)
        {
            if (ModelState.IsValid)
            {
                if (dish.Photo != null && dish.Photo != "")
                {
                    if (!VerifyDishPhotoSize(dish.Photo, maxPhotoFileSize))
                    {
                        return Problem($"Розмір картинки завелекий. Максимально допустимий розмір: 400 КБ");
                    }
                }

                await _dishRepository.CreateDish(dish);

                return Ok();
            }

            return Problem();
        }

        [HttpPost]
        [Authorize(Roles = "Cook")]
        public async Task<IActionResult> UpdateDish([FromBody] DishInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Photo != null && model.Photo != "")
                {
                    if (!VerifyDishPhotoSize(model.Photo, maxPhotoFileSize))
                    {
                        return Problem($"Розмір картинки завелекий. Максимально допустимий розмір: 400 КБ");
                    }
                }

                Dish dish = await _dishRepository.GetDishById(model.DishId);

                dish.CategoryId = model.CategoryId;
                dish.Name = model.Name;
                dish.Cost = model.Cost;
                dish.Description = model.Description;
                dish.Photo = model.Photo;
                dish.ImageMimeType = model.ImageMimeType;
                dish.Count = model.Count;

                await _dishRepository.UpdateDish(dish);

                return Ok();
            }

            return Problem();
        }

        [HttpPost]
        [Authorize(Roles = "Cook")]
        public async Task<IActionResult> UpdateDishCount([FromBody] DishCountViewModel model)
        {
            if (ModelState.IsValid)
            {
                Dish dish = await _dishRepository.GetDishById(model.DishId);

                dish.Count += model.Count;

                await _dishRepository.UpdateDish(dish);

                return Ok();
            }

            return Problem();
        }

        [HttpPost]
        [Authorize(Roles = "Cook")]
        public async Task<IActionResult> DeleteDishById(int id)
        {
            Dish dish = await _dishRepository.GetDishById(id);

            if (dish != null)
            {
                await _dishRepository.DeleteDish(dish);

                return Ok();
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDishes()
        {
            List<DishInfoViewModel> dishes = new List<DishInfoViewModel>();

            (await _dishRepository.GetDishes()).ForEach(dish =>
            {
                dishes.Add(new DishInfoViewModel 
                {
                    DishId = dish.DishId,
                    CategoryId = dish.CategoryId,
                    CategoryName = dish.Category.Name,
                    Name = dish.Name,
                    Cost = dish.Cost,
                    Description = dish.Description,
                    Photo = dish.Photo,
                    ImageMimeType = dish.ImageMimeType,
                    Count = dish.Count
                });
            });

            return Json(dishes);
        }
        private bool VerifyDishPhotoSize(string photo, int size)
        {
            byte[] image = Convert.FromBase64String(photo);

            if (photo.Length < size)
            {
                return true;
            }

            return false;
        }
    }

}