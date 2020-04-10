using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Chats
{
    public class OrdersHub : Hub
    {
        private IHubContext<OrdersHub> _context;

        public OrdersHub(IHubContext<OrdersHub> context)
        {
            _context = context;
        }

        [Authorize(Roles = "Customer")]
        public async Task SendToCashier(object order)
        {
            await this.Clients.All.SendAsync("SendToCashier", order);
        }
    }
}
