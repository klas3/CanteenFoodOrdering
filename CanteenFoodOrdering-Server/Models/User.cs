using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<Order> Order { get; set; }
    }
}
