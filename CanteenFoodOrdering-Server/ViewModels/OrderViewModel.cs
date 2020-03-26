using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Models;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Введіть бажану дату приготування")]
        public string DesiredDate { get; set; }
        public string Wishes { get; set; }
        [Required(ErrorMessage = "Виберіть хоча б одну страву")]
        public IEnumerable<int> DishesId { get; set; }
    }
}
