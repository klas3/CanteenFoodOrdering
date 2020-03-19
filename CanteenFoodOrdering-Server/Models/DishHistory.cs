using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Models
{
    public class DishHistory
    {
        public int DishHistroyId { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public string Description { get; set; }

        public virtual ICollection<OrderedDishHistory> OrderedDishHistories { get; set; }
    }
}
