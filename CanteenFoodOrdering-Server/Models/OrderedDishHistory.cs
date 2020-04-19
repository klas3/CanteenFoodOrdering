using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Models
{
    public class OrderedDishHistory
    {
        public int OrderedDishHistoryId { get; set; }
        public int OrderHistoryId { get; set; }
        public int DishHistoryId { get; set; }
        public int DishCount { get; set; }

        public virtual OrderHistory OrderHistory { get; set; }
        public virtual DishHistory DishHistory { get; set; }
    }
}
