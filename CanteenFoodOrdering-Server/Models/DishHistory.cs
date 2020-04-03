using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Models
{
    public class DishHistory
    {
        public int DishHistoryId { get; set; }
        public string Name { get; set; }
        public float Cost { get; set; }
        public string Description { get; set; }

        public virtual ICollection<OrderedDishHistory> OrderedDishHistories { get; set; }
    }
}
