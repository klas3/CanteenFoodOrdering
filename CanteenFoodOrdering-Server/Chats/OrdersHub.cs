using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Chats
{
    [Authorize]
    public class OrdersHub : Hub
    {
        private IHubContext<OrdersHub> _context;

        public OrdersHub(IHubContext<OrdersHub> context)
        {
            _context = context;
        }

        [Authorize(Roles="Customer")]
        public async Task SendToCashier(object order)
        {
            await _context.Clients.All.SendAsync("SendToCashier", order);
        }

        [Authorize(Roles = "Cash")]
        public async Task SendToCook(object order)
        {
            await _context.Clients.All.SendAsync("SendToCook", order);
        }

        [Authorize(Roles = "Cash")]
        public async Task RemoveOnCashier(object id)
        {
            await _context.Clients.All.SendAsync("RemoveOnCashier", id);
        }

        [Authorize(Roles = "Cook")]
        public async Task RemoveOnCook(object id)
        {
            await _context.Clients.All.SendAsync("RemoveOnCook", id);
        }
    }
}
