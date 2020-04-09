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
        [Authorize(Roles="Customer")]
        public async Task SendToCashier(object order)
        {
            await Clients.All.SendAsync("SendToCashier", order);
        }

        [Authorize(Roles = "Cash")]
        public async Task SendToCook(object order)
        {
            await Clients.All.SendAsync("SendToCook", order);
        }
    }
}
