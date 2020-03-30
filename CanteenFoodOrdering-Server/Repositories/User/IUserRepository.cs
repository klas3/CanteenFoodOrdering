using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Repositories
{
    public interface IUserRepository
    {
        Task<bool> IsUserNameUnique(string username);
        Task<bool> IsEmailUnique(string email);
        Task<string> GetUserEmailById(string id);
    }
}