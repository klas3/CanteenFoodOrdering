using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Models
{
    public class OrderHistory
    {
        public int OrderHistoryId { get; set; }
        public DateTime CompletionDate { get; set; }

        public virtual ICollection<OrderedDishHistory> OrderedDishHistories { get; set; }
    }
}
