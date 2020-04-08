using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Data;
using CanteenFoodOrdering_Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CanteenFoodOrdering_Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private Context _context;
        private UserManager<User> _userManager;

        public UserRepository(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> IsUserNameUnique(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(user => user.UserName == username) == null;
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(user => user.Email == email) == null;
        }

        public async Task<string> GetUserEmailById(string id)
        {
            return (await _userManager.FindByIdAsync(id)).Email;
        }

        public async Task ChangePasswordHash(User user, string newPasswordHash)
        {
            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
        }

        public async Task SetPushTokenToUser(User user, string pushToken)
        {
            user.PushToken = pushToken;
            await _context.SaveChangesAsync();
        }

        public string GenerateRandomKey()
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = 8;

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<User> GetUserById(string id)
        {
            return await _context.DBUsers.SingleOrDefaultAsync(u => u.Id == id);
        }
    }
}