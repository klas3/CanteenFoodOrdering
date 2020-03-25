using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CanteenFoodOrdering_Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private Context _context;
        private UserManager<IdentityUser> _userManager;

        public UserRepository(Context context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(user => user.Email == email) == null;
        }

        public async Task<string> GetUserEmailById(string id)
        {
            return (await _userManager.FindByIdAsync(id)).Email;
        }
    }
}