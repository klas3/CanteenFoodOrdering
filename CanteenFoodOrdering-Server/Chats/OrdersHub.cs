using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Chats
{
    public class OrdersHub : Hub
    {
        public async Task SendToCashier(string order)
        {
            await this.Clients.All.SendAsync("SendToCashier", order);
        }
    }
}
