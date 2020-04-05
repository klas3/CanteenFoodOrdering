using CanteenFoodOrdering_Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class FullOrderViewModel
    {
        [Required]
        public int OrderId { get; set; }
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime DesiredDate { get; set; }
        public string Wishes { get; set; }
        public bool IsPaid { get; set; }
        [Required]
        public List<Dish> Dishes { get; set; }
    }
}
