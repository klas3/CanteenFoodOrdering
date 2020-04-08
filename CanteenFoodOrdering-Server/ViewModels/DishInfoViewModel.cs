using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class DishInfoViewModel
    {
        [Required]
        public int DishId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        [Required]
        public float Cost { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public string ImageMimeType { get; set; }
        public int Count { get; set; }
    }
}
