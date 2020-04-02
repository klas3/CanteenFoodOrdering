using CanteenFoodOrdering_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class FullOrderViewModel
    {
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DesiredDate { get; set; }
        public string Wishes { get; set; }
        public bool IsPaid { get; set; }
        public List<DishInfoViewModel> Dishes { get; set; }
    }
}
