using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Repositories;
using CanteenFoodOrdering_Server.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CanteenFoodOrdering_Server.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository _orderRepository;
        private IOrderedDishRepository _orderedDishRepository;
        private IDishRepository _dishRepository;

        public OrderController
        (
            IOrderRepository orderRepository,
            IOrderedDishRepository orderedDishRepository,
            IDishRepository dishRepository
        )
        {
            _orderRepository = orderRepository;
            _orderedDishRepository = orderedDishRepository;
            _dishRepository = dishRepository;
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

        [HttpGet]
        public async Task<IActionResult> GetOrderInfoById(int id)
        {
            return Json(await _orderRepository.GetOrderById(id));
        }

        public async Task<IActionResult> GetFullOrderInfoById(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);

            FullOrderViewModel fullOrder = new FullOrderViewModel
            {
                CreationDate = order.CreationDate,
                DesiredDate = order.DesiredDate,
                Wishes = order.Wishes,
                IsPaid = order.IsPaid
            };

            foreach (OrderedDish orderedDish in await _orderRepository.GetFullOrderById(id))
            {
                Dish dish = await _dishRepository.GetDishById(orderedDish.DishId);

                fullOrder.Dishes.Add(dish);
            }

            return Json(fullOrder);
        }

        [HttpPost]
        [Authorize(Roles = "Cashier")]
        public async Task<IActionResult> UpdateOrderPaymentStatus(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);
            
            if (order != null)
            {
                order.IsPaid = !order.IsPaid;

                await _orderRepository.UpdateOrder(order);

                return Ok();
            }

            return Problem();
        }
    }
}