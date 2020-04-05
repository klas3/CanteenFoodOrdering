using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class DishCountViewModel
    {
        [Required]
        public int DishId { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
