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
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Services;
using System.Threading;

namespace CanteenFoodOrdering_Server.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IUserRepository _userRepository;
        private IEmailSender _emailSender;

        public AccountController
        (
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserRepository userRepository,
            IEmailSender emailSender
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Login, loginModel.Password, true, false);
                if (result.Succeeded)
                {
                    await _userRepository.SetPushTokenToUser(await _userRepository.GetUserByLogin(loginModel.Login), loginModel.PushToken);
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
                    if (await _userRepository.IsUserNameUnique(viewModel.Login))
                    {
                        User user = new User
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
                                await _roleManager.CreateAsync(new IdentityRole("Customer"));
                                await _roleManager.CreateAsync(new IdentityRole("Cash"));
                            }
                            
                            if (viewModel.Role == "Administrator")
                            {
                                await _userManager.AddToRoleAsync(user, "Administrator");
                            }
                            else if (viewModel.Role == "Cook" && User.IsInRole("Administrator"))
                            {
                                await _userManager.AddToRoleAsync(user, "Cook");
                            }
                            else if (viewModel.Role == "Cash" && User.IsInRole("Administrator"))
                            {
                                await _userManager.AddToRoleAsync(user, "Cash");
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, "Customer");
                            }

                            if (!string.IsNullOrWhiteSpace(user.Email))
                            {
                                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                            }

                            if(!User.IsInRole("Administrator"))
                            {
                                var signInUserResult = await _signInManager.PasswordSignInAsync(viewModel.Login, viewModel.Password, true, false);

                                if (signInUserResult.Succeeded)
                                {
                                    await _userRepository.SetPushTokenToUser(await _userManager.FindByIdAsync(user.Id), viewModel.PushToken);
                                    return Ok();
                                }
                            }
                            else
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
                        ModelState.AddModelError("", $"Логін {viewModel.Login} вже зайнятий");
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

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (user.ResetCode == model.ResetCode)
                    {
                        var result = await _userManager.RemovePasswordAsync(user);

                        if (result.Succeeded)
                        {
                            result = await _userManager.AddPasswordAsync(user, model.NewPassword);

                            if (result.Succeeded)
                            {
                                return Ok();
                            }

                            return Problem();
                        }

                        return Problem();
                    }

                    return Problem();
                }

                return NotFound();
            }

            return Problem();
        }

        [HttpPost]
        public async Task<IActionResult> RequestResetPassword([FromBody] SubmitEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    string resetCode = GenerateRandomKey();

                    await _userRepository.AddResetCodeForUser(user, resetCode);
                    await _emailSender.SendEmailAsync(user.Email, "Відновлення паролю", user.UserName, resetCode);

                    return Ok();
                }

                return NotFound();
            }

            return Problem();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (user.ResetCode == model.ResetCode)
                    {
                        if (user.LastResetCodeCreationTime.AddMinutes(5) > DateTime.Now)
                        {
                            return Ok();
                        }

                        await _userRepository.ClearResetCodeForUser(user);

                        return Problem("Ваш код вже недійсний");
                    }

                    return Problem("Ви ввели недійсний код");
                }

                return NotFound();
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
        [Authorize]
        public async Task<IActionResult> GetAuthorizedUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            return Json(new UserInfoViewModel { Email = user.Email, Login = user.UserName });
        }

        [HttpGet]
        public IActionResult CheckIfUserAlreadyAuthorized()
        {
            return Json(User.Identity.IsAuthenticated);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserId()
        {
            return Json((await _userManager.GetUserAsync(User)).Id);
        }

        private string GenerateRandomKey()
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = 8;

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}