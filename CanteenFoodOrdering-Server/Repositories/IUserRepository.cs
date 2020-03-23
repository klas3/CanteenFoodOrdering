using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Repository
{
    public interface IUserRepository
    {
        bool IsEmailUnique(string email);
        Task<string> GetUserEmailById(string id);
    }
}