using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "Поле Email обов'язкове.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Поле Логін обов'язкове.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Поле Пароль обов'язкове.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Role { get; set; }
        public bool RememberMe { get; set; }
    }
}