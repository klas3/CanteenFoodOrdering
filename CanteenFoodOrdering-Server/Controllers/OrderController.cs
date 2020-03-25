using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Repositories;
using CanteenFoodOrdering_Server.ViewModels;

namespace CanteenFoodOrdering_Server.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository _orderRepository;
        private IOrderedDishRepository _orderedDishRepository;

        public OrderController
        (
            IOrderRepository orderRepository,
            IOrderedDishRepository orderedDishRepository
        )
        {
            _orderRepository = orderRepository;
            _orderedDishRepository = orderedDishRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                Order order = new Order
                {
                    CreationDate = DateTime.Now,
                    DesiredDate = DateTime.Parse(model.DesiredDate),
                    Wishes = model.Wishes,
                    IsPaid = false
                };

                await _orderRepository.CreateOrder(order);

                foreach (int dishId in model.DishesId)
                {
                    await _orderedDishRepository.CreateOrderedDish(new OrderedDish
                    {
                        OrderId = order.OrderId,
                        DishId = dishId
                    });
                }

                return Ok();
            }

            return Problem();
        }
    }
}