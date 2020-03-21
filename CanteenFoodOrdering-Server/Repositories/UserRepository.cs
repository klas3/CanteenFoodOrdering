using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenFoodOrdering_Server.Data;

namespace CanteenFoodOrdering_Server.Repository
{
    public class UserRepository : IUserRepository
    {
        private Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public bool IsEmailUnique(string email)
        {
            return _context.Users.SingleOrDefault(user => user.Email == email) == null;
        }
    }
}