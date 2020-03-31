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
    //[Authorize]
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

        [HttpGet]
        public async Task<IActionResult> GetAllOrdersInfo()
        {
            return Json(await _orderRepository.GetOrders());
        }

        [HttpGet]
        public async Task<IActionResult> GetFullOrderInfoById(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);

            FullOrderViewModel fullOrder = new FullOrderViewModel
            {
                CreationDate = order.CreationDate,
                DesiredDate = order.DesiredDate,
                Wishes = order.Wishes,
                IsPaid = order.IsPaid,
                DishesId = new List<int>()
            };

            foreach (OrderedDish orderedDish in await _orderedDishRepository.GetOrderedDishesByOrderId(id))
            {
                fullOrder.DishesId.Add(orderedDish.DishId);
            }

            return Json(fullOrder);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFullOrdersInfo()
        {
            List<Order> orders = await _orderRepository.GetOrders();
            List<FullOrderViewModel> models = new List<FullOrderViewModel>();

            for (int i = 0; i < orders.Count; i++)
            {
                models.Add(new FullOrderViewModel
                {
                    CreationDate = orders[i].CreationDate,
                    DesiredDate = orders[i].DesiredDate,
                    Wishes = orders[i].Wishes,
                    IsPaid = orders[i].IsPaid,
                    DishesId = new List<int>()
                });

                foreach (OrderedDish orderedDish in await _orderedDishRepository.GetOrderedDishesByOrderId(i + 1))
                {
                    models[i].DishesId.Add(orderedDish.DishId);
                }
            }

            return Json(models);
        }

        [HttpPost]
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