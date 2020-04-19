using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Models
{
    public class User : IdentityUser
    {
        public string PushToken { get; set; }
        public string ResetCode { get; set; }
        public TimeSpan ResetCodeCreationTime { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
