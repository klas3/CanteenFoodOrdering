using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class DishInfoViewModel
    {
        public int DishId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public float Cost { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public string ImageMimeType { get; set; }
        public int Count { get; set; }
    }
}
