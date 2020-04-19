using CanteenFoodOrdering_Server.Data;
using CanteenFoodOrdering_Server.Models;
using CanteenFoodOrdering_Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.Services
{
    public class ResetCodesCleaner : IResetCodesCleaner
    {
        private Timer timer;
        private Context _context;

        private bool isExecuting;

        public ResetCodesCleaner(Context context)
        {
            _context = context;
        }

        public void ProvideCleaningAsync()
        {
            timer = new Timer(ClearAllUsersResetCodes, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            isExecuting = true;
        }

        public bool CheckIfTimerIsExecuting()
        {
            return isExecuting;
        }

        private Task StopCleaningAsync()
        {
            timer?.Dispose();

            return Task.CompletedTask;
        }

        private async void ClearAllUsersResetCodes(object obj)
        {
            List<User> usersWithResetCode = await _context.DBUsers.Where(user => user.ResetCode != "" && user.ResetCode != null).ToListAsync();

            if (usersWithResetCode != null)
            {
                foreach (User user in usersWithResetCode)
                {
                    var time = TimeSpan.Compare(DateTime.Now.TimeOfDay, user.ResetCodeCreationTime);

                    user.ResetCode = null;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                await StopCleaningAsync();
                isExecuting = false;
            }
        }
    }
}
