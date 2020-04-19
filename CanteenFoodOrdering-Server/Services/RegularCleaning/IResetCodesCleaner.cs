using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Services
{
    public interface IResetCodesCleaner
    {
        void ProvideCleaningAsync();
        bool CheckIfTimerIsExecuting();
    }
}
