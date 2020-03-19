using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DesiredDate { get; set; }
        public string Wishes { get; set; }
        public bool IsPaid { get; set; }

        public virtual ICollection<OrderedDish> OrderedDish { get; set; }
    }
}
