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
using System.Net.Http;
using CanteenFoodOrdering_Server.Chats;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;

namespace CanteenFoodOrdering_Server.Controllers
{
    public class OrderController : Controller
    {
        private UserManager<User> _userManager;
        private IOrderRepository _orderRepository;
        private IOrderedDishRepository _orderedDishRepository;
        private IDishRepository _dishRepository;
        private IUserRepository _userRepository;
        private OrdersHub _ordersHub;
        private IConfiguration _configuration;

        public OrderController
        (   UserManager<User> userManager,
            IOrderRepository orderRepository,
            IOrderedDishRepository orderedDishRepository,
            IDishRepository dishRepository,
            IUserRepository userRepository,
            IHubContext<OrdersHub> hubContext,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderedDishRepository = orderedDishRepository;
            _dishRepository = dishRepository;
            _userRepository = userRepository;
            _ordersHub = new OrdersHub(hubContext);
            _configuration = configuration;
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
                    TotalSum = 0,
                    IsPaid = false,
                    IsReady = false,
                    UserId = (await _userManager.GetUserAsync(User)).Id
                };

                foreach (DishCountViewModel dishToOrder in model.Dishes)
                {
                    Dish dish = await _dishRepository.GetDishById(dishToOrder.DishId);

                    order.TotalSum += dish.Cost * dishToOrder.Count;
                }

                await _orderRepository.CreateOrder(order);

                foreach (DishCountViewModel dishToOrder in model.Dishes)
                {
                    await _orderedDishRepository.CreateOrderedDish(new OrderedDish
                    {
                        OrderId = order.OrderId,
                        DishId = dishToOrder.DishId,
                        DishCount = dishToOrder.Count
                    });
                }

                await _ordersHub.SendToCashier(await GetAllFullOrderInfoByOrder(await _orderRepository.GetOrderById(order.OrderId)));

                return Ok();
            }

            return Problem();
        }

        [HttpGet]
        [Authorize(Roles = "Customer, Cash, Cook")]
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
        [Authorize]
        public async Task<IActionResult> GetAllFullOrdersInfo()
        {
            List<FullOrderViewModel> models = new List<FullOrderViewModel>();
            List<Order> orders;

            if (User.IsInRole("Administrator"))
            {
                orders = await _orderRepository.GetUnpaidOrders();
            }
            else if (User.IsInRole("Cook"))
            {
                orders = await _orderRepository.GetPaidOrders();
            }
            else if (User.IsInRole("Cash"))
            {
                orders = await _orderRepository.GetCashierOrders((await _userManager.GetUserAsync(User)).Id);
            }
            else
            {
                orders = await _orderRepository.GetOdersByUserId(await _userManager.GetUserAsync(User));

                if (orders == null)
                {
                    return Json(orders);
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
        [Authorize(Roles = "Cash, Administrator")]
        public async Task<IActionResult> SetToTrueOrderPaymentStatusById(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);
            
            if (order != null)
            {
                order.IsPaid = true;

                await _orderRepository.UpdateOrder(order);

                await _ordersHub.RemoveOnCashier(id);
                await _ordersHub.SendToCook(await GetAllFullOrderInfoByOrder(order));

                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Cook")]
        public async Task<IActionResult> SetToTrueOrderReadyStatudById(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);

            if (order != null)
            {
                if (order.IsPaid)
                {
                    order.IsReady = true;

                    await _orderRepository.UpdateOrder(order);

                    await SendPushNotification(order.UserId, order.OrderId);

                    return Ok();
                }

                return Problem("Замовлення ще не оплачене");
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
                    CompletionDate = DateTime.Now
                };

                await _orderRepository.DeleteOrder(order);
                await _orderRepository.CreateOrderHistory(orderHistory);

                foreach (OrderedDish orderedDish in await _orderedDishRepository.GetOrderedDishesByOrderId(id))
                {
                    await _orderedDishRepository.CreateOrderedDishHistory(new OrderedDishHistory
                    {
                        OrderHistoryId = orderHistory.OrderHistoryId,
                        DishHistoryId = (await _dishRepository.GetDishById(orderedDish.DishId)).DishId
                    });
                }

                await _ordersHub.RemoveOnCook(id);

                return Ok();
            }

            return NotFound();
        }

        private async Task<FullOrderViewModel> GetAllFullOrderInfoByOrder(Order order)
        {
            FullOrderViewModel fullOrder = new FullOrderViewModel
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                CreationDate = order.CreationDate,
                DesiredDate = order.DesiredDate,
                Wishes = order.Wishes,
                TotalSum = order.TotalSum,
                IsPaid = order.IsPaid,
                IsReady = order.IsReady,
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
                    Count = orderedDish.DishCount
                });
            }

            return fullOrder;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteOrderById(int id)
        {
            Order order = await _orderRepository.GetOrderById(id);

            if (order != null)
            {
                if(User.IsInRole("Customer"))
                {
                    if (order.UserId != (await _userManager.GetUserAsync(User))?.Id)
                    {
                        return Problem();
                    }

                    if(order.IsPaid)
                    {
                        return Problem();
                    }
                }

                await _orderRepository.DeleteOrder(order);

                if (!order.IsPaid)
                {
                    await _ordersHub.RemoveOnCashier(id);
                }
                else
                {
                    await _ordersHub.RemoveOnCook(id);
                }

                return Ok();
            }

            return NotFound();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaymentData(int orderId)
        {
            Order order = await _orderRepository.GetOrderById(orderId);

            if ((await _userManager.GetUserAsync(User)).Id == order.UserId && !order.IsPaid)
            {
                string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new PaymentJson
                {
                    action = "pay",
                    amount = order.TotalSum.ToString(),
                    description = $"Оплата замовлення №{orderId}",
                    version = "3",
                    order_Id = orderId.ToString(),
                    currency = "UAH",
                    public_key = "i77133712504",
                    server_url = $"https://canteenfoodordering-server.azurewebsites.net/Order/PayForOrder/{orderId}"
                })));

                return Json(new PaymentData
                {
                    data = data,
                    signature = GenerateSignature(data)
                });
            }

            return Problem();
        }

        [HttpPost]
        [Route("{controller}/{action}/{orderId}")]
        public async Task<IActionResult> PayForOrder(int orderId, string signature, string data)
        {
            signature = signature.Replace(' ', '+');
            
            if(GenerateSignature(data) == signature)
            {
                if((JsonConvert.DeserializeObject<PaymentStatus>(Encoding.UTF8.GetString(Convert.FromBase64String(data)))).Status == "success")
                {
                    Order order = await _orderRepository.GetOrderById(orderId);
                    order.IsPaid = true;
                    await _orderRepository.UpdateOrder(order);
                }
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetArchivedOrders(DateTime date)
        {
            return Json(await _orderRepository.GetArchivedOrdersDishes(date));
        }

        private string GenerateSignature(string data)
        {
            string privateKey = "WwnkpnDCwSNHncFvNCbT3oBmoTVyGY7z4NJ5dVzT";
            return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{privateKey}{data}{privateKey}")));
        }

        private async Task SendPushNotification(string userId, int orderId)
        {
            try
            {
                await (new HttpClient()).PostAsync(
                    "https://exp.host/--/api/v2/push/send", 
                    new StringContent($"{{ \"to\": \"{(await _userRepository.GetUserById(userId))?.PushToken}\", \"title\": \"Замовлення\", \"body\": \"Ваше замовлення №{orderId} чекає на вас.\", \"sound\": \"default\" }}", Encoding.UTF8, "application/json")
                );
            }
            catch { }
        }
    }
}