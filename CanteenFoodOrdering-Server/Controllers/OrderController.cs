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
        [Authorize]
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
        [Authorize(Roles = "Customer, Cashier, Cook")]
        public async Task<IActionResult> GetCustomerFullOrderInfoById(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            Order order = await _orderRepository.GetOrderById(id);

            if (order != null)
            {
                if (order.UserId == user.Id || User.IsInRole("Cook"))
                {
                    return Json(await GetAllFullOrderInfoByOrder(order));
                }

                return NotFound();
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFullOrdersInfo()
        {
            List<FullOrderViewModel> models = new List<FullOrderViewModel>();
            List<Order> orders;

            if (User.IsInRole("Cashier"))
            {
                orders = await _orderRepository.GetOrders();
            }
            else
            {
                orders = await _orderRepository.GetCustomerOders(await _userManager.GetUserAsync(User));

                if (orders == null)
                {
                    return NotFound();
                }
            }

            foreach (Order order in orders)
            {
                models.Add(await GetAllFullOrderInfoByOrder(order));
            }

            foreach (FullOrderViewModel model in models)
            {
                foreach (DishInfoViewModel dishModel in model.Dishes)
                {
                    dishModel.Photo = null;
                    dishModel.ImageMimeType = null;
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

            return NotFound();
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

        private async Task<FullOrderViewModel> GetAllFullOrderInfoByOrder(Order order)
        {
            FullOrderViewModel fullOrder = new FullOrderViewModel
            {
                UserId = order.UserId,
                CreationDate = order.CreationDate,
                DesiredDate = order.DesiredDate,
                Wishes = order.Wishes,
                IsPaid = order.IsPaid,
                Dishes = new List<DishInfoViewModel>()
            };

            foreach (OrderedDish orderedDish in await _orderedDishRepository.GetOrderedDishesByOrderId(order.OrderId))
            {
                Dish dish = await _dishRepository.GetDishById(orderedDish.DishId);

                fullOrder.Dishes.Add(new DishInfoViewModel
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

            return fullOrder;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrderById(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);

            if (order != null)
            {

                if (User.IsInRole("Cashier"))
                {
                    await _orderRepository.DeleteOrder(order);

                    return Ok();
                }
                else
                {
                    //Need to verify UserId in Order
                    await _orderRepository.DeleteOrder(order);

                    return Ok();
                }
            }

            return NotFound();
        }
    }
}