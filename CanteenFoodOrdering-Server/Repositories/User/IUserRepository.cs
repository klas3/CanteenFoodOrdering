using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Repositories
{
    public interface IUserRepository
    {
        Task<bool> IsEmailUnique(string email);
        Task<string> GetUserEmailById(string id);
    }
}