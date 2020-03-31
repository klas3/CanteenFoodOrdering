using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class DishViewModel
    {
        public int DishId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public float Cost { get; set; }
        public string Description { get; set; }
    }
}
