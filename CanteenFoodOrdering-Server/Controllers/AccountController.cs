using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CanteenFoodOrdering_Server.ViewModels;
using System.Security.Claims;
using CanteenFoodOrdering_Server.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace CanteenFoodOrdering_Server.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IUserRepository _userRepository;

        public AccountController
        (
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserRepository userRepository
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Login, loginModel.Password, loginModel.RememberMe, false);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            ModelState.AddModelError("", "Неправильний логін або пароль");
            return Problem(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (await _userRepository.IsEmailUnique(viewModel.Email))
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = viewModel.Login,
                        Email = viewModel.Email
                    };

                    var createUserResult = await _userManager.CreateAsync(user, viewModel.Password);

                    if (createUserResult.Succeeded)
                    {
                        if (!await _roleManager.RoleExistsAsync("Customer"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Administrator"));
                            await _roleManager.CreateAsync(new IdentityRole("Cook"));
                            await _roleManager.CreateAsync(new IdentityRole("Сashier"));
                            await _roleManager.CreateAsync(new IdentityRole("Customer"));
                        }

                        if(viewModel.Role == "Administrator")
                        {
                            await _userManager.AddToRoleAsync(user, "Administrator");
                        }
                        else if (viewModel.Role == "Cook" && User.IsInRole("Administrator"))
                        {
                            await _userManager.AddToRoleAsync(user, "Cook");
                        }
                        else if (viewModel.Role == "Cashier" && User.IsInRole("Administrator"))
                        {
                            await _userManager.AddToRoleAsync(user, "Cashier");
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "Customer");
                        }

                        if (!string.IsNullOrWhiteSpace(user.Email))
                        {
                            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                        }

                        var signInUserResult = await _signInManager.PasswordSignInAsync(viewModel.Login, viewModel.Password, viewModel.RememberMe, false);

                        if (signInUserResult.Succeeded)
                        {
                            return Ok();
                        }
                    }
                    else
                    {
                        foreach (var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Ця пошта вже зареєстрована");
                }
            }

            return Problem(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            var result = await _userManager.ChangePasswordAsync(await _userManager.GetUserAsync(User), model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserRole()
        {
            string roleName = (await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User))).FirstOrDefault();

            if (roleName == null)
            {
                return NotFound();
            }
            
            return Ok(roleName);
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthorizedUserInfo()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                return Json(new UserInfoViewModel { Email = user.Email, Login = user.UserName });
            }

            return NotFound();
        }
    }
}