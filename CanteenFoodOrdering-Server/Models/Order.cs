using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DesiredDate { get; set; }
        public string Wishes { get; set; }
        public float TotalSum { get; set; }
        public bool IsPaid { get; set; }
        public bool IsReady { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderedDish> OrderedDish { get; set; }
    }
}
