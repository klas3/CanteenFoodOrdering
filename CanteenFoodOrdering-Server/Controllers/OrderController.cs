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
using Microsoft.AspNetCore.Identity;

namespace CanteenFoodOrdering_Server.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private UserManager<User> _userManager;
        private IOrderRepository _orderRepository;
        private IOrderedDishRepository _orderedDishRepository;
        private IDishRepository _dishRepository;

        public OrderController
        (   UserManager<User> userManager,
            IOrderRepository orderRepository,
            IOrderedDishRepository orderedDishRepository,
            IDishRepository dishRepository
        )
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderedDishRepository = orderedDishRepository;
            _dishRepository = dishRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Customer, Cashier")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                Order order = new Order
                {
                    CreationDate = DateTime.Now,
                    DesiredDate = DateTime.Parse(model.DesiredDate),
                    Wishes = model.Wishes,
                    IsPaid = false,
                    UserId = (await _userManager.GetUserAsync(User)).Id
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
                        DishId = dishToOrder.DishId,
                        DishCount = dishToOrder.Count
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
        public async Task<IActionResult> GetAllCustomerOrdersInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Order> orders = await _orderRepository.GetCustomerOders(user);

            if (orders != null)
            {
                return Json(orders);
            }

            return NotFound();
        }

        [HttpGet]
        [Authorize(Roles = "Customer, Cashier, Cook")]
        public async Task<IActionResult> GetOrderInfoById(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            Order order = await _orderRepository.GetOrderById(id);

            if (order != null)
            {
                if (order.UserId == user.Id || User.IsInRole("Cook"))
                {
                    return Json(order);
                }

                return NotFound();
            }

            return NotFound();
        }

        [HttpGet]
        [Authorize(Roles = "Customer, Cashier, Cook")]
        public async Task<IActionResult> GetDishesByOrderId(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            Order order = await _orderRepository.GetOrderById(id);

            if (order != null)
            {
                if (order.UserId == user.Id || User.IsInRole("Cook"))
                {
                    List<DishInfoViewModel> orderDishes = new List<DishInfoViewModel>();

                    foreach (OrderedDish orderedDish in await _orderedDishRepository.GetOrderedDishesByOrderId(id))
                    {
                        Dish dish = await _dishRepository.GetDishById(orderedDish.DishId);

                        orderDishes.Add(new DishInfoViewModel
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
                    }

                    return Json(orderDishes);
                }

                return NotFound();
            }

            return NotFound();
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
                    UserId = orders[i].UserId,
                    CreationDate = orders[i].CreationDate,
                    DesiredDate = orders[i].DesiredDate,
                    Wishes = orders[i].Wishes,
                    IsPaid = orders[i].IsPaid,
                    Dishes = new List<DishInfoViewModel>()
                });

                foreach (OrderedDish orderedDish in await _orderedDishRepository.GetOrderedDishesByOrderId(i + 1))
                {
                    Dish dish = await _dishRepository.GetDishById(orderedDish.DishId);

                    models[i].Dishes.Add(new DishInfoViewModel
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
        [Authorize(Roles = "Cook")]
        public async Task<IActionResult> ArchiveOrderById(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);

            if (order != null)
            {
                OrderHistory orderHistory = new OrderHistory
                {
                    CreationDate = order.CreationDate,
                    DesiredDate = order.DesiredDate,
                    Wishes = order.Wishes,
                    IsPaid = order.IsPaid
                };

                await _orderRepository.CreateOrderHistory(orderHistory);

                foreach (OrderedDish orderedDish in await _orderedDishRepository.GetOrderedDishesByOrderId(id))
                {
                    await _orderedDishRepository.CreateOrderedDishHistory(new OrderedDishHistory
                    {
                        OrderHistoryId = orderHistory.OrderHistoryId,
                        DishHistoryId = (await _dishRepository.GetDishById(orderedDish.DishId)).DishId
                    });
                }

                return Ok();
            }

            return NotFound();
        }
    }
}