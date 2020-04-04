using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Repositories;
using CanteenFoodOrdering_Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace CanteenFoodOrdering_Server.Controllers
{
    [Authorize]
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
        [Authorize(Roles = "Customer")]
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

                foreach (DishCountViewModel dishToOrder in model.Dishes)
                {
                    Dish dish = await _dishRepository.GetDishById(dishToOrder.DishId);

                    if (dishToOrder.Count > dish.Count)
                    {
                        return Problem($"Недостатня кількість страви: {dish.Name}. Вибрано: {dishToOrder.Count}. В наявності: {dish.Count}");
                    }
                }

                await _orderRepository.CreateOrder(order);

                foreach (DishCountViewModel dishToOrder in model.Dishes)
                {
                    Dish dish = await _dishRepository.GetDishById(dishToOrder.DishId);

                    await _orderedDishRepository.CreateOrderedDish(new OrderedDish
                    {
                        OrderId = order.OrderId,
                        DishId = dishToOrder.DishId
                    });

                    dish.Count -= dishToOrder.Count;

                    await _dishRepository.UpdateDish(dish);
                }

                return Ok();
            }

            return Problem();
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetFullOrderInfoById(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);

            FullOrderViewModel fullOrder = new FullOrderViewModel
            {
                CreationDate = order.CreationDate,
                DesiredDate = order.DesiredDate,
                Wishes = order.Wishes,
                IsPaid = order.IsPaid,
                Dishes = new List<Dish>()
            };

            foreach (OrderedDish orderedDish in await _orderedDishRepository.GetOrderedDishesByOrderId(id))
            {
                fullOrder.Dishes.Add(await _dishRepository.GetDishById(orderedDish.DishId));
            }

            return Json(fullOrder);
        }

        [HttpGet]
        [Authorize(Roles = "Cook, Cashier")]
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
                    Dishes = new List<Dish>()
                });

                foreach (OrderedDish orderedDish in await _orderedDishRepository.GetOrderedDishesByOrderId(i + 1))
                {
                    models[i].Dishes.Add(await _dishRepository.GetDishById(orderedDish.DishId));
                }
            }

            return Json(models);
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

        [HttpPost]
        public async Task<IActionResult> DeleteOrderById(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);

            if (order != null)
            {
                List<OrderedDish> orderedDishes = await _orderedDishRepository.GetOrderedDishesByOrderId(id);

                foreach (OrderedDish orderedDish in orderedDishes)
                {
                    await _orderedDishRepository.DeleteOrderedDish(orderedDish);
                }

                await _orderRepository.DeleteOrder(order);

                return Ok();
            }

            return NotFound();
        }
    }
}