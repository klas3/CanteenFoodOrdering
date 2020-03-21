using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введіть ваш Логін")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Введіть ваш пароль")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
